using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppSettingStronglyTyped
{
    public class AppSettingStronglyTyped : Task
    {
        //The name of the class which is going to be generated
        [Required]
        public string SettingClassName { get; set; }

        //The name of the namespace where the class is going to be generated
        [Required]
        public string SettingNamespaceName { get; set; }

        //List of files which we need to read with the defined format: 'propertyName:type:defaultValue' per line
        [Required]
        public ITaskItem[] SettingFiles { get; set; }

        //The filename where the class was generated
        [Output]
        public string ClassNameFile { get; set; }

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

        private (bool, IDictionary<string, object>) ReadProjectSettingFiles()
        {
            var values = new Dictionary<string, object>();
            foreach (var item in SettingFiles)
            {
                int lineNumber = 0;

                var settingFile = item.GetMetadata("FullPath");
                foreach (string line in File.ReadLines(settingFile))
                {
                    lineNumber++;

                    var lineParse = line.Split(':');
                    if (lineParse.Length != 3)
                    {
                        Log.LogError(subcategory: null,
                                     errorCode: "APPS0001",
                                     helpKeyword: null,
                                     file: settingFile,
                                     lineNumber: lineNumber,
                                     columnNumber: 0,
                                     endLineNumber: 0,
                                     endColumnNumber: 0,
                                     message: "Incorrect line format. Valid format prop:type:defaultvalue");
                        return (false, null);
                    }
                    var value = GetValue(lineParse[1], lineParse[2]);
                    if (!value.Item1)
                    {
                        return (value.Item1, null);
                    }

                    values[lineParse[0]] = value.Item2;
                }
            }
            return (true, values);
        }

        private (bool, object) GetValue(string type, string value)
        {
            try
            {
                // So far only few types are supported values.
                if ("string".Equals(type))
                {
                    return (true, value);
                }
                if ("int".Equals(type))
                {
                    return (true, int.Parse(value));
                }
                if ("long".Equals(type))
                {
                    return (true, long.Parse(value));
                }
                if ("guid".Equals(type))
                {
                    return (true, Guid.Parse(value));
                }
                if ("bool".Equals(type))
                {
                    return (true, bool.Parse(value));
                }
                Log.LogError($"Type not supported -> {type}");
                return (false, null);
            }
            catch
            {
                Log.LogError($"It is not possible parse some value based on the type -> {type} - {value}");
                return (false, null);
            }
        }

        private bool CreateSettingClass(IDictionary<string, object> settings)
        {
            try
            {
                ClassNameFile = $"{SettingClassName}.generated.cs";
                File.Delete(ClassNameFile);
                StringBuilder settingsClass = new StringBuilder(1024);
                // open namespace
                settingsClass.Append($@" using System;
 namespace {SettingNamespaceName} {{

  public class {SettingClassName} {{
");
                //For each element in the dictionary create a static property
                foreach (var keyValuePair in settings)
                {
                    string typeName = GetTypeString(keyValuePair.Value.GetType().Name);
                    settingsClass.Append($"    public readonly static {typeName}  {keyValuePair.Key} = {GetValueString(keyValuePair, typeName)};\r\n");
                }
                // close namespace and class
                settingsClass.Append(@"  }

}");
                File.WriteAllText(ClassNameFile, settingsClass.ToString());

            }
            catch (Exception ex)
            {
                //This logging helper method is designed to capture and display information from arbitrary exceptions in a standard way.
                Log.LogErrorFromException(ex, showStackTrace: true);
                return false;
            }
            return true;
        }

        private string GetTypeString(string typeName)
        {
            if ("String".Equals(typeName))
            {
                return "string";
            }
            if ("Boolean".Equals(typeName))
            {
                return "bool";
            }
            if ("Int32".Equals(typeName))
            {
                return "int";
            }
            if ("Int64".Equals(typeName))
            {
                return "long";
            }
            return typeName;
        }

        private static object GetValueString(KeyValuePair<string, object> keyValuePair, string typeName)
        {
            if ("Guid".Equals(typeName))
            {
                return $"Guid.Parse(\"{keyValuePair.Value}\")";
            }
            if ("string".Equals(typeName))
            {
                return $"\"{keyValuePair.Value}\"";
            }
            if ("bool".Equals(typeName))
            {
                return $"{keyValuePair.Value.ToString().ToLower()}";
            }

            return keyValuePair.Value;
        }
    }
}
