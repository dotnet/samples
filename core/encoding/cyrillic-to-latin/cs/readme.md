---
languages:
- csharp
products:
- dotnet
page_type: sample
name: ".NET Core Cyrillic to Latin Transliteration Utility (C#)"
urlFragment: "cyrillic-transliteration-cs"
description: "A .NET Core console application written in C# that uses the encoding fallback functionality to transliterate Cyrillic to Latin characters."
---
cyrillic-to-latin is a command-line utility that transliterates modern Cyrillic characters
to their Latin equivalents. It uses a modified Library of Congress system for
transliteration. Its syntax is:

   ```
   CyrillicToLatin <sourceFile> <destinationFile>
   ```

where *sourceFile* is the path and filename of a text file that contains modern Cyrillic
characters, and *destinationFile* is the name of the text file that will store the
original text with its Cyrillic characters replaced by transliterated Latin characters.
If a file path is included in *destinationFile* and any portion of that path does
not exist, the utility terminates.

The specific mappings of upper- and lower-case Cyrillic characters
to Latin characters are listed in the constructor of the `CyrillicToLatinFallback`
class, where the entries of a case mapping table named `table` are defined.

The utility illustrates the extensibility of character encoding in the .NET
Framework. An encoding system consists of an encoder and a decoder. The encoder is
responsible for translating a sequence of characters into a sequence of bytes. The
decoder is responsible for translating the sequence of bytes into a sequence of
characters. .NET Core supports ASCII as well as the standard Unicode
encodings and allows the [Encoding](https://docs.microsoft.com/dotnet/api/system.text.encoding) class to be overridden to support otherwise
unsupported encodings. It also allows an encoder and a decoder's handling of
unmapped characters and bytes to be customized. Broadly, an encoder or a decoder can handle data that it cannot map by throwing an exception or by using some alternate mapping. For more information, see [Character Encoding in .NET Framework](https://docs.microsoft.com/dotnet/standard/base-types/character-encoding).

The transliteration utility works by instantiating an [Encoding](https://docs.microsoft.com/dotnet/api/system.text.encoding) object that represents ASCII encoding, which supports ASCII characters in the range from U+00 to U+FF. Because modern Cyrillic characters occupy the range from U+0410 to U+044F, they do not automatically map to ASCII encoding. When the utility instantiates its Encoding object, it passes its constructor an instance of a class named `CyrillicToLatinFallback` that is derived from [EncoderFallback](https://docs.microsoft.com/dotnet/api/system.text.encoderfallback). This class maintains an internal table that maps modern Cyrillic characters to one or more Latin characters.

When the encoder encounters a character that it cannot encode, it calls the fallback
object's [CreateFallbackBuffer](https://docs.microsoft.com/dotnet/api/system.text.encoderfallback.createfallbackbuffer) method. This method instantiates a `CyrillicToLatinFallbackBuffer` object (a subclass of the [EncoderFallbackBuffer](https://docs.microsoft.com/dotnet/api/system.text.encoderfallbackbuffer) class) and passes its constructor
the modern Cyrillic character mapping table. It then passes the `CyrillicToLatinFallbackBuffer`
object's [Fallback](https://docs.microsoft.com/dotnet/api/system.text.encoderfallbackbuffer.fallback) method each character that it is unable to encode, and if a mapping is available, the method can provide a suitable replacement.
