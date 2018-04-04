# .NET Samples

This repo contains all the sample code that is part of any topic under
the .NET documentation. There are several different projects that
are organized in sub-folders. These sub-folders are organized similar
to the organization of the docs for .NET.

Some of the articles will have more than one sample associated with them. 

The readme.md file for each sample will refer to the article so that
you can read more about the concepts covered in each sample.

There are two classes of code in this repository:

- **Samples** represent programs that demonstrate apoplication or library scenarios. These samples are typically larger, and often use more than one technology, feature, or toolkit.
- **Snippets** represent small focused examples that demonstrate one feature or syntax. These should be no more than a single screen of code.

We are working toward having a CI system in place for all samples and snippets. 
When you make any updates to samples, make sure each update is part of a buildable
project. Ideally, add tests for correctness on samples as well.

## Snippets

Snippets are extracted from small programs that include the snippet. Snippets are all located in the top level **/snippets** folder. Snippets should follow all other sample guidance.

## Building a sample

You build any .NET core sample using the .NET Core CLI. You can download the CLI from
[the .NET Core SDK download page](https://www.microsoft.com/net/download). Then, execute
these commands from the CLI in the directory of any sample:

```
dotnet build
dotnet run
```

These will install any needed dependencies, build the project, and run
the project respectively.

Multi-project samples have instructions in their root directory in
a `README.md` file.  

Except where noted, all samples build from the command line on
any platform supported by .NET Core. There are a few samples that are
specific to Visual Studio and require Visual Studio 2017 or later. In 
addition, some samples show platform specific features and will require 
a specific platform. Other samples and snippets require the .NET Framework
and will run on Windows platforms, and will need the Developer Pack for
the target Framework version.

## Creating new samples

If you wish to add a code sample:

1. Your sample **must be part of a buildable project**. Where possible, the projects should build on all platforms suppported by .NET Core. Exceptions to this are samples that demonstrate a platform specific feature or platform specific tool.
3. Your sample should conform to the [corefx coding style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) to maintain consistency.
	- Additionally, we prefer the use of `static` methods rather than instance methods when demonstrating something that doesn't require instantiating a new object.
4. If your sample builds a standalone package, you must include the runtimes used by our CI build system, in addition to any runtimes used by your sample:
    - `win7-x64`
    - `win8-x64`
    - `win81-x64`
    - `ubuntu.16.04-x64`

We will have a CI system in place to build these projects shortly.

To create a sample:

1. File an [issue](https://github.com/dotnet/docs/issues) or add a comment to an existing one that you are working on it.
2. Write the topic that explains the concepts demonstrated in your sample (example: `docs/standard/linq/where-clause.md`) 
3. Write your sample (example: `WhereClause-Sample1.cs`)
4. Create a Program.cs with a Main entry point that calls your samples. If there is already one there, add the call to your sample:
  ```csharp
    public class Program
    {
        public void Main(string[] args)
        {
            WhereClause1.QuerySyntaxExample();

            // Add the method syntax as an example.
            WhereClause1.MethodSyntaxExample();
        }
    }
  ```
  To build and run your sample...


5. Go to the sample folder and Build to check for errors.

 ```
    dotnet build
 ```
7. Run!

 ```
    dotnet run
 ```

8. Add a readme.md to the root directory of your sample.
    - This should include a brief description of the code, and refer people to the article that references the sample.

