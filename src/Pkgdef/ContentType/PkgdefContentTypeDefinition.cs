using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.ExtensibilityTools.Pkgdef
{
    class PkgdefContentTypeDefinition
    {
        public const string PkgdefContentType = "Pkgdef";

        /// <summary>
        /// Exports the Pkgdef HTML content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(PkgdefContentType)]
        [BaseDefinition("plaintext")]
        public ContentTypeDefinition IPkgdefContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(PkgdefContentType)]
        [FileExtension(".pkgdef")]
        public FileExtensionToContentTypeDefinition PkgdefFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(PkgdefContentType)]
        [FileExtension(".pkgundef")]
        public FileExtensionToContentTypeDefinition PkgundefFileExtension { get; set; }
    }
}
