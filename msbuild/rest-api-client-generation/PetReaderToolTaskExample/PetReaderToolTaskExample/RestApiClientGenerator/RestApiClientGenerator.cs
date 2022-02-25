using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RestApiClientGenerator
{
    public class RestApiClientGenerator : ToolTask
    {
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

        protected override string ToolName => "RestApiClientGenerator";

        protected override string GenerateFullPathToTool()
        {
            return $"{NSwagCommandFullPath}\\NSwag.exe";
        }

        protected override string GenerateCommandLineCommands()
        {
            return $"openapi2csclient /input:{InputOpenApiSpec}  /classname:{ClientClassName} /namespace:{ClientNamespaceName} /output:{FolderClientClass}\\{ClientClassName}.cs";
        }

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
    }
}
