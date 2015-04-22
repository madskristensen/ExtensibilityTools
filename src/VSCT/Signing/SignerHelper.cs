using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MadsKristensen.ExtensibilityTools.VSCT.Signing
{
    /// <summary>
    /// Helper class performing some signing operations.
    /// This code comes from: https://github.com/phofman/signature
    /// </summary>
    static class SignerHelper
    {
        /// <summary>
        /// Signs the binary.
        /// </summary>
        public static bool Sign(string binaryPath, X509Certificate2 certificate, string certificatePath, string certificatePassword)
        {
            if (string.IsNullOrEmpty(binaryPath))
                throw new ArgumentNullException("binaryPath");
            if (certificate == null && string.IsNullOrEmpty(certificatePath))
                throw new ArgumentException("certificate");

            // is it a VSIX package?
            var extension = Path.GetExtension(binaryPath);
            if (string.Compare(extension, ".vsix", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (certificate == null)
                {
                    try
                    {
                        certificate = new X509Certificate2(certificatePath, certificatePassword);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                return SignVsix(binaryPath, certificate);
            }

            return false;
        }

        private static bool SignVsix(string vsixPackagePath, X509Certificate2 certificate)
        {
            // many thanks to Jeff Wilcox for the idea and code
            // check for details: http://www.jeff.wilcox.name/2010/03/vsixcodesigning/
            using (var package = Package.Open(vsixPackagePath))
            {
                var signatureManager = new PackageDigitalSignatureManager(package);
                signatureManager.CertificateOption = CertificateEmbeddingOption.InSignaturePart;

                var partsToSign = new List<Uri>();
                foreach (var packagePart in package.GetParts())
                {
                    partsToSign.Add(packagePart.Uri);
                }

                partsToSign.Add(PackUriHelper.GetRelationshipPartUri(signatureManager.SignatureOrigin));
                partsToSign.Add(signatureManager.SignatureOrigin);
                partsToSign.Add(PackUriHelper.GetRelationshipPartUri(new Uri("/", UriKind.RelativeOrAbsolute)));

                try
                {
                    signatureManager.Sign(partsToSign, certificate);
                }
                catch (CryptographicException)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
