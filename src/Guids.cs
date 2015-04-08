// Guids.cs
// MUST match guids.h
using System;

namespace MadsKristensen.ExtensibilityTools
{
    static class GuidList
    {
        public const string guidExtensibilityToolsPkgString = "f8330d54-0469-43a7-8fc0-7f19febeb897";
        public const string guidExtensibilityToolsCmdSetString = "5dbd2975-75e4-4f09-8b6d-1183b0c83762";

        public static readonly Guid guidExtensibilityToolsCmdSet = new Guid(guidExtensibilityToolsCmdSetString);
    };

    static class PkgCmdIDList
    {
        public const uint cmdidMyCommand = 0x100;
    };
}