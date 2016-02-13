# Roadmap

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
- [x] Add button to enable support for VsixGallery.com
- [ ] New item templates for:
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