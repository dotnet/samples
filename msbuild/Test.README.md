# How to test a MSBuild Custom Task

A really important item when we create a MSBuild Custom Task, which is going to be distributed, is to ensure the correctness of the code.
The way to be confident about that is testing it.
For information about the benefits of doing tests and basic test tooling, see [basics about unit tests](https://docs.microsoft.com/visualstudio/test/walkthrough-creating-and-running-unit-tests-for-managed-code).
We are going to use examples which have already been developed. The following projects includes unit and integration MSBuild Custom Tasks testing

1. [Custom Task-Code Generation](./custom-task-code-generation/)
1. [The Rest-Api client Generation - Option 2 - MSBuild Tool Task](./rest-api-client-generation/)

## Unit Test

A MSBuild Custom Task is a class which inherits from MSBuild Task (directly or indirectly, because MSBuild Tool Task is a MSBuild Task). The method which generates the action is `Execute()`.
We have some input values (parameters), and output parameters which we will be able to assert.
In our case some input parameters are paths to files, so we generated test input files on a folder called _Resources_. Our MSBuild task also generates files, so we are going to assert the generated files.

:white_check_mark: A build engine is needed, a class which implements [IBuildEngine](https://docs.microsoft.com/dotnet/api/microsoft.build.framework.ibuildengine?view=msbuild-17-netcore). In our example we created a mock using [Moq](https://github.com/Moq/moq4/wiki/Quickstart), but you can use other mock tools. I was interesting in collecting the errors, but you can collect other information and then assert it.
The Engine Mock is needed on all the tests, so it was included as _TestInitialize_ (it is going to be executed before each test, and each test will have its own build engine). [Complete example](./custom-task-code-generation/AppSettingStronglyTyped/AppSettingStronglyTyped.Test/AppSettingStronglyTypedTest.cs)

```csharp
private Mock<IBuildEngine> buildEngine;
private List<BuildErrorEventArgs> errors;

[TestInitialize()]
public void Startup()
{
    buildEngine = new Mock<IBuildEngine>();
    errors = new List<BuildErrorEventArgs>();
    buildEngine.Setup(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>())).Callback<BuildErrorEventArgs>(e => errors.Add(e));
}
```

Now we need to create our Task and set the parameters as part of the test arrangement.

```csharp
//Arrange
var item = new Mock<ITaskItem>();
item.Setup(x => x.GetMetadata("Identity")).Returns($".\\Resources\\complete-prop.setting");

var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = "MyCompletePropSetting", SettingNamespaceName = "MyNamespace", SettingFiles = new[] { item.Object } };

appSettingStronglyTyped.BuildEngine = buildEngine.Object;
```

First, we create the ITaskItem parameter mock (using [Moq](https://github.com/Moq/moq4/wiki/Quickstart)), and point to the file to be parsed. Then, we create our _AppSettingStronglyTyped_ Custom Task with its parameters. Finally, we set the build engine to our MSBuild Custom Task.

At this point we need to do the action

```csharp
//Act
var success = appSettingStronglyTyped.Execute();
```

Last but not least, we need to assert the expected outcome from our test

```csharp
//Assert
Assert.IsTrue(success); // The execution was success
Assert.AreEqual(errors.Count, 0); //Not error were found
Assert.AreEqual($"MyCompletePropSetting.generated.cs", appSettingStronglyTyped.ClassNameFile); // The Task expected output
Assert.AreEqual(true, File.Exists(appSettingStronglyTyped.ClassNameFile)); // The file was generated
Assert.IsTrue(File.ReadLines(appSettingStronglyTyped.ClassNameFile).SequenceEqual(File.ReadLines(".\\Resources\\complete-prop-class.txt"))); // Assenting the file content
```

Following this pattern, you should expand all the possibilities.
:warning: When there are files generated, we need to use different file name for each test to avoid collision. Remember to delete the generated files as test cleanup.

## Integration Test

Unit tests are important, but we would like to test our Custom MSBuild task in a real build context.

[System.Diagnostics.Process Class](https://docs.microsoft.com/dotnet/api/system.diagnostics.process?view=net-6.0) provides access to local and remote processes and enables you to start and stop local system processes.
We are going to run a real build on a unit test using test MSBuild files.

We need to initialize the execution context for each test. Pay attention to ensure the path to _dotnet_ command is accurate for your environment. The complete example is [here](./custom-task-code-generation/AppSettingStronglyTyped/AppSettingStronglyTyped.Test/AppSettingStronglyTypedIntegrationTest.cs)

```csharp
public const string MSBUILD = "C:\\Program Files\\dotnet\\dotnet.exe";

private Process buildProcess;
private List<string> output;

[TestInitialize()]
public void Startup()
{
    output = new List<string>();
    buildProcess = new Process();
    buildProcess.StartInfo.FileName = MSBUILD;
    buildProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    buildProcess.StartInfo.CreateNoWindow = true;
    buildProcess.StartInfo.RedirectStandardOutput = true;
}
```

On cleanup, we need to finish the process

```csharp
[TestCleanup()]
public void Cleanup()
{
    buildProcess.Close();
}
```

Now we need to create each test. Each test will need its own msbuild file definition to be executed. For example [testscript-success.msbuild](./custom-task-code-generation/AppSettingStronglyTyped/AppSettingStronglyTyped.Test/Resources/testscript-success.msbuild). For understanding the file please read [Custom Task-Code Generation](./custom-task-code-generation/).

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <UsingTask TaskName="AppSettingStronglyTyped.AppSettingStronglyTyped" AssemblyFile="..\AppSettingStronglyTyped.dll" />
    <PropertyGroup>
        <TargetFramework>netstandard2.1</TargetFramework>
    </PropertyGroup>

    <PropertyGroup>
        <SettingClass>MySettingSuccess</SettingClass>
        <SettingNamespace>example</SettingNamespace>
    </PropertyGroup>

    <ItemGroup>
        <SettingFiles Include="complete-prop.setting" />
    </ItemGroup>

    <Target Name="generateSettingClass">
        <AppSettingStronglyTyped SettingClassName="$(SettingClass)" SettingNamespaceName="$(SettingNamespace)" SettingFiles="@(SettingFiles)">
            <Output TaskParameter="ClassNameFile" PropertyName="SettingClassFileName" />
        </AppSettingStronglyTyped>
    </Target>
</Project>
```

Our test arrangement will be the indication to build this MSBuild file.

```csharp
//Arrage
buildProcess.StartInfo.Arguments = "build .\\Resources\\testscript-success.msbuild /t:generateSettingClass";
```

Now, we are going to execute and get the output.

```csharp
//Act
ExecuteCommandAndCollectResults();
```

Where `ExecuteCommandAndCollectResults()` is defined as:

```csharp
private void ExecuteCommandAndCollectResults()
{
    buildProcess.Start();
    while (!buildProcess.StandardOutput.EndOfStream)
    {
        output.Add(buildProcess.StandardOutput.ReadLine() ?? string.Empty);
    }
    buildProcess.WaitForExit();
}
```

Last but not least, we are going to assess the expected result.

```csharp
//Assert
Assert.AreEqual(0, buildProcess.ExitCode); //Finished success
Assert.IsTrue(File.Exists(".\\Resources\\MySettingSuccess.generated.cs")); // the expected resource was generated
Assert.IsTrue(File.ReadLines(".\\Resources\\MySettingSuccess.generated.cs").SequenceEqual(File.ReadLines(".\\Resources\\testscript-success-class.txt"))); // asserting the file content
```

## Conclusion

Testing is the only way to ensure the correctness. Unit test is really useful because you can test and debug all the scenarios easily, but having some, at least some basic, integration test is key to ensure the task executes in a build context.
In this article we put on the table how to test MSBuild Custom Task.
