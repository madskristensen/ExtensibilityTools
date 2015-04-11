using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Vsct
{
    class VsctContentTypeDefinition
    {
        public const string VsctContentType = "VSCT";

        /// <summary>
        /// Exports the Vsct HTML content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(VsctContentType)]
        [BaseDefinition("XML")]
        public ContentTypeDefinition IVsctContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(VsctContentType)]
        [FileExtension(".vsct")]
        public FileExtensionToContentTypeDefinition VsctFileExtension { get; set; }
    }
}
