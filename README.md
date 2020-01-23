# MarkdownView _for Xamarin.Forms_

[![NuGet](https://img.shields.io/nuget/v/Xam.Forms.MarkdownView.svg?label=NuGet)](https://www.nuget.org/packages/Xam.Forms.MarkdownView/) [![Donate](https://img.shields.io/badge/donate-paypal-yellow.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ZJZKXPPGBKKAY&lc=US&item_name=GitHub&item_number=0000001&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted)

A native Xamarin.Forms Markdown renderer.

## Gallery

![Light theme](Documentation/Screenshot.png)

## Introduction

Compared to a majority of solutions, MarkdownView will render every component as **a native Xamarin.Forms view instead of via an HTML backend.** The Markdown is directly translated from a syntax tree to a hierarchy of Xamarin.Forms views, : no HTML is being produced at all (hurray)!

This will produce a more reactive user interface, at the cost of rendering functionalities _(at the moment though!)_.

## Install

Available on [NuGet](https://www.nuget.org/packages/Xam.Forms.MarkdownView/).

## Quickstart

```csharp
var view = new MarkdownView();
view.Markdown = "# Hello world\n\nThis is my first native markdown rendering";
view.Theme = new DarkMarkdownTheme(); // Default is white, you also modify various values
this.Content = view;
```

## Limitations

Unfortunately, Xamarin.Forms string rendering has some limitations ...

-   **Inlined images aren't supported** (_Xamarin.Forms formatted strings doesn't support inlined views_) : They will be displayed after the block they are referenced from.
-   **Links are only clickable at a leaf block level** (_Xamarin.Forms formatted strings doesn't support span user interactions_) : if a leaf block contains more than one link, the user is prompted. This is almost a feature since text may be too small to be enough precise! ;)
-   **SVG rendering is very limited** (_The SVG rendering is based on SkiaSharp which doesn't seem to manage well all svg renderings_)

## Roadmap

-   **Customization**
    _ [X] Styles
    _ [X] Themes
-   **Leaf blocks**
    _ [X] Headings
    _ [X] Paragraphs
    _ [ ] HTML Blocks (maybe partial and specific support)
    _ [ ] Link reference definitions
    _ [X] Code blocks
    _ [X] Thematic breaks
-   **Container blocks**
    _ [X] Block quote
    _ [X] Lists
    _ [ ] Numbers bullet formats
    _ [ ] Custom bullets
-   **Inlines**
    _ [X] Textual content
    _ [X] Emphasis and string emphasis
    _ [X] Code spans
    _ [X] Links (partial, no interaction)
    _ [X] Image blocks (partial, not inlined)
    _ [X] SVG Rendering (Skia)
-   **Extensions**
    _ [ ] Table blocks
    _ [ ] Emojis (ascii) \* [ ] Task lists

## Thanks

-   [lunet-io/markdig](https://github.com/lunet-io/markdig) : used for Markdown parsing
-   [mono/SkiaSharp](https://github.com/mono/SkiaSharp) : used for SVG rendering

## Contributions

Contributions are welcome! If you find a bug please report it and if you want a feature please report it.

If you want to contribute code please file an issue and create a branch off of the current dev branch and file a pull request.

## License

MIT © [Aloïs Deniel](http://aloisdeniel.github.io), [Bartosz Jędrecki](https://github.com/Ickerday)
