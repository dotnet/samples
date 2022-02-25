using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AppSettingStronglyTyped.Test
{
    /**
     * MSBuild starts worker nodes when building multiple projects. These are independent
     * processes that can be long-lived (by default, they have a 15 minute idle timeout).
     * Reusing these processes allows subsequent builds to benefit from reduced startup
     * time and increased caching in internal mechanisms, BUT it also means that there
     * will be a process holding a lock on any used task assemblies for a long time.
     * The command-line argument -nodeReuse:false tells MSBuild not to do that: any
     * additional processes it creates will exit when the build does.
     *
     * If you've ever noticed builds failing because of a failure to copy to the output
     * because a file was in use after running tests, it's because of this.
     **/
    [TestClass]
    public class AppSettingStronglyTypedIntegrationTest
    {
        public const string MSBUILD = "C:\\Program Files\\dotnet\\dotnet.exe";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Process buildProcess;
        private List<string> output;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

        [TestCleanup()]
        public void Cleanup()
        {
            buildProcess.Close();
        }

        [TestMethod]
        public void ValidSettingTextFile_SettingClassGenerated()
        {
            //Arrage
            buildProcess.StartInfo.Arguments = "build -nodeReuse:false .\\Resources\\testscript-success.msbuild /t:generateSettingClass";

            //Act
            ExecuteCommandAndCollectResults();

            //Assert
            Assert.AreEqual(0, buildProcess.ExitCode);
            Assert.IsTrue(File.Exists(".\\Resources\\MySettingSuccess.generated.cs"));
            Assert.IsTrue(File.ReadLines(".\\Resources\\MySettingSuccess.generated.cs").SequenceEqual(File.ReadLines(".\\Resources\\testscript-success-class.txt")));

            //Cleanup
            File.Delete(".\\Resources\\MySettingSuccess.generated.cs");
        }

        [TestMethod]
        public void NotValidSettingTextFile_SettingClassNotGenerated()
        {
            //Arrage
            buildProcess.StartInfo.Arguments = "build -nodeReuse:false .\\Resources\\testscript-fail.msbuild /t:generateSettingClass";

            //Act
            ExecuteCommandAndCollectResults();

            //Assert
            Assert.AreEqual(1, buildProcess.ExitCode);
            Assert.IsFalse(File.Exists(".\\Resources\\MySettingFail.generated.cs"));
            Assert.IsTrue(output.Any(line => line.Contains("Incorrect line format. Valid format prop:type:defaultvalue")));
        }

        private void ExecuteCommandAndCollectResults()
        {
            buildProcess.Start();
            while (!buildProcess.StandardOutput.EndOfStream)
            {
                output.Add(buildProcess.StandardOutput.ReadLine() ?? string.Empty);
            }
            buildProcess.WaitForExit();
        }
    }
}
