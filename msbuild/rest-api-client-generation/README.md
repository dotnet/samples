# Rest Api Client Generation

Nowadays, an application which consumes RestApi is a very common scenario. We are going to generate the Rest API client automatically during the build process. We will use [NSwag](https://docs.microsoft.com/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-6.0&tabs=visual-studio)

Our app will be a console app, because it is simpler. The kind of app which use the Rest Api client is not important.
The example will consume the public [Pet Store API](https://petstore.swagger.io), which publish the [OpenAPI spec](https://petstore.swagger.io/v2/swagger.json)

If you are not clear on terms such as tasks, targets, properties, or runtimes, you could first check out the docs that explain these concepts, starting with the [MSBuild Concepts article](https://docs.microsoft.com/visualstudio/msbuild/msbuild-concepts).

## Option 1: Use pre defined MSBuid Exec Task

We will use the ["Exec" MSBuild task](https://docs.microsoft.com/dotnet/api/microsoft.build.tasks.exec?view=msbuild-17-netcore), which simply invokes the specified process with the specified arguments, waits for it to complete, and then returns True if the process completed successfully, and False if an error occurred.

NSwag code generation is possible to be used from MSBuild, by [NSwag.MSBuild](https://github.com/RicoSuter/NSwag/wiki/NSwag.MSBuild)

The complete code version is in this PetReaderExecTaskExample folder, you can download and take a look. Anyway, we are going to go through step by step and explain some concepts on the way.

- We are going to create a new console application on Visual Studio named PetReaderExecTaskExample. We use .NET 6.0.
- Create another project in the same solution: PetShopRestClient (This is going to contain the generated client as a Library). We use netstandard 2.1. The generated client doesn't compile on netstandard 2.0.
- Go to the PetReaderExecTaskExample project, and add a project dependence to PetShopRestClient project.
- On PetShopRestClient, include the following NuGet packages
  - Nswag.MSBuild, it will allow us access to the code generator from MSBuild
  - Newtonsoft.Json, it will be needed to compile the generated client
  - System.ComponentModel.Annotations, it will be needed to compile the generated client
- On PetShopRestClient, add a folder (named PetShopRestClient) for the code generation and delete the Class1.cs automatically generated.
- Create a text file named petshop-openapi-spec.json (on root). We are going to add the OpenApi spec, please copy the content from [here](https://petstore.swagger.io/v2/swagger.json) inside the file. Why did we commit the spec instead of reading it online?, we like a reproducible build that depends only from the input, consuming the API directly could transform a build which works today into a build which fails tomorrow from the same source. The picture saved on petshop-openapi-spec.json will allow us to still have a version which builds even if the spec changes.
- Now, the most important part. We are going to modify PetShopRestClient.csproj and add a [MSBuild targets](https://docs.microsoft.com/visualstudio/msbuild/msbuild-targets) to generate the client during build process.

  - First, we are going to add some props useful for our client generation

    ```xml
    <PropertyGroup>
        <PetOpenApiSpecLocation>petshop-openapi-spec.json</PetOpenApiSpecLocation>
        <PetClientClassName>PetShopRestClient</PetClientClassName>
        <PetClientNamespace>PetShopRestClient</PetClientNamespace>
        <PetClientOutputDirectory>PetShopRestClient</PetClientOutputDirectory>
    </PropertyGroup>
    ```

  - Please add the following targets:

    ```xml
    <Target Name="generatePetClient" BeforeTargets="CoreCompile" Inputs="$(PetOpenApiSpecLocation)" Outputs="$(PetClientOutputDirectory)\$(PetClientClassName).cs">
         <Exec Command="$(NSwagExe) openapi2csclient /input:$(PetOpenApiSpecLocation)  /classname:$(PetClientClassName) /namespace:$(PetClientNamespace) /output:$(PetClientOutputDirectory)\$(PetClientClassName).cs" ConsoleToMSBuild="true">
                <Output TaskParameter="ConsoleOutput" PropertyName="OutputOfExec" />
      </Exec>
    </Target>
    <Target Name="forceReGenerationOnRebuild" AfterTargets="CoreClean">
        <Delete Files="$(PetClientOutputDirectory)\$(PetClientClassName).cs"></Delete>
    </Target>
    ```

    You can notice we are using [BeforeTarget and AfterTarget](https://docs.microsoft.com/visualstudio/msbuild/target-build-order#beforetargets-and-aftertargets) as way to define build order.
    The first target called "generatePetClient" will be executed before the core compilation target, so we will create the source before the compiler executes. The input and output parameter are related to [Incremental Build](https://docs.microsoft.com/visualstudio/msbuild/how-to-build-incrementally). MSBuild can compare the timestamps of the input files with the timestamps of the output files and determine whether to skip, build, or partially rebuild a target.
    After installing the NSwag.MSBuild NuGet package in your project, you can use the variable $(NSwagExe) in your .csproj file to run the NSwag command line tool in an MSBuild target. This way the tools can easily be updated via NuGet. Here we are using the _Exec MSBUild Task_ to execute the NSwag program with the required parameters to generate the client Rest Api. [More about NSwag command and parameters](https://github.com/RicoSuter/NSwag/wiki/NSwag.MSBuild).
    You can capture output from `<Exec>` addig ConsoleToMsBuild="true" to your `<Exec>` tag and then capture the output using the ConsoleOutput parameter in an `<Output>` tag. ConsoleOutput returns the output as an Item. Whitespace is trimmed. ConsoleOutput is enabled when ConsoleToMSBuild is true.
    The second target called "forceReGenerationOnRebuild" deletes the generated class during clean up to force the regeneration on rebuild target execution. This target runs after core clean MSBuild pre defined target.

- At this point in time we can execute a Visual Studio Solution rebuild and see the client generated on the PetShopRestClient folder.
- We are going to use the generated client. Go to the client Program.cs and copy the following code

  ```csharp
  using System;
  using System.Net.Http;

  namespace PetReaderExecTaskExample
  {
      internal class Program
      {
          private const string baseUrl = "https://petstore.swagger.io/v2";
          static void Main(string[] args)
          {
              HttpClient httpClient = new HttpClient();
              httpClient.BaseAddress = new Uri(baseUrl);
              var petClient = new PetShopRestClient.PetShopRestClient(httpClient);
              var pet = petClient.GetPetByIdAsync(1).Result;
              Console.WriteLine($"Id: {pet.Id} Name: {pet.Name} Status: {pet.Status} CategoryName: {pet.Category.Name}");
          }
      }
  }
  ```

  _Note:_ We are using `new HttpClient()` because it's simple and shows our example working, but it is not appropriate in real-world code. The best practice is to use HTTPClientFactory to create an HTTPClient object which addresses the known issues of HTTPClient request like Resource Exhaustion or Stale DNS problems. [Read more about it](https://docs.microsoft.com/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)

- Congrats!! Now, you can execute the program to see how it is working

## Option 2: Use the Custom task derived from MSBuid Tool Task

In many cases the option 1 is good enough to execute an external tools to do something, like Rest Api Client Code Generation.
We are going to continue with the same example, but the ideas can be used for others examples.
What if we want to allow Rest Api Client Code Generation if only if we don't use an absolute Windows path as input? Or What if we need to calculate in some way where the executable is?
When there is any situation where we need execute some code to do extra work, the [MSBuild Tool Task](https://docs.microsoft.com/dotnet/api/microsoft.build.utilities.tooltask) is the best solution. This is an abstract class derivated from MSBuild Task, we need to define a concrete subclass (_We will need to create a Custom MSBuild Task_). It is prepare for command execution and allows us to introduce code during the process.

We are going to generate a Custom Task derived from [MSBuild Tool Task](https://docs.microsoft.com/dotnet/api/microsoft.build.utilities.tooltask) which will generate a Rest API client but it will fail if we try to reference the OpenApi spec using a http address. NSwag supports a http address as OpenApi spec input, but we don't want to allow that. It is recommended read about [Custom task code generation](../custom-task-code-generation/) first, that article contains useful information about Custom Task creation.

The complete code version is in this PetReaderToolTaskExample folder, you can download and take a look. Anyway, we are going to go through step by step and explain some concepts on the way.

- We are going to create a new Visual Studio Project for the Custom Task. We will call it "RestApiClientGenerator" and it must be library C# netstandard2.0. The solution name will be "PetReaderToolTaskExample"
- Delete Class1.cs automatically generated
- Add _Microsoft.Build.Utilities.Core_ NuGet package
- Create a class called "RestApiClientGenerator"
- Inherit MSBuild Tool Task and implement abstract method, we will get something like

  ```csharp
  using Microsoft.Build.Utilities;

  namespace RestApiClientGenerator
  {
      public class RestApiClientGenerator : ToolTask
      {
          protected override string ToolName => throw new System.NotImplementedException();

          protected override string GenerateFullPathToTool()
          {
              throw new System.NotImplementedException();
          }
      }
  }
  ```

- Adding parameters

  - InputOpenApiSpec, where the spec is
  - ClientClassName, name of the generated class
  - ClientNamespaceName, namespace where the class is generated
  - FolderClientClass, path to the folder where the class will be located
  - NSwagCommandFullPath, full path to the directory where NSwag.exe is located

  ```csharp
  [Required]
  public string InputOpenApiSpec { get; set; }
  [Required]
  public string ClientClassName { get; set; }
  [Required]
  public string ClientNamespaceName { get; set; }
  [Required]
  public string FolderClientClass { get; set; }
  [Required]
  public string NSwagCommandFullPath { get; set; }
  ```

- We need to install [NSwag command line tool](https://github.com/RicoSuter/NSwag/releases). Then we need the full path to the directory where NSwag.exe is located.
- Implementing the abstract methods

  ```csharp
  protected override string ToolName => "RestApiClientGenerator";

  protected override string GenerateFullPathToTool()
  {
      return $"{NSwagCommandFullPath}\\NSwag.exe";
  }
  ```

- There are many method that we can override. At least we will need to work on two for the current implementation

  01. Defining the command parameter

      ```csharp
      protected override string GenerateCommandLineCommands()
      {
          return $"openapi2csclient /input:{InputOpenApiSpec}  /classname:{ClientClassName} /namespace:{ClientNamespaceName} /output:{FolderClientClass}\\{ClientClassName}.cs";
      }
      ```

  01. Parameters Validation

      ```csharp
      protected override bool ValidateParameters()
      {
            //http address is not allowed
            var valid = true;
            if (InputOpenApiSpec.StartsWith("http:") || InputOpenApiSpec.StartsWith("https:"))
            {
                valid = false;
                Log.LogError("URL is not allowed");
            }
  
            return valid;
      }
      ```

      _Note:_This simple validation could be done in other way on the MSBuild file, but it is recommended do it in C# code and encapsulate the command and the logic.

- Build the project, everything should compile.
- Create a console app to use our new MSBuild Task

  - Create a Console App, we will call "PetReaderToolTaskConsoleApp". We are going to use .NET 6.0 in our case. Mark it as startup project.
  - Create a Library project to generate the code, called "PetRestApiClient". Use .NET Standard 2.1
  - On "PetReaderToolTaskConsoleApp" create dependency to "PetRestApiClient"
  - On PetRestApiClient project create a folder "PetRestApiClient", this folder will contain the generated code and delete Class1.cs automatically generated.
  - On PetRestApiClient add the following NuGet packages:
    - Newtonsoft.Json, it will be needed to compile the generated client
    - System.ComponentModel.Annotations, it will be needed to compile the generated client
- On PetRestApiClient, create a text file named petshop-openapi-spec.json (on root). We are going to add the OpenApi spec, please copy the content from [here](https://petstore.swagger.io/v2/swagger.json) inside the file. We need a reproducible build that depends only on the input,. Consuming the API directly could transform a build which works today into a build which fails tomorrow from the same input. In this example, we are going to raise a build error if you choose a URL as OpenApi spec input.
- :warning: A general rebuild won't work. You will see errors like 'unable to copy or delete RestApiClientGenerator.dll'. This is because we are trying to build the MBuild Custom Task on the same build process which use it. Select "PetReaderToolTaskConsoleApp" and rebuild only that project. The another solution is put the Custom Task in a completely independent Visual Studio Solutions as we did on [Custom task code generation](../custom-task-code-generation/) example. We are testing different flavors.
- Define the Program.cs
  
  ```csharp
  using System;
  using System.Net.Http;
    namespace PetReaderToolTaskConsoleApp
    {
    internal class Program
      {
          private const string baseUrl = "https://petstore.swagger.io/v2";
          static void Main(string[] args)
          {
              HttpClient httpClient = new HttpClient();
              httpClient.BaseAddress = new Uri(baseUrl);
              var petClient = new PetRestApiClient.PetRestApiClient(httpClient);
              var pet = petClient.GetPetByIdAsync(1).Result;
              Console.WriteLine($"Id: {pet.Id} Name: {pet.Name} Status: {pet.Status} CategoryName: {pet.Category.Name}");
          }
      }
  }
  ```

- We need to change the MSBuild instructions to call our task and generate the code. Edit PetRestApiClient.csproj

  01. Register to the MSBuild custom task

      ```xml
      <UsingTask TaskName="RestApiClientGenerator.RestApiClientGenerator" AssemblyFile="..\RestApiClientGenerator\bin\Debug\netstandard2.0\RestApiClientGenerator.dll" />
      ```

  01. Add some props needed to execute our task

      ```xml
      <PropertyGroup>
          <!--The place where the OpenApi spec is in-->
          <PetClientInputOpenApiSpec>petshop-openapi-spec.json</PetClientInputOpenApiSpec>
          <PetClientClientClassName>PetRestApiClient</PetClientClientClassName>
          <PetClientClientNamespaceName>PetRestApiClient</PetClientClientNamespaceName>
          <PetClientFolderClientClass>PetRestApiClient</PetClientFolderClientClass>
          <!--The directory where NSwag.exe is in-->
          <NSwagCommandFullPath>C:\NSwag\Win</NSwagCommandFullPath>
        </PropertyGroup>
      ```

      :warning: Select the proper NSwagCommandFullPath value based on your computer

  01. Add a [MSBuild targets](https://docs.microsoft.com/visualstudio/msbuild/msbuild-targets) to generate the client during build process. We are going to execute before the core compile execute to generates the code.

      ```xml
      <Target Name="generatePetClient" BeforeTargets="CoreCompile" Inputs="$(PetClientInputOpenApiSpec)" Outputs="$(PetClientFolderClientClass)\$(PetClientClientClassName).cs">
          <!--Calling our custom task derivated from MSBuild Tool Task-->
          <RestApiClientGenerator InputOpenApiSpec="$(PetClientInputOpenApiSpec)" ClientClassName="$(PetClientClientClassName)" ClientNamespaceName="$(PetClientClientNamespaceName)" FolderClientClass="$(PetClientFolderClientClass)" NSwagCommandFullPath="$(NSwagCommandFullPath)"></RestApiClientGenerator>
      </Target>

      <Target Name="forceReGenerationOnRebuild" AfterTargets="CoreClean">
          <Delete Files="$(PetClientFolderClientClass)\$(PetClientClientClassName).cs"></Delete>
      </Target>
      ```

  Input and Output are related to [Incremental Build](https://docs.microsoft.com/visualstudio/msbuild/how-to-build-incrementally), and _forceReGenerationOnRebuild_ target delete the generated file after core clean, and it force the client re generation during the rebuild target execution.

- Select "PetReaderToolTaskConsoleApp" and rebuild only that project. Now, the client code must be generated and the code compiles. It is possible to be executed and see how it works. We are generating from a file, and that is allowed.
- On this step, we are going to show the parameter validation. On _PetRestApiClient.csproj_ change the prop _PetClientInputOpenApiSpec_ to use the url

  ```xml
  <PetClientInputOpenApiSpec>https://petstore.swagger.io/v2/swagger.json</PetClientInputOpenApiSpec>
  ```

Select "PetReaderToolTaskConsoleApp" and rebuild only that project. You will get the error "URL is not allowed"

_Note:_ If you like to publish your custom task as NuGet package, please read the [Custom task code generation](../custom-task-code-generation/) example.

### Downloading code

We need to install [NSwag command line tool](https://github.com/RicoSuter/NSwag/releases). Then,  we need the full path to the directory where NSwag.exe is located. After that, edit PetRestApiClient.csproj selecting the proper NSwagCommandFullPath value based on your computer.
Now, select "RestApiClientGenerator" and build only that project, and finally select and rebuild "PetReaderToolTaskConsoleApp".
You can execute "PetReaderToolTaskConsoleApp".
