using System;
using System.CodeDom.Compiler;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace MadsKristensen.ExtensibilityTools.VSCT.Generator
{
    /// <summary>
    /// Base class for any kind of code generator.
    /// </summary>
    public abstract class BaseCodeGenerator : BaseCodeGeneratorWithSite
    {
        private CodeDomProvider _codeDomProvider;

        /// <summary>
        /// Gets the default extension for this generator
        /// </summary>
        /// <returns>
        /// String with the default extension for this generator
        /// </returns>
        public override string GetDefaultExtension()
        {
            var codeDom = GetCodeProvider();

            if (codeDom == null)
                throw new ApplicationException("Unable to determine destination code model");

            return "." + codeDom.FileExtension;
        }

        /// <summary>
        /// The method that does the actual work of generating the output source code.
        /// </summary>
        /// <param name="inputFileContent">File contents as a string</param>
        /// <returns>The generated code file as a byte-array</returns>
        protected abstract string GenerateStringCode(string inputFileContent);

        /// <summary>
        /// The method that does the actual work of generating code given the input file.
        /// </summary>
        /// <param name="inputFileName">Input file name</param>
        /// <param name="inputFileContent">File contents as a string</param>
        /// <returns>
        /// The generated code file as a byte-array
        /// </returns>
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            var result = GenerateStringCode(inputFileContent);

            if (string.IsNullOrEmpty(result))
                return null;

            // return as bytes:
            return Encoding.UTF8.GetBytes(result);
        }

        /// <summary>
        /// Returns the EnvDTE.ProjectItem object that corresponds to the project item the code generator was called on.
        /// </summary>
        /// <returns>The EnvDTE.ProjectItem of the project item the code generator was called on</returns>
        protected ProjectItem GetProjectItem()
        {
            return GetService(typeof(ProjectItem)) as ProjectItem;
        }

        /// <summary>
        /// Returns the EnvDTE.Project object of the project containing the project item the code generator was called on
        /// </summary>
        /// <returns>
        /// The EnvDTE.Project object of the project containing the project item the code generator was called on
        /// </returns>
        protected Project GetProject()
        {
            return GetProjectItem().ContainingProject;
        }

        /// <summary>
        /// Returns a CodeDomProvider object for the language of the project containing
        /// the project item the generator was called on
        /// </summary>
        /// <returns>A CodeDomProvider object</returns>
        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (_codeDomProvider == null)
                _codeDomProvider = GetCodeProvider(GetProject().CodeModel.Language);

            return _codeDomProvider;
        }

        /// <summary>
        /// Returns a <see cref="CodeDomProvider"/> object for the language of the project item Language property.
        /// </summary>
        public static CodeDomProvider GetCodeProvider(string languageGuid)
        {
            switch (languageGuid)
            {
                case CodeModelLanguageConstants.vsCMLanguageVB:
                    return CodeDomProvider.CreateProvider("VB");
                case CodeModelLanguageConstants2.vsCMLanguageJSharp:
                    return CodeDomProvider.CreateProvider("VJ#");
                case CodeModelLanguageConstants.vsCMLanguageVC:
                case CodeModelLanguageConstants.vsCMLanguageMC:
                    return CodeDomProvider.CreateProvider("MC");
                default:
                    return CodeDomProvider.CreateProvider("C#");
            }
        }
    }
}
