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
