
## Extensibility Tools for Visual Studio

[![Build status](https://ci.appveyor.com/api/projects/status/hv6uyc059rqbc6fj?svg=true)](https://ci.appveyor.com/project/madskristensen/extensibilitytools)

An extension built for and by Visual Studio extension writers.

Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
or get the [nightly build](http://vsixgallery.com/extension/f8330d54-0469-43a7-8fc0-7f19febeb897/).

### Editor margin
The margin is located below the bottom scrollbar. This comes in handy
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