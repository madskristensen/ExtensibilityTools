namespace MadsKristensen.ExtensibilityTools
{
    using System;
    
    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidExtensibilityToolsPkgString = "f8330d54-0469-43a7-8fc0-7f19febeb897";
        public const string guidExtensibilityToolsCmdSetString = "5dbd2975-75e4-4f09-8b6d-1183b0c83762";
        public const string guidSolutionCmdSetString = "f4f37d70-d3e8-40be-a76e-7abef91a332c";
        public static Guid guidExtensibilityToolsPkg = new Guid(guidExtensibilityToolsPkgString);
        public static Guid guidExtensibilityToolsCmdSet = new Guid(guidExtensibilityToolsCmdSetString);
        public static Guid guidSolutionCmdSet = new Guid(guidSolutionCmdSetString);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int ProjectMenuGroup = 0x03E8;
        public const int cmdAddCustomTool = 0x0001;
        public const int cmdSignBinary = 0x0002;
        public const int cmdShowInformation = 0x0003;
        public const int cmdExportMoniker = 0x0004;
        public const int cmdSwatchesWindow = 0x0005;
        public const int cmdActivityLog = 0x0006;
        public const int cmdVsipLogging = 0x0007;
        public const int cmdResxGenerator = 0x0008;
        public const int Menu = 0x1000;
        public const int MenuGroup = 0x0100;
        public const int cmdGitHubPrepare = 0x0001;
        public const int cmdAppVeyorPrepare = 0x0002;
    }
}
