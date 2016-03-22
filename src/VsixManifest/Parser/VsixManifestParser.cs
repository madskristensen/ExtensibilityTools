using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MadsKristensen.ExtensibilityTools.VsixManifest
{
    static class VsixManifestParser
    {
        public async static Task<Manifest> FromFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return null;

            using (var reader = new StreamReader(fileName))
            {
                var content = await reader.ReadToEndAsync();
                return FromManifest(content);
            }
        }

        public static Manifest FromManifest(string vsixManifestContent)
        {
            var xml = Regex.Replace(vsixManifestContent, "( xmlns(:\\w+)?)=\"([^\"]+)\"", string.Empty)
                           .Replace(" d:", " ");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var package = new Manifest();

            if (doc.GetElementsByTagName("DisplayName").Count > 0)
            {
                Vs2012Format(doc, package);
            }
            else
            {
                Vs2010Format(doc, package);
            }

            return package;
        }

        static void Vs2012Format(XmlDocument doc, Manifest manifest)
        {
            manifest.Author = ParseNode(doc, "Identity", true, "Publisher");
            manifest.Description = ParseNode(doc, "Description", true);
            manifest.GettingStartedUrl = ParseNode(doc, "GettingStartedGuide", false);
            manifest.Icon = ParseNode(doc, "Icon", false);
            manifest.ID = ParseNode(doc, "Identity", true, "Id");
            manifest.Language = ParseNode(doc, "Identity", true, "Language");
            manifest.License = ParseNode(doc, "License", false);
            manifest.MoreInfoUrl = ParseNode(doc, "MoreInfo", false);
            manifest.Name = ParseNode(doc, "DisplayName", true);
            manifest.Preview = ParseNode(doc, "PreviewImage", false);
            manifest.ReleaseNotesUrl = ParseNode(doc, "ReleaseNotes", false);
            manifest.Tags = ParseNode(doc, "Tags", false);
            manifest.Version = new Version(ParseNode(doc, "Identity", true, "Version")).ToString();
        }

        static void Vs2010Format(XmlDocument doc, Manifest manifest)
        {
            manifest.Author = ParseNode(doc, "Author", true);
            manifest.Description = ParseNode(doc, "Description", true);
            manifest.GettingStartedUrl = ParseNode(doc, "GettingStartedGuide", false);
            manifest.Icon = ParseNode(doc, "Icon", false);
            manifest.ID = ParseNode(doc, "Identifier", true, "Id");
            manifest.Language = "en-US";
            manifest.License = ParseNode(doc, "License", false);
            manifest.MoreInfoUrl = ParseNode(doc, "MoreInfo", false);
            manifest.Name = ParseNode(doc, "Name", true);
            manifest.Preview = ParseNode(doc, "PreviewImage", false);
            manifest.ReleaseNotesUrl = ParseNode(doc, "ReleaseNotes", false);
            manifest.Version = new Version(ParseNode(doc, "Version", true)).ToString();
        }

        static string ParseNode(XmlDocument doc, string name, bool required, string attribute = "")
        {
            XmlNodeList list = doc.GetElementsByTagName(name);

            if (list.Count > 0)
            {
                XmlNode node = list[0];

                if (string.IsNullOrEmpty(attribute))
                    return node.InnerText;

                XmlAttribute attr = node.Attributes[attribute];

                if (attr != null)
                    return attr.Value;
            }

            if (required)
            {
                string message = string.Format("Attribute '{0}' could not be found on the '{1}' element in the .vsixmanifest file.", attribute, name);
                throw new Exception(message);
            }

            return null;
        }
    }
}
