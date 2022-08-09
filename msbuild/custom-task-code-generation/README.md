# Custom Task-Code Generation

If you are not clear on terms such as tasks, targets, properties, or runtimes, you could first check out the docs that explain these concepts, starting with the [MSBuild Concepts article](https://docs.microsoft.com/visualstudio/msbuild/msbuild-concepts).

The basic idea of the current example is defined as:

```text
Input text => Generation => Output C# (Some code generation)
```

We are going to create a MSBuild custom task named AppSettingStronglyTyped. The task is going to read a set of text files, and each file with lines with the following format:

```text
propertyName:type:defaultValue
```

Then our code will generate a C# class with all the constants. :innocent: This is not useful at all, it is simple, the idea is help us to learn the mechanism.
A problem should stop the build and give us enough information.

## Step 1, create the AppSettingStronglyTyped project

Create a Class Library Net Standard. The Framework should be .Net Standard 2.0.

:warning: Before we go too far, you must first understand the different between “full” MSBuild (the one that powers Visual Studio) and “portable” MSBuild, or the one bundled in the .NET Core Command Line.

- Full MSBuild: This version of MSBuild usually lives inside Visual Studio. Runs on .NET Framework. Visual Studio uses this when you execute “Build” on your solution or project.
- Dotnet MSBuild: This version of MSBuild is bundled in the .NET Core Command Line. Runs on .NET Core. Visual Studio does not directly invoke this version of MSBuild. Currently only supports projects that build using Microsoft.NET.Sdk.

If you want to share code between .NET Framework and any other .NET implementation, such as .NET Core, your library should target [.NET Standard 2.0](https://docs.microsoft.com/dotnet/standard/net-standard), and we want to run inside Visual Studio which runs on .NET Framework. .NET Framework doesn't support .NET Standard 2.1.

## Step 2, create the AppSettingStronglyTyped MSBuild Custom Task

We need to create our MSBuild CustomTask. Information about how to [write MSBuild custom task](https://docs.microsoft.com/visualstudio/msbuild/task-writing), it is good information to understand the following steps.

We need to include _Microsoft.Build.Utilities.Core_ NuGet package, and then create a AppSettingStronglyTyped derived from Microsoft.Build.Utilities.Task.

We are going use three parameters:

```csharp
//The name of the class which is going to be generated
[Required]
public string SettingClassName { get; set; }

//The name of the namespace where the class is going to be generated
[Required]
public string SettingNamespaceName { get; set; }

//List of files which we need to read with the defined format: 'propertyName:type:defaultValue' per line
[Required]
public ITaskItem[] SettingFiles { get; set; }
```

The task is going to process the _SettingFiles_ and generate a class 'SettingNamespaceName.SettingClassName'. The class will have a set of constants based on the text file's content.
The task output will be:

```csharp
//The filename where the class was generated
[Output]
public string ClassNameFile { get; set; }
```

We need to override the Execute method. The execute method returns true if the task was successful and false in other cases. Task implements ITask and provides default implementations of some ITask members and additionally, logging is easier. It is important the log to know what is going on. And even more important if we are going to return not succeed (false). On error, we should use Log.LogError.

```csharp
public override bool Execute()
{
    //Read the input files and return a IDictionary<string, object> with the properties to be created.
    //Any format error it will return not succeed and Log.LogError properly
    var (success, settings) = ReadProjectSettingFiles();
    if (!success)
    {
        return !Log.HasLoggedErrors;
    }
    //Create the class based on the Dictionary
    success = CreateSettingClass(settings);

    return !Log.HasLoggedErrors;
}
```

Then, the details are really not important for our purpose. You can copy from the source code and improve if you like.

:shipit:Food for thought. We are generating C# code during build process as example.The task is like any other C# class, you could do whatever you want. For example, sending an email, generating change log, reading a GitHub repository. This is the power of MSBuild custom tasks.

### Step 3, Change the AppSettingStronglyTyped.csproj

We need to make some changes on the project file. Now we have something simple like

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Build.Utilities.Core" Version="17.0.0" />
  </ItemGroup>

</Project>
```

We are going to generate a NuGet package, so first we need to add some basic information

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
        <version>1.0.0</version>
        <title>AppSettingStronglyTyped</title>
        <authors>John Doe</authors>
        <description>Generates a strongly typed setting class base on a txt file</description>
        <tags>MyTags</tags>
        <copyright>Copyright ©Microsoft Company 2022</copyright>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Build.Utilities.Core" Version="17.0.0" />
  </ItemGroup>

</Project>
```

#### Mark dependencies as private

The dependencies of your MSBuild task must be packaged inside the package; they cannot be expressed as normal package references. The package won't expose any regular dependencies to external users. This takes two steps to accomplish: marking your assemblies as private and actually embedding them in the generated package. For this example, we'll assume that your task depends on `Microsoft.Extensions.DependencyInjection` to work, so add a `PackageReference` to `Microsoft.Extensions.DependencyInjection` at version `6.0.0`.

```xml
<ItemGroup>
   <PackageReference
      Include="Microsoft.Build.Utilities.Core"
      Version="17.0.0" />
   <PackageReference
      Include="Microsoft.Extensions.DependencyInjection"
      Version="6.0.0" />
</ItemGroup>
```

Now, mark every dependency of this Task project, both `PackageReference` and `ProjectReference`, with the `PrivateAssets="all"` attribute. This tells NuGet not to expose these dependencies to consuming projects at all. You can read more about controlling dependency assets [in the NuGet documentation](https://docs.microsoft.com/nuget/consume-packages/package-references-in-project-files#controlling-dependency-assets).

```xml
<ItemGroup>
   <PackageReference
     Include="Microsoft.Build.Utilities.Core"
     Version="17.0.0"
     PrivateAssets="all"
   />
   <PackageReference
     Include="Microsoft.Extensions.DependencyInjection"
     Version="6.0.0"
     PrivateAssets="all"
    />
</ItemGroup>
```

### Bundle dependencies into the package

Then, the dependencies of your MSBuild task must be packaged inside the package, they cannot be expressed as normal PackageReferences. We don't expose any regular dependencies to the outside world. It is not needed for the current example, because we don't have extra dependencies, but it is worth being aware of this for other scenarios.

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
        <version>1.0.0</version>
        <title>AppSettingStronglyTyped</title>
        <authors>John</authors>
        <description>Generates a strongly typed setting class base on a txt file</description>
        <tags>MyTags</tags>
        <copyright>Copyright ©Contoso 2022</copyright>
        <!-- we need the assemblies bundled, so set this so we don't expose any dependencies to the outside world -->
        <GenerateDependencyFile>true</GenerateDependencyFile>
        <TargetsForTfmSpecificBuildOutput>$(TargetsForTfmSpecificBuildOutput);CopyProjectReferencesToPackage</TargetsForTfmSpecificBuildOutput>
        <DebugType>embedded</DebugType>
        <IsPackable>true</IsPackable>
    </PropertyGroup>

    <ItemGroup>
      <PackageReference
        Include="Microsoft.Build.Utilities.Core"
        Version="17.0.0"
        PrivateAssets="all" />
      <PackageReference
        Include="Microsoft.Extensions.DependencyInjection"
        Version="6.0.0"
        PrivateAssets="all" />
    </ItemGroup>

    <ItemGroup>
        <!-- These lines pack the build props/targets files to the `build` folder in the generated package.
         By convention, the .NET SDK will look for build\<Package Id>.props and build\<Package Id>.targets
         for automatic inclusion in the build. -->
        <Content Include="build\AppSettingStronglyTyped.props" PackagePath="build\" />
        <Content Include="build\AppSettingStronglyTyped.targets" PackagePath="build\" />
    </ItemGroup>

    <Target Name="CopyProjectReferencesToPackage" DependsOnTargets="ResolveReferences">
        <ItemGroup>
            <!-- The dependencies of your MSBuild task must be packaged inside the package, they cannot be expressed as normal PackageReferences -->
            <BuildOutputInPackage
                Include="@(ReferenceCopyLocalPaths)"
                TargetPath="%(ReferenceCopyLocalPaths.DestinationSubPath)" />
        </ItemGroup>
    </Target>

    <!-- This target adds the generated deps.json file to our package output -->
    <Target Name="AddBuildDependencyFileToBuiltProjectOutputGroupOutput"
            BeforeTargets="BuiltProjectOutputGroup"
            Condition=" '$(GenerateDependencyFile)' == 'true'">

       <ItemGroup>
          <BuiltProjectOutputGroupOutput
              Include="$(ProjectDepsFilePath)"
              TargetPath="$(ProjectDepsFileName)"
              FinalOutputPath="$(ProjectDepsFilePath)" />
       </ItemGroup>
    </Target>

</Project>

```

### Step 4, Include MSBuild props and targets in a package

We recommend first reading the basics about [properties and targets](https://docs.microsoft.com/visualstudio/msbuild/customize-your-build) and then how to [include properties and targets for a NuGet package](https://docs.microsoft.com/nuget/create-packages/creating-a-package#include-msbuild-props-and-targets-in-a-package).

In some cases, you might want to add custom build targets or properties in projects that consume your package, such as running a custom tool or process during a build. You do this by placing files in the form <package_id>.targets or <package_id>.props within the \build folder of the project.
Files in the root \build folder are considered suitable for all target frameworks.
In this next step we’ll wire up the task implementation in a .props and .targets file, which will be included in our NuGet package and automatically loaded from a referencing project.
First, we should modify the AppSettingStronglyTyped.csproj, adding

```xml
<ItemGroup>
    <!-- these lines pack the build props/targets files to the `build` folder in the generated package.
      by convention, the .NET SDK will look for build\<Package Id>.props and build\<Package Id>.targets
      for automatic inclusion in the build. -->
    <Content Include="build\AppSettingStronglyTyped.props" PackagePath="build\" />
    <Content Include="build\AppSettingStronglyTyped.targets" PackagePath="build\" />
</ItemGroup>
```

Then we must create a _build_ folder and inside two text files: _AppSettingStronglyTyped.props_ and _AppSettingStronglyTyped.targets_.
AppSettingStronglyTyped.props is imported very early in Microsoft.Common.props, and properties defined later are unavailable to it. So, avoid referring to properties that are not yet defined (and will evaluate to empty).

Directory.Build.targets is imported from Microsoft.Common.targets after importing .targets files from NuGet packages. So, it can override properties and targets defined in most of the build logic, or set properties for all your projects regardless of what the individual projects set. You can see the [import order](https://docs.microsoft.com/visualstudio/msbuild/customize-your-build#import-order).
_AppSettingStronglyTyped.props_ includes the task and define some property with default values:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <!--defining properties interesting for my task-->
    <PropertyGroup>
        <!--The folder where the custom task will be present. It points to inside the NuGet package. -->
        <CustomTasksFolder>$(MSBuildThisFileDirectory)..\tasks\netstandard2.0</CustomTasksFolder>
        <!--Reference to the assembly which contains the MSBuild Task-->
        <CustomTasksAssembly>$(CustomTasksFolder)\$(MSBuildThisFileName).dll</CustomTasksAssembly>
    </PropertyGroup>

    <!--Register our custom task-->
    <UsingTask TaskName="$(MSBuildThisFileName).$(MSBuildThisFileName)" AssemblyFile="$(CustomTasksAssembly)"/>

    <!--Task parameters default values, this can be overridden-->
    <PropertyGroup>
        <RootFolder Condition="'$(RootFolder)' == ''">$(MSBuildProjectDirectory)</RootFolder>
        <SettingClass Condition="'$(SettingClass)' == ''">MySetting</SettingClass>
        <SettingNamespace Condition="'$(SettingNamespace)' == ''">example</SettingNamespace>
        <SettingExtensionFile Condition="'$(SettingExtensionFile)' == ''">mysettings</SettingExtensionFile>
    </PropertyGroup>
</Project>
```

Beyond the [build properties](https://docs.microsoft.com/visualstudio/msbuild/walkthrough-using-msbuild#build-properties) defined, actually, an important part of this file is the task registration. MSBuild must know how to locate and run the assembly that contains the task class. Tasks are registered using the [UsingTask element (MSBuild)](https://docs.microsoft.com/visualstudio/msbuild/usingtask-element-msbuild). TaskName is the name of the task to reference from the assembly. This attribute should always specify full namespaces. AssemblyFile is the file path of the assembly.

The _AppSettingStronglyTyped.props_ will be automatically included when the package is installed, then our client has the task available and some default values. However, it is never used. In order to put this code in action we need to define some targets on _AppSettingStronglyTyped.targets_ file which also will be also automatically included when the package is installed:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

    <!--Defining all the text files input parameters-->
    <ItemGroup>
        <SettingFiles Include="$(RootFolder)\*.$(SettingExtensionFile)" />
    </ItemGroup>

    <!--It is generated a target which is executed before the compilation-->
    <Target Name="BeforeCompile" Inputs="@(SettingFiles)" Outputs="$(RootFolder)\$(SettingClass).generated.cs">
        <!--Calling our custom task-->
        <AppSettingStronglyTyped SettingClassName="$(SettingClass)" SettingNamespaceName="$(SettingNamespace)" SettingFiles="@(SettingFiles)">
            <Output TaskParameter="ClassNameFile" PropertyName="SettingClassFileName" />
        </AppSettingStronglyTyped>
        <!--Our generated file is included to be compiled-->
        <ItemGroup>
            <Compile Remove="$(SettingClassFileName)" />
            <Compile Include="$(SettingClassFileName)" />
        </ItemGroup>
    </Target>

    <!--The generated file is deleted after a general clean. It will force the regeneration on rebuild-->
    <Target Name="AfterClean">
        <Delete Files="$(RootFolder)\$(SettingClass).generated.cs" />
    </Target>
</Project>
```

The first step is the creation of an [InputGroup](https://docs.microsoft.com/visualstudio/msbuild/msbuild-items) which represents the text files (there could be more than one) to read and it will be some of our task parameters. There are defaults for the location and the extension to search for, but you can override the values defining the properties on the client MSBuild project file.

Then we define two [MSBuild targets](https://docs.microsoft.com/visualstudio/msbuild/msbuild-targets). We [extends the MSBuild process](https://docs.microsoft.com/visualstudio/msbuild/how-to-extend-the-visual-studio-build-process) overriding predefined targets:

1. BeforeCompile: The goal is to call our custom task to generate the class and include the class to be compiled. Tasks are inserted before core compilation is done. Input and Output fields are related to [incremental build](https://docs.microsoft.com/visualstudio/msbuild/incremental-builds). If all output items are up-to-date, MSBuild skips the target. This incremental build of the target can significantly improve the build speed. An item is considered up-to-date if its output file is the same age or newer than its input file or files.
1. AfterClean: The goal is to delete the generated class file after a general clean happens. Tasks that are inserted after the core clean functionality is invoked. It forces the generation on MSBuild rebuild target execution.

### Step 5, Generates the NuGet package

We can use Visual Studio (Right-click on the project and select 'pack').
We can also do it by command line. Move to the folder where the AppSettingStronglyTyped.csproj is present, and execute:

```dotnetcli
//-o is to define the output, we are choose the current folder
dotnet pack -o .
```

Congrats!! You must have `\AppSettingStronglyTyped\AppSettingStronglyTyped\AppSettingStronglyTyped.1.0.0.nupkg` generated.

.nupkg files are a zip file. You can open with a zip tool. On the build folder the .target and .props files must be present. On lib\netstandard2.0\ folder the .dll file must be present. On the root must be the AppSettingStronglyTyped.nuspec file.

### Step 6, Generate console app and test our new MSBuild task

Now, we are going to create a standard .NET Core console app for testing the NuGet package generated.
:warning: We need to avoid generating a MSBuild custom task in the same MSBuild process which is going to consume it. The new project should be in a completely different Visual Studio Solution or the new project should use a pre-generated DLL and relocated from the standard output.
We could call MSBuildConsoleExample the new project on a new Visual Studio Solution.
We must import the AppSettingStronglyTyped NuGet. We need to define a new package source and define a local folder as package source, [please follow the instructions](https://docs.microsoft.com/nuget/consume-packages/install-use-packages-visual-studio#package-sources). Then copy our NuGet package on that folder and install it on our console app.

Then, we should rebuild to be sure everything is ok.

At this point we are going to create our text file with the extension defined to be discovered. Using the default extension we are going to create MyValues.mysettings on the root, and add the following content:

```text
Greeting:string:Hello World!
```

Now, we are going to rebuild again and the magic should happen, the generated file must be there. If you are using the standards you must see the _MySetting.generated.cs_ file on your solution.

The class _MySetting_ is in the _example_ namespace, we are going to redefine to use our app namespace. Open csproj and add

```text
    <PropertyGroup>
        <SettingNamespace>MSBuildConsoleExample</SettingNamespace>
    </PropertyGroup>
```

Now, we are going to rebuild again and the class is on _MSBuildConsoleExample_ namespace. In this way you can redefine the generated class name(SettingClass), the text extension files(SettingExtensionFile) to be use as input and the location (RootFolder) of them if you like.

Go to Program.cs and change the hardcoded 'Hello Word!!' to our constant

```csharp
        static void Main(string[] args)
        {
            Console.WriteLine(MySetting.Greeting);
        }
```

We can execute the program, it will greet our generated class.

_Note:_ It is a good practice include **PrivateAssets="All"** on the PackageReference. The project which use the MSBuild Custom Task as NuGet. It should look like

```xml
<ItemGroup>
        <PackageReference Include="AppSettingStronglyTyped" Version="1.0.0" PrivateAssets="All"/>
</ItemGroup>
```

It means that assets will be consumed but won't flow to the parent project. [More information](https://docs.microsoft.com/nuget/consume-packages/package-references-in-project-files#controlling-dependency-assets)

### Step 7 (Optional), Check what is going on during build process

It is possible to compile using a command-line command. We need to go to the MSBuildConsoleExample\MSBuildConsoleExample folder.
We are going to use the -bl (binary log) option to generate a binary log. The binary log will have very useful information to know what is going on during the build process.

```dotnet
# Using Dotnet MSBuild (run core environment)
dotnet build -bl

# or Full MSBuild (run on net framework environment, this is used by Visual Studio)
msbuild -bl
```

Both of them will generate a log msbuild.binlog, and it can be opened with [this tool](https://msbuildlog.com/)
The option `/t:rebuild` means run the rebuild target. It will force regeneration.

### Development

During development and debugging it could be hard to ship your custom task as a NuGet package.
It could be easier to include all the information on properties and targets directly in your MSBuildConsoleExample.csproj and then move to the NuGet shipping format.
For example (Note that the NuGet package is not referenced):

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <UsingTask TaskName="AppSettingStronglyTyped.AppSettingStronglyTyped" AssemblyFile="..\..\AppSettingStronglyTyped\AppSettingStronglyTyped\bin\Debug\netstandard2.0\AppSettingStronglyTyped.dll"/>

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net6.0</TargetFramework>
        <RootFolder>$(MSBuildProjectDirectory)</RootFolder>
        <SettingClass>MySetting</SettingClass>
        <SettingNamespace>MSBuildConsoleExample</SettingNamespace>
        <SettingExtensionFile>mysettings</SettingExtensionFile>
    </PropertyGroup>

    <ItemGroup>
        <SettingFiles Include="$(RootFolder)\*.mysettings" />
    </ItemGroup>

    <Target Name="GenerateSetting" BeforeTargets="CoreCompile" Inputs="@(SettingFiles)" Outputs="$(RootFolder)\$(SettingClass).generated.cs">
        <AppSettingStronglyTyped SettingClassName="$(SettingClass)" SettingNamespaceName="$(SettingNamespace)" SettingFiles="@(SettingFiles)">
            <Output TaskParameter="ClassNameFile" PropertyName="SettingClassFileName" />
        </AppSettingStronglyTyped>
        <ItemGroup>
            <Compile Remove="$(SettingClassFileName)" />
            <Compile Include="$(SettingClassFileName)" />
        </ItemGroup>
    </Target>

    <Target Name="ForceReGenerateOnRebuild" AfterTargets="CoreClean">
        <Delete Files="$(RootFolder)\$(SettingClass).generated.cs" />
    </Target>
</Project>
```

_Note:_ We are using another way to order the targets [(BeforeTarget and AfterTarget)](https://docs.microsoft.com/visualstudio/msbuild/target-build-order#beforetargets-and-aftertargets). The note in the [Override predefined targets](https://docs.microsoft.com/visualstudio/msbuild/how-to-extend-the-visual-studio-build-process#override-predefined-targets) section in the MSBuild extension article says: 'SDK-style projects have an implicit import of targets after the last line of the project file. This means that you cannot override default targets unless you specify your imports manually.'
