# IniFileParser

[![NuGet version (SoftCircuits.IniFileParser)](https://img.shields.io/nuget/v/SoftCircuits.IniFileParser.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.IniFileParser/)

```
Install-Package SoftCircuits.IniFileParser
```

`IniFile` is a lightweight .NET class library that makes it easy to read and write INI files. It provide direct support for `string`, `int`, `double` and `bool` setting. It can return all of the sections in an INI file, and also return all the settings within a particular INI-file section.

#### Writing Settings

To write an INI file, create an instance of the `IniFile` class and call the `SetSetting()` method to set each setting. This method is overloaded to accept `string`, `int`, `double` and `bool` value types. Then call the `Save()` method to write the settings to a file.

```cs
// Write values
IniFile file = new IniFile();

file.SetSetting(IniFile.DefaultSectionName, "Name", "Bob Smith");
file.SetSetting(IniFile.DefaultSectionName, "Age", 34);
file.SetSetting(IniFile.DefaultSectionName, "Rating", 123.45);
file.SetSetting(IniFile.DefaultSectionName, "Active", true);

file.Save(path);
```

#### Reading Settings

To read an INI file, create an instance of the `IniFile` class and call the `Load()` method to read the settings from a file. Then call the `GetSetting()` method to get each setting.

The `GetSetting()` method accepts a default value parameter. The method returns the value of this parameter if the setting was not found or could not be converted to the specified type. The `GetSetting()` method is overloaded to accept default values of type `string`, `int`, `double` and `bool`. The default value parameter determines the return type.

```cs
// Read values
IniFile file = new IniFile();

file.Load(path);

string name = file.GetSetting(IniFile.DefaultSectionName, "Name", string.Empty));
int age = file.GetSetting(IniFile.DefaultSectionName, "Age", 0));
double rating = file.GetSetting(IniFile.DefaultSectionName, "Rating", 0.0));
bool active = file.GetSetting(IniFile.DefaultSectionName, "Active", false));
```

If any settings are found that are not under a section header, they will be added to the `IniFile.DefaultSectionName` section.

#### Reading All Sections in the File

Use the `GetSections()` method to retrieve all the sections in the file.

```cs
IEnumerable<string> sections = file.GetSections();
```

#### Reading All Settings in a Section

Use the `GetSectionSettings()` method to retrieve all the settings in a section.

```cs
IEnumerable<IniSetting> settings = file.GetSectionSettings(IniFile.DefaultSectionName);
```

#### Comments and Empty Lines

By default, any line with a semicolon (;) as the first non-space character is assumed to be a comment. The comment character can be changed by setting the `CommentCharacter` property.

In addition, any comments found when reading an INI file are stored in the `Comments` collection. And any comments in this collection will be written when saving an INI file. This makes it easy to add comments to INI files you create, or to maintain comments in INI files you modify. (Note, however, that all comments are written to the start of the INI file regardless of where those comments might have been when read.)

Empty lines are also ignored.

#### Custom Boolean Handling

By default, the `bool` version of the `GetSetting()` method understands the words "true", "false", "yes", "no", "on", "off", "1" and "0", and will convert those words to the corresponding `bool` value. The comparison is not case-sensitive. In addition, any string value that can be interpreted as an integer value will considered `true` if that integer value is non-zero, or `false` if that integer value is zero.

However, you can override these settings by passing an instance of the `BoolOptions` class to the `IniFile` constructor. The following example creates an instance of the `BoolOptions` class, sets the string comparer to use when comparing Boolean words, replaces the default Boolean words with a new list, specifies that strings that can be interpreted as integers should be translated to `bool` values, and passes the object to the `IniFile` constructor.

```cs
BoolOptions options = new BoolOptions(StringComparer.CurrentCultureIgnoreCase);
options.SetBoolWords(new[] {
    new BoolWord("sure", true),
    new BoolWord("okay", true),
    new BoolWord("yeppers", true),
    new BoolWord("nope", false),
    new BoolWord("nah", false),
    new BoolWord("nopers", false),
});
options.NonZeroNumbersAreTrue = true;

IniFile file = new IniFile(StringComparer.CurrentCultureIgnoreCase, options);
```

**NOTE:** The `SetBoolWords()` method will search the word list to find the first word with a `true` value and the first word with a `false` value, and these words will then be used by the `SetSetting()` method to write `bool` values. In the example above, any `bool` values would be written as "sure" or "nope". Keep this in mind if you are writing INI files that may be read by other programs, as INI parsers expecting `bool` values to be "true" or "false" would be unable to correctly interpret such a file. If the list of words passed to `SetBoolWords()` does not include any words with a `true` value, or does not include any words with a `false` value, an exception is thrown.
