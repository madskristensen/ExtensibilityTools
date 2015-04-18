
## Extensibility Tools for Visual Studio

[![Build status](https://ci.appveyor.com/api/projects/status/hv6uyc059rqbc6fj?svg=true)](https://ci.appveyor.com/project/madskristensen/extensibilitytools)

An extension built for and by Visual Studio extension writers.

Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
or get the [nightly build](http://vsixgallery.com/extension/f8330d54-0469-43a7-8fc0-7f19febeb897/).

### VSCT files
The Visual Studio Command Table leaves a lot to be desired in terms of
both Intellisense and discoverability of even the most common use cases.

This extension improves on that. 

#### Intellisense for custom GUIDs
Get Intellisense for symbols and groups through out the .vsct file
whether you're defining `Groups`, `Menus`, `Buttons`, `KeyBindings` or 
`CommandPlacements`.

![VSCT Intellisense GUIDs](art/vsct-intellisense-guid.png)

#### Intellisense for custom IDs
Provides Intellisense for IDs based on the `guid` attribute on
the same XML element.

![VSCT Intellisense IDs](art/vsct-intellisense-id.png)

#### Intellisense built-in groups and menus
All the built-in groups and menus are located under the `guidSHLMainMenu`
GUID and Intellisense is now provided for all the corresponding IDs.

![VSCT Intellisense groups menus](art/vsct-intellisense-builtin.png)

#### Snippets
All main XML elements in the VSCT file has snippets associated with
them and can be invoked by hitting the Tab key.

![VSCT snippets](art/vsct-snippets.gif)

#### Auto-sync VSCT commands
Auto-generate the `GuidList` and `PackageCommand` classes every time
you save the VSCT file. This keeps your code in sync with the VSCT file
at all times.

![VSCT auto-sync](art/vsct-autosync.png)
*Feature contributed by @phofman*

### Editor margin
The margin is located below the bottom scrollbar and comes in handy
when writing extensions that extends the VS editor.

![Bottom margin](art/margin.png)

#### Document encoding
Shows the encoding of the current document and more details on hover.

![Document encoding](art/margin-encoding.png)

#### Content type
Shows the content type of the textbuffer at the caret position. The
over tooltip shows the name of the base content type.

#### Classification
Displays the name of the classification at the caret position in the
document. The hover tooltip shows the inheritance hierarchy of the
`EditorFormatDefinition`'s `BaseDefinition` attribute.

![Classifications](art/margin-classification.png)

### Pkgdef files

#### Syntax highlighting
Colorizes registry keys, strings, keywords, comments and more.

![Pkgdef colorization](art/pkgdef-colorization.png)

#### Intellisense
Intellisense is provided for tokens and GUIDs.

![Pkgdef token Intellisense](art/pkgdef-intellisense-tokens.png)

![Pkgdef token Intellisense](art/pkgdef-intellisense-guids.png)

#### Snippets
By typing a question mark on an empty line, a list of snippets appear.
Hit `Tab` on the snippet you want and it will be inserted.

![Pkgdef snippets](art/pkgdef-snippets.gif)

#### Brace matching
Matches parantheses and square brackets.

#### Validation
Validates various common mistakes like unknown tokens and unclosed strings and braces.

![Pkgdef validation](art/pkgdef-validation.png)

#### Formatting
Format the entire document or just the selected lines.