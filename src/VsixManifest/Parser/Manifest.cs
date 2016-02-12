namespace MadsKristensen.ExtensibilityTools.VsixManifest
{
    class Manifest
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Icon { get; set; }
        public string Preview { get; set; }
        public string Tags { get; set; }
        public string License { get; set; }
        public string GettingStartedUrl { get; set; }
        public string ReleaseNotesUrl { get; set; }
        public string MoreInfoUrl { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
