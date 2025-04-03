using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppSettingStronglyTyped.Test
{
    [TestClass]
    public class AppSettingStronglyTypedTest
    {
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
        public void EmptySettingFileList_EmptyClassGenerated()
        {
            //Arrange
            var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = "MySettingEmpty", SettingNamespaceName = "MyNamespace", SettingFiles = new ITaskItem[0] };
            appSettingStronglyTyped.BuildEngine = buildEngine.Object;

            //Act
            var success = appSettingStronglyTyped.Execute();

            //Assert
            Assert.IsTrue(success);
            Assert.AreEqual(0, errors.Count);
            Assert.AreEqual("MySettingEmpty.generated.cs", appSettingStronglyTyped.ClassNameFile);
            Assert.IsTrue(File.Exists(appSettingStronglyTyped.ClassNameFile));
            Assert.IsTrue(File.ReadLines(appSettingStronglyTyped.ClassNameFile).SequenceEqual(File.ReadLines(".\\Resources\\empty-class.txt")));

            //Cleanup
            File.Delete(appSettingStronglyTyped.ClassNameFile);
        }

        [TestMethod]
        public void SettingFileBadFormat_NotSuccess()
        {
            //Arrange
            var item = new Mock<ITaskItem>();
            item.Setup(x => x.GetMetadata("FullPath")).Returns(".\\Resources\\error-prop.setting");
            var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = "ErrorPropSetting", SettingNamespaceName = "MyNamespace", SettingFiles = new[] { item.Object } };
            appSettingStronglyTyped.BuildEngine = buildEngine.Object;

            //Act
            var success = appSettingStronglyTyped.Execute();

            //Assert
            Assert.IsFalse(success);
            Assert.AreEqual(errors.Count, 1);
            Assert.AreEqual(null, appSettingStronglyTyped.ClassNameFile);

            Assert.AreEqual(1, errors.Count);

            var error = errors.First();

            Assert.AreEqual("Incorrect line format. Valid format prop:type:defaultvalue", error.Message);
            Assert.AreEqual(1, error.LineNumber);
        }

        [TestMethod]
        public void SettingInvalidType_NotSuccess()
        {
            //Arrange
            var item = new Mock<ITaskItem>();
            item.Setup(x => x.GetMetadata("FullPath")).Returns(".\\Resources\\notvalidtype-prop.setting");
            var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = "ErrorPropSetting", SettingNamespaceName = "MyNamespace", SettingFiles = new[] { item.Object } };
            appSettingStronglyTyped.BuildEngine = buildEngine.Object;

            //Act
            var success = appSettingStronglyTyped.Execute();

            //Assert
            Assert.IsFalse(success);
            Assert.AreEqual(errors.Count, 1);
            Assert.AreEqual(null, appSettingStronglyTyped.ClassNameFile);
            Assert.AreEqual("Type not supported -> car", errors.First().Message);
        }

        [TestMethod]
        public void SettingInvalidValue_NotSuccess()
        {
            //Arrange
            var item = new Mock<ITaskItem>();
            item.Setup(x => x.GetMetadata("FullPath")).Returns(".\\Resources\\notvalidvalue-prop.setting");
            var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = "ErrorPropSetting", SettingNamespaceName = "MyNamespace", SettingFiles = new[] { item.Object } };
            appSettingStronglyTyped.BuildEngine = buildEngine.Object;

            //Act
            var success = appSettingStronglyTyped.Execute();

            //Assert
            Assert.IsFalse(success);
            Assert.AreEqual(errors.Count, 1);
            Assert.AreEqual(null, appSettingStronglyTyped.ClassNameFile);
            Assert.AreEqual("It is not possible parse some value based on the type -> bool - awsome", errors.First().Message);
        }

        [DataTestMethod]
        [DataRow("string")]
        [DataRow("int")]
        [DataRow("bool")]
        [DataRow("guid")]
        [DataRow("long")]
        public void SettingFileWithProperty_ClassGeneratedWithOneProperty(string value)
        {
            //Arrange
            var item = new Mock<ITaskItem>();
            item.Setup(x => x.GetMetadata("FullPath")).Returns($".\\Resources\\{value}-prop.setting");
            var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = $"My{value}PropSetting", SettingNamespaceName = "MyNamespace", SettingFiles = new[] { item.Object } };
            appSettingStronglyTyped.BuildEngine = buildEngine.Object;

            //Act
            var success = appSettingStronglyTyped.Execute();

            //Assert
            Assert.IsTrue(success);
            Assert.AreEqual(errors.Count, 0);
            Assert.AreEqual($"My{value}PropSetting.generated.cs", appSettingStronglyTyped.ClassNameFile);
            Assert.IsTrue(File.Exists(appSettingStronglyTyped.ClassNameFile));
            Assert.IsTrue(File.ReadLines(appSettingStronglyTyped.ClassNameFile).SequenceEqual(File.ReadLines($".\\Resources\\{value}-prop-class.txt")));

            //Cleanup
            File.Delete(appSettingStronglyTyped.ClassNameFile);
        }

        [DataTestMethod]
        public void SettingFileWithMultipleProperty_ClassGeneratedWithMultipleProperty()
        {
            //Arrange
            var item = new Mock<ITaskItem>();
            item.Setup(x => x.GetMetadata("FullPath")).Returns($".\\Resources\\complete-prop.setting");
            var appSettingStronglyTyped = new AppSettingStronglyTyped { SettingClassName = $"MyCompletePropSetting", SettingNamespaceName = "MyNamespace", SettingFiles = new[] { item.Object } };
            appSettingStronglyTyped.BuildEngine = buildEngine.Object;

            //Act
            var success = appSettingStronglyTyped.Execute();

            //Assert
            Assert.IsTrue(success);
            Assert.AreEqual(errors.Count, 0);
            Assert.AreEqual($"MyCompletePropSetting.generated.cs", appSettingStronglyTyped.ClassNameFile);
            Assert.IsTrue(File.Exists(appSettingStronglyTyped.ClassNameFile));
            Assert.IsTrue(File.ReadLines(appSettingStronglyTyped.ClassNameFile).SequenceEqual(File.ReadLines(".\\Resources\\complete-prop-class.txt")));

            //Cleanup
            File.Delete(appSettingStronglyTyped.ClassNameFile);
        }

    }
}
