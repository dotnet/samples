using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RestApiClientGenerator.Test
{
    [TestClass]
    public class RestApiClientGeneratorTest
    {
        private const string NSWAG_FOLDER = "C:\\Nwag\\Win";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Mock<IBuildEngine> buildEngine;
        private List<BuildErrorEventArgs> errors;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize()]
        public void Startup()
        {
            buildEngine = new Mock<IBuildEngine>();
            errors = new List<BuildErrorEventArgs>();
            buildEngine.Setup(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>())).Callback<BuildErrorEventArgs>(e => errors.Add(e));
        }

        [TestMethod]
        public void SpecAsFile_GeneratesTheClient()
        {
            //Arrange
            var restApiClientGenerator = new RestApiClientGenerator
            {
                InputOpenApiSpec = ".\\Resources\\petshop-openapi-spec.json",
                ClientClassName = "MyClient",
                ClientNamespaceName = "MyNamespace",
                FolderClientClass = ".",
                NSwagCommandFullPath = NSWAG_FOLDER
            };
            restApiClientGenerator.BuildEngine = buildEngine.Object;

            //Act
            var result = restApiClientGenerator.Execute();

            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(errors.Count, 0);
            Assert.IsTrue(File.Exists($"{restApiClientGenerator.FolderClientClass}\\{restApiClientGenerator.ClientClassName}.cs"));

            //Cleanup
            File.Delete($"{restApiClientGenerator.FolderClientClass}\\{restApiClientGenerator.ClientClassName}.cs");
        }

        [TestMethod]
        public void SpecAsFile_BadFormat_ClientNotGenerated()
        {
            //Arrange
            var restApiClientGenerator = new RestApiClientGenerator
            {
                InputOpenApiSpec = ".\\Resources\\bad-spec.json",
                ClientClassName = "BadSpec",
                ClientNamespaceName = "MyNamespace",
                FolderClientClass = ".",
                NSwagCommandFullPath = NSWAG_FOLDER
            };
            restApiClientGenerator.BuildEngine = buildEngine.Object;

            //Act
            var result = restApiClientGenerator.Execute();

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(errors.Count, 1);
            Assert.IsFalse(File.Exists($"{restApiClientGenerator.FolderClientClass}\\{restApiClientGenerator.ClientClassName}.cs"));
            Assert.AreEqual("\"RestApiClientGenerator\" exited with code -1.", errors.First().Message);
        }

        [TestMethod]
        public void SpecAsURL_ClientNotGenerated()
        {
            //Arrange
            var restApiClientGenerator = new RestApiClientGenerator
            {
                InputOpenApiSpec = "https://petstore.swagger.io/v2/swagger.json",
                ClientClassName = "ClientNotGenerated",
                ClientNamespaceName = "MyNamespace",
                FolderClientClass = ".",
                NSwagCommandFullPath = NSWAG_FOLDER
            };
            restApiClientGenerator.BuildEngine = buildEngine.Object;

            //Act
            var result = restApiClientGenerator.Execute();

            //Assert
            Assert.IsFalse(result);
            Assert.IsFalse(File.Exists($"{restApiClientGenerator.FolderClientClass}\\{restApiClientGenerator.ClientClassName}.cs"));
            Assert.AreEqual(errors.Count, 1);
            Assert.AreEqual("URL is not allowed", errors.First().Message);
        }
    }
}