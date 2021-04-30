// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Class to read and write INI files.
    /// </summary>
    public class IniFile
    {
        private readonly Dictionary<string, IniSection> Sections;
        private readonly StringComparer StringComparer;
        private readonly BoolOptions BoolOptions;

        /// <summary>
        /// Default section name. Used for any settings found outside any section header.
        /// </summary>
        public const string DefaultSectionName = "General";

        /// <summary>
        /// Gets or sets the character used to signify a comment. Must be the first
        /// non-space character on the line. Default value is a semicolon (<c>;</c>).
        /// </summary>
        public char CommentCharacter { get; set; }


        /// <summary>
        /// Contains the list of comment lines read from a file, or that will be written
        /// to a file.
        /// </summary>
        public List<string?> Comments { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="IniFile"></see> instance.
        /// </summary>
        /// <param name="comparer"><see cref="StringComparer"></see> used to compare section and setting
        /// names. If not specified, <see cref="StringComparer.CurrentCultureIgnoreCase"></see> is used
        /// (i.e. names are not case-sensitive).</param>
        /// <param name="boolOptions">Options for interpreting <c>bool</c> values.</param>
        public IniFile(StringComparer? comparer = null, BoolOptions? boolOptions = null)
        {
            Sections = new Dictionary<string, IniSection>(StringComparer);
            StringComparer = comparer ?? StringComparer.CurrentCultureIgnoreCase;
            BoolOptions = boolOptions ?? new BoolOptions();
            Comments = new();
            CommentCharacter = ';';
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load.</param>
        public void Load(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using StreamReader reader = new(path);
            Load(reader);
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using StreamReader reader = new(path, detectEncodingFromByteOrderMarks);
            Load(reader);
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load settings from.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public void Load(string path, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using StreamReader reader = new(path, encoding);
            Load(reader);
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load settings from.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using StreamReader reader = new(path, encoding, detectEncodingFromByteOrderMarks);
            Load(reader);
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"></see> to load settings from.</param>
        public void Load(StreamReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // Tracks the current section
            IniSection? section = null;

            // Clear any existing data
            Clear();

            string? line = reader.ReadLine();
            while (line != null)
            {
                // Trim leading whitespace
                int start = 0;
                while (start < line.Length && char.IsWhiteSpace(line[start]))
                    start++;

                // Process line
                if (start < line.Length)
                {
                    if (line[start] == CommentCharacter)
                    {
                        // Store comments
#if NETSTANDARD2_0
                        Comments.Add(line.Substring(1));
#else
                        Comments.Add(line[1..]);
#endif
                    }
                    else if (line[start] == '[')
                    {
                        // Parse section header
                        start++;
                        int pos = line.IndexOf(']', start);
                        if (pos == -1)
                            pos = line.Length;
#if NETSTANDARD2_0
                        string name = line.Substring(start, pos - start).Trim();
#else
                        string name = line[start..pos].Trim();
#endif
                        if (name.Length > 0)
                        {
                            // Add section if it doesn't already exist
                            if (!Sections.TryGetValue(name, out section))
                            {
                                section = new IniSection(name, StringComparer);
                                Sections.Add(section.Name, section);
                            }
                        }
                    }
                    else
                    {
                        // Parse setting name and value
                        string name, value;

                        int pos = line.IndexOf('=', start);
                        if (pos == -1)
                        {
                            name = line.Trim();
                            value = string.Empty;
                        }
                        else
                        {
#if NETSTANDARD2_0
                            name = line.Substring(start, pos - start).Trim();
                            value = line.Substring(pos + 1);    // Do not trim value
#else
                            name = line[start..pos].Trim();
                            value = line[(pos + 1)..];    // Do not trim value
#endif
                        }

                        if (name.Length > 0)
                        {
                            // Ensure we have a section
                            if (section == null)
                            {
                                section = new IniSection(DefaultSectionName, StringComparer);
                                Sections.Add(section.Name, section);
                            }

                            if (section.TryGetValue(name, out IniSetting? setting))
                            {
                                // Override previously read value
                                setting.Value = value;
                            }
                            else
                            {
                                // Create new setting
                                setting = new IniSetting { Name = name, Value = value };
                                section.Add(name, setting);
                            }
                        }
                    }
                }
                // Read next line
                line = reader.ReadLine();
            }
        }

        /// <summary>
        /// Saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the INI file to write settings to.</param>
        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using StreamWriter writer = new(path, false);
            Save(writer);
        }

        /// <summary>
        /// Saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the INI file to write settings to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public void Save(string path, Encoding encoding)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            using StreamWriter writer = new(path, false, encoding);
            Save(writer);
        }

        /// <summary>
        /// Writes the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"></see> to save the settings
        /// to.</param>
        public void Save(StreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            bool firstLine = true;

            // Write comments
            if (Comments.Count > 0)
            {
                foreach (string? comment in Comments)
                    writer.WriteLine($"{CommentCharacter}{comment ?? string.Empty}");
                firstLine = false;
            }

            // Write settings
            foreach (IniSection section in Sections.Values)
            {
                if (section.Count > 0)
                {
                    // Write empty line if starting new section
                    if (firstLine)
                        firstLine = false;
                    else
                        writer.WriteLine();

                    writer.WriteLine("[{0}]", section.Name);
                    foreach (IniSetting setting in section.Values)
                        writer.WriteLine(setting.ToString());
                }
            }
        }

        #region Read values

        /// <summary>
        /// Returns the value of an INI setting.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found.</param>
        /// <returns>Returns the specified setting value.</returns>
        public string? GetSetting(string section, string setting, string? defaultValue = null)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            if (Sections.TryGetValue(section, out IniSection? iniSection))
            {
                if (iniSection.TryGetValue(setting, out IniSetting? iniSetting))
                {
                    Debug.Assert(iniSetting.Value != null);
                    return iniSetting.Value;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of an INI setting as an integer value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a integer value.</param>
        /// <returns>Returns the specified setting value as an integer value.</returns>
        public int GetSetting(string section, string setting, int defaultValue)
        {
            return int.TryParse(GetSetting(section, setting), out int value) ?
                value :
                defaultValue;
        }

        /// <summary>
        /// Returns the value of an INI setting as a double value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a double value.</param>
        /// <returns>Returns the specified setting value as a double value.</returns>
        public double GetSetting(string section, string setting, double defaultValue)
        {
            return double.TryParse(GetSetting(section, setting), out double value) ?
                value :
                defaultValue;
        }

        /// <summary>
        /// Returns the value of an INI setting as a Boolean value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a Boolean value.</param>
        /// <returns>Returns the specified setting value as a Boolean.</returns>
        public bool GetSetting(string section, string setting, bool defaultValue)
        {
            return BoolOptions.TryParse(GetSetting(section, setting), out bool value) ?
                value :
                defaultValue;
        }

        /// <summary>
        /// Returns all the section names in the current INI file.
        /// </summary>
        /// <returns>A list of all section names.</returns>
        public IEnumerable<string> GetSections() => Sections.Keys;

        /// <summary>
        /// Returns all settings in the given INI section.
        /// </summary>
        /// <param name="section">The name of the section that contains the settings to be retrieved.</param>
        /// <returns>Returns the settings in the given INI section.</returns>
        public IEnumerable<IniSetting> GetSectionSettings(string section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            return (Sections.TryGetValue(section, out IniSection? iniSection)) ?
                iniSection.Values :
                Enumerable.Empty<IniSetting>();
        }

#endregion

        #region Write values

        /// <summary>
        /// Sets an INI file setting. The setting is not written to disk until
        /// <see cref="Save"/> is called.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, string value)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            // Add section if needed
            if (!Sections.TryGetValue(section, out IniSection? iniSection))
            {
                iniSection = new IniSection(section, StringComparer);
                Sections.Add(iniSection.Name, iniSection);
            }
            // Add setting if needed
            if (!iniSection.TryGetValue(setting, out IniSetting? iniSetting))
            {
                iniSetting = new IniSetting { Name = setting };
                iniSection.Add(iniSetting.Name, iniSetting);
            }
            // Set setting value
            iniSetting.Value = value ?? string.Empty;
        }

        /// <summary>
        /// Sets an INI file setting with an integer value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, int value) => SetSetting(section, setting, value.ToString());

        /// <summary>
        /// Sets an INI file setting with a double value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, double value) => SetSetting(section, setting, value.ToString());

        /// <summary>
        /// Sets an INI file setting with a Boolean value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, bool value) => SetSetting(section, setting, BoolOptions.ToString(value));

        #endregion

        /// <summary>
        /// Clears all sections, settings and comments.
        /// </summary>
        public void Clear()
        {
            Sections.Clear();
            Comments.Clear();
        }
    }
}
