using System;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSWinFormDataGridView
{
    public static class ResourceDeserializationExtensions
    {
        static BinaryFormatter Formatter = new BinaryFormatter();

        /// <summary>
        /// Returns an image deserialized from a string resource to work around https://github.com/dotnet/corefx/issues/26745
        /// </summary>
        public static Image GetImage(this ResourceManager resourceManager, string name)
        {
            var imageString = resourceManager.GetString(name);
            using (var ms = new MemoryStream(Convert.FromBase64String(imageString)))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
