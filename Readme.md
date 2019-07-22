# IniFileParser

[![NuGet version (SoftCircuits.IniFileParser)](https://img.shields.io/nuget/v/SoftCircuits.IniFileParser.svg?style=flat-square)](https://www.nuget.org/packages/SoftCircuits.IniFileParser/)

```
Install-Package SoftCircuits.IniFileParser
```

The `IniFile` class is a lightweight INI-file parser that can be used to easily read and write INI files. It includes direct support for `string`, `int`, `double` and `bool` setting types.

In addition to reading and writing individual settings, it can return all of the sections in the INI file, and also return all the settings within a particular INI-file section.

The `SetSetting()` method is overloaded to accept different value types. The `GetSetting()` method is also overloaded to allow different `defaultValue` parameter types. The `defaultValue` parameter specifies the value to return if the setting was not found or could not be converted to the specified type. The `defaultValue` parameter type also indicates the return type.

#### Write Example

```cs
// Write values
IniFile file = new IniFile();

file.SetSetting(IniFile.DefaultSectionName, "String", "abc");
file.SetSetting(IniFile.DefaultSectionName, "Integer", 123);
file.SetSetting(IniFile.DefaultSectionName, "Double", 123.45);
file.SetSetting(IniFile.DefaultSectionName, "Boolean1", true);
file.SetSetting(IniFile.DefaultSectionName, "Boolean2", false);
file.Save(path);
```

#### Read Example

```cs
// Read values
IniFile file = new IniFile();

file.Load(path);
string s = file.GetSetting(IniFile.DefaultSectionName, "String", string.Empty));
int i = file.GetSetting(IniFile.DefaultSectionName, "Integer", 0));
double d = file.GetSetting(IniFile.DefaultSectionName, "Double", 0.0));
bool b1 = file.GetSetting(IniFile.DefaultSectionName, "Boolean1", false));
bool b2 = file.GetSetting(IniFile.DefaultSectionName, "Boolean2", true));
```

#### Comments and Empty Lines

Any line with a semicolon (;) as the first non-space character is assumed to be a comment and is ignored by the parser. Empty lines are also ignored.

#### Customizing Boolean Handling

By default, the `bool` version of the `GetSetting()` method understands the words "true", "false", "yes", "no", "on", "off", "1" and "0", and will convert those words to the corresponding `bool` value. The comparison is not case-sensitive. In addition, any string value that can be interpreted as an integer value will considered `True` if that integer value is non-zero, or `False` if that integer value is zero. However, you can override these settings by passing an instance of `BoolOptions` to the `IniFile` constructor.

The following example creates an instance of the `BoolOptions` class, sets the string comparer to use when comparing Boolean words, replaces the default Boolean words with a new list, specifies that strings that can be interpreted as integers should be translated to `bool` values, and passes the object to the `IniFile` constructor.

```cs
BoolOptions options = new BoolOptions(StringComparer.CurrentCultureIgnoreCase);
options.SetBoolWords(new[] {
    new BoolWord("vraie", true),
    new BoolWord("faux", false),
    new BoolWord("oui", true),
    new BoolWord("non", false),
    new BoolWord("sur", true),
    new BoolWord("de", false),
    new BoolWord("1", true),
    new BoolWord("0", false),
});
options.NonZeroNumbersAreTrue = true;

IniFile file = new IniFile(StringComparer.CurrentCultureIgnoreCase, options);
```

**IMPORTANT:** The `SetBoolWords()` method will search the word list to find the first word with a `true` value and the first word with a `false` value, and these words will then be used by the `SetSetting()` method to write `bool` values. In the example above, any `bool` values would be written as "vraie" or "faux". Keep this in mind if you are writing INI files that may be read by other programs, as INI parsers expecting `bool` values to be "true" or "false" would be unable to correctly interpret such a file. If the list of words passed to `SetBoolWords()` does not include any words with a `true` value, or does not include any words with a `false` value, an exception is thrown.
