# Roadmap

- [ ] Create new item templates for:
  - [ ] TextViewCreationListener
  - [ ] TextViewConnectionListener
  - [ ] Task Runner Explorer extension
- [ ] Show all classification types rendered (#21)
- [ ] .pkgdef QuickInfo showing values of registry keys
- [ ] .pkgdef Intellisense for registry keys

Features that have a checkmark are complete and available for
download in the
[nightly build](http://vsixgallery.com/extension/f8330d54-0469-43a7-8fc0-7f19febeb897/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.9

**2016-06-24**

- [x] Add file icon for .imagemanifest files
- [x] Generation of .imagemanifest files
- [x] Show snapshot version in bottom margin (#37)
- [x] Tooltips in bottom margin shows on category label
- [x] Installs for AllUsers so it works in Experimental Instance

## 1.8

**2016-03-23**

- [x] Support for VS15
- [x] Fixed crashing bug for PTVS
- [x] Support for .vsixmanifest in sub folder
- [x] Fixed crashing bug when no solution was loaded

## 1.7

**2016-03-23**

- [x] Added GUID_TextEditorFactory to completion
- [x] Added Language to vsixmanifest class generator
- [x] Hide button when project is unloaded
- [x] Use indent settings for VsixManifest generator
- [x] Added contribution guidelines to readme.md
- [x] Added GitHub issue template

## 1.6

**2016-02-15**

- [x] Intellisense for `context` in `VisibilityItem`
- [x] Support for `.pkgundef` files
- [x] XML snippet for setting `<StartProgram />` in .csproj
- [x] Added infrastructure for item templates
  - [x] Added snippet file item template
  - [x] Added Browser Link extension template
  - [x] Added Drop handler template
- [x] Button to enable/disable VSIP logging (#22)
- [x] Single File Generator for syncing .resx with vsixmanifest
  - [x] Generate .ico from icon in .vsixmanifest
  - [x] Generate .cs file from .vsixmanifest
- [x] Add buttons to enable support for VsixGallery.com

## 1.5

**2016-01-21**

- [x] **VSCT files**
  - [x] Intellisense for custom GUIDs
  - [x] Intellisense for custom IDs
  - [x] Intellisense for built-in groups and menus
  - [x] ImageMoniker Intellisense
  - [x] Snippets
  - [x] Auto-sync VSCT commands to C# class
- [x] **Editor margin**
  - [x] Shows document encoding
  - [x] Shows content type of the `ITextBuffer` under caret
  - [x] Shows active classifications under caret
  - [x] Shows caret position and selection range
- [x] Dialog for **digitally signing** VSIXs
- [x] **Pkgdef files**
  - [x] Syntax highlighting
  - [x] Intellisense
  - [x] Snippets
  - [x] Brace matching
  - [x] Validation
  - [x] Formatting
- [x] **Show Project information** (for debug purposes)
- [x] **Export KnownMonikers to PNG file**
- [x] **VS Theme color swatch window**
- [x] **View Activity Log**