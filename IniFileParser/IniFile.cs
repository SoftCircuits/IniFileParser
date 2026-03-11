// Copyright (c) 2019-2026 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
        /// Contains the list of comment lines read from a file, or that will be written
        /// to a file.
        /// </summary>
        public List<string?> Comments { get; private set; }

        /// <summary>
        /// Default section name. Used for any settings found outside a section header.
        /// </summary>
        public const string DefaultSectionName = "General";

        /// <summary>
        /// The default character used to signify a comment.
        /// </summary>
        public const char DefaultCommentCharacter = ';';

        /// <summary>
        /// The default format string used to encode <see cref="DateTime"/> values;
        /// </summary>
        public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// Gets or sets the format string used to encode and decode <see cref="DateTime"/> values.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the character used to signify a comment. Must be the first
        /// non-space character on the line. Default value is a semicolon (<c>;</c>).
        /// </summary>
        public char CommentCharacter { get; set; }

        /// <summary>
        /// Constructs a new <see cref="IniFile"></see> instance.
        /// </summary>
        /// <param name="comparer"><see cref="StringComparer"></see> used to compare section and setting
        /// names. If not specified, <see cref="StringComparer.CurrentCultureIgnoreCase"></see> is used
        /// (i.e. names are not case-sensitive).</param>
        /// <param name="boolOptions">Optional settings for interpreting <c>bool</c> values.</param>
        public IniFile(StringComparer? comparer = null, BoolOptions? boolOptions = null)
        {
            StringComparer = comparer ?? StringComparer.CurrentCultureIgnoreCase;
            BoolOptions = boolOptions ?? new BoolOptions();
            Sections = new Dictionary<string, IniSection>(StringComparer);
            Comments = [];
            DateTimeFormat = DefaultDateTimeFormat;
            CommentCharacter = DefaultCommentCharacter;
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load.</param>
        public void Load(string path)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path);
            Load(reader);
        }

        /// <summary>
        /// Asynchronously loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load.</param>
        public async Task LoadAsync(string path)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path);
            await LoadAsync(reader);
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path, detectEncodingFromByteOrderMarks);
            Load(reader);
        }

        /// <summary>
        /// Asynchronously loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public async Task LoadAsync(string path, bool detectEncodingFromByteOrderMarks)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path, detectEncodingFromByteOrderMarks);
            await LoadAsync(reader);
        }

        /// <summary>
        /// Loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load settings from.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public void Load(string path, Encoding encoding)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path, encoding);
            Load(reader);
        }

        /// <summary>
        /// Asynchronously loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load settings from.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public async Task LoadAsync(string path, Encoding encoding)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path, encoding);
            await LoadAsync(reader);
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
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path, encoding, detectEncodingFromByteOrderMarks);
            Load(reader);
        }

        /// <summary>
        /// Asynchronously loads all settings from the specified INI file. Overwrites any existing settings.
        /// </summary>
        /// <param name="path">Path of the INI file to load settings from.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public async Task LoadAsync(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamReader reader = new(path, encoding, detectEncodingFromByteOrderMarks);
            await LoadAsync(reader);
        }

        /// <summary>
        /// Loads all settings from the specified stream. Overwrites any existing settings.
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"></see> to load settings from.</param>
        public void Load(StreamReader reader)
        {
#if NETSTANDARD2_0
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
#else
            ArgumentNullException.ThrowIfNull(reader);
#endif

            // Tracks the current section
            IniSection? section = null;
            string? line;

            // Clear any existing data
            Clear();

            // Read lines
            line = reader.ReadLine();
            while (line != null)
            {
                ParseLine(line, ref section);
                line = reader.ReadLine();
            }
        }

        /// <summary>
        /// Asynchronously loads all settings from the specified stream. Overwrites any existing settings.
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"></see> to load settings from.</param>
        public async Task LoadAsync(StreamReader reader)
        {
#if NETSTANDARD2_0
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
#else
            ArgumentNullException.ThrowIfNull(reader);
#endif

            // Tracks the current section
            IniSection? section = null;
            string? line;

            // Clear any existing data
            Clear();

            // Read lines
            line = await reader.ReadLineAsync();
            while (line != null)
            {
                ParseLine(line, ref section);
                line = await reader.ReadLineAsync();
            }
        }

        /// <summary>
        /// Parses a line from an INI file.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <param name="section">Reference to the <see cref="IniSection"/> that represents the current section.</param>
        private void ParseLine(string line, ref IniSection? section)
        {
            // Trim leading whitespace
            int start = 0;
            while (start < line.Length && char.IsWhiteSpace(line[start]))
                start++;

            // Ignore empty lines
            if (start >= line.Length)
                return;

            // Process line
            if (line[start] == CommentCharacter)
            {
                // Store comment
                start++;
#if !NETSTANDARD2_0
                Comments.Add(line[start..]);
#else
                Comments.Add(line.Substring(start));
#endif
            }
            else if (line[start] == '[')
            {
                // Parse section header
                start++;
                int pos = line.IndexOf(']', start);
                if (pos == -1)
                    pos = line.Length;

                // Trim whitespace
                while (start < pos && char.IsWhiteSpace(line[start]))
                    start++;
                while (pos > start && char.IsWhiteSpace(line[pos - 1]))
                    pos--;

                if (pos > start)
                {
#if !NETSTANDARD2_0
                    string name = line[start..pos];
#else
                    string name = line.Substring(start, pos - start);
#endif
                    // Add section if it doesn't already exist
                    if (!Sections.ContainsKey(name))
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
                    pos = line.Length;
                    // Trim whitespace
                    while (pos > start && char.IsWhiteSpace(line[pos - 1]))
                        pos--;
#if !NETSTANDARD2_0
                    name = line[start..pos];
#else
                    name = line.Substring(start, pos - start);
#endif
                    value = string.Empty;
                }
                else
                {
                    // Do not trim end of value
#if !NETSTANDARD2_0
                    value = line[(pos + 1)..];
#else
                    value = line.Substring(pos + 1);
#endif
                    // Trim whitespace
                    while (pos > start && char.IsWhiteSpace(line[pos - 1]))
                        pos--;
#if !NETSTANDARD2_0
                    name = line[start..pos];
#else
                    name = line.Substring(start, pos - start);
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

        /// <summary>
        /// Saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the INI file to write settings to.</param>
        public void Save(string path)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamWriter writer = new(path, false);
            Save(writer);
        }

        /// <summary>
        /// Asynchronously saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the INI file to write settings to.</param>
        public async Task SaveAsync(string path)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamWriter writer = new(path, false);
            await SaveAsync(writer);
        }

        /// <summary>
        /// Saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the INI file to write settings to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public void Save(string path, Encoding encoding)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamWriter writer = new(path, false, encoding);
            Save(writer);
        }

        /// <summary>
        /// Asynchronously saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the INI file to write settings to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public async Task SaveAsync(string path, Encoding encoding)
        {
#if NETSTANDARD2_0
            if (path == null)
                throw new ArgumentNullException(nameof(path));
#else
            ArgumentNullException.ThrowIfNull(path);
#endif

            using StreamWriter writer = new(path, false, encoding);
            await SaveAsync(writer);
        }

        /// <summary>
        /// Writes the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"></see> to save the settings
        /// to.</param>
        public void Save(StreamWriter writer)
        {
#if NETSTANDARD2_0
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
#else
            ArgumentNullException.ThrowIfNull(writer);
#endif

            foreach (string line in GetSaveLines())
                writer.WriteLine(line);
        }

        /// <summary>
        /// Asynchronously writes the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"></see> to save the settings
        /// to.</param>
        public async Task SaveAsync(StreamWriter writer)
        {
#if NETSTANDARD2_0
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
#else
            ArgumentNullException.ThrowIfNull(writer);
#endif

            foreach (string line in GetSaveLines())
                await writer.WriteLineAsync(line);
        }

        /// <summary>
        /// Builds the lines to be written to an INI file. This includes comment lines and section and setting lines.
        /// </summary>
        private IEnumerable<string> GetSaveLines()
        {
            bool firstLine = true;

            // Write comments
            if (Comments.Count > 0)
            {
                foreach (string? comment in Comments)
                    yield return $"{CommentCharacter}{comment ?? string.Empty}";
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
                        yield return string.Empty;

                    yield return $"[{section.Name}]";
                    foreach (IniSetting setting in section.Values)
                        yield return setting.ToString();
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
#if NETSTANDARD2_0
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
#else
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(setting);
#endif

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
        /// Returns the value of an INI setting as an <see cref="System.Int32"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a <see cref="System.Int32"/> value.</param>
        /// <returns>Returns the specified setting value as an <see cref="System.Int32"/> value.</returns>
        public int GetSetting(string section, string setting, int defaultValue) =>
            int.TryParse(GetSetting(section, setting), out int value) ? value : defaultValue;

        /// <summary>
        /// Returns the value of an INI setting as a <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a <see cref="System.Double"/> value.</param>
        /// <returns>Returns the specified setting value as a <see cref="System.Double"/> value.</returns>
        public double GetSetting(string section, string setting, double defaultValue) =>
            double.TryParse(GetSetting(section, setting), out double value) ? value : defaultValue;

        /// <summary>
        /// Returns the value of an INI setting as a <see cref="System.Boolean"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a <see cref="System.Boolean"/> value.</param>
        /// <returns>Returns the specified setting value as a <see cref="System.Boolean"/>.</returns>
        public bool GetSetting(string section, string setting, bool defaultValue) =>
            BoolOptions.TryParse(GetSetting(section, setting), out bool value) ? value : defaultValue;

        /// <summary>
        /// Returns the value of an INI setting as a <see cref="System.DateTime"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a <see cref="System.DateTime"/> value.</param>
        /// <returns>Returns the specified setting value as a <see cref="System.DateTime"/>.</returns>
        public DateTime GetSetting(string section, string setting, DateTime defaultValue) =>
            DateTime.TryParseExact(GetSetting(section, setting), DateTimeFormat, null, DateTimeStyles.None, out DateTime value) ? value : defaultValue;

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
#if NETSTANDARD2_0
            if (section == null)
                throw new ArgumentNullException(nameof(section));
#else
            ArgumentNullException.ThrowIfNull(section);
#endif

            return (Sections.TryGetValue(section, out IniSection? iniSection)) ? iniSection.Values : [];
        }

        #endregion

        #region Write values

        /// <summary>
        /// Sets an INI file setting. The setting is not written to disk until
        /// <see cref="Save(string)"/> or <see cref="SaveAsync(string)"/> is called.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, string value)
        {
#if NETSTANDARD2_0
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
#else
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(setting);
#endif

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
        /// Sets an INI file setting with an <see cref="System.Int32"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, int value) => SetSetting(section, setting, value.ToString());

        /// <summary>
        /// Sets an INI file setting with a <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, double value) => SetSetting(section, setting, value.ToString());

        /// <summary>
        /// Sets an INI file setting with a <see cref="System.Boolean"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, bool value) => SetSetting(section, setting, BoolOptions.ToString(value));

        /// <summary>
        /// Sets an INI file setting with a <see cref="System.DateTime"/> value.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <param name="value">The value of the INI file setting.</param>
        public void SetSetting(string section, string setting, DateTime value) => SetSetting(section, setting, value.ToString(DateTimeFormat));

        /// <summary>
        /// Deletes the specified section and all settings within that section.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <returns>True if the section was deleted, false if the section was not found.</returns>
        public bool DeleteSection(string section)
        {
#if NETSTANDARD2_0
            if (section == null)
                throw new ArgumentNullException(nameof(section));
#else
            ArgumentNullException.ThrowIfNull(section);
#endif
            return Sections.Remove(section);
        }

        /// <summary>
        /// Deletes the specified setting.
        /// </summary>
        /// <param name="section">The INI file section name.</param>
        /// <param name="setting">The name of the INI file setting.</param>
        /// <returns>True if the setting was deleted, false if the setting was not found.</returns>
        public bool DeleteSetting(string section, string setting)
        {
#if NETSTANDARD2_0
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
#else
            ArgumentNullException.ThrowIfNull(section);
            ArgumentNullException.ThrowIfNull(setting);
#endif
            if (Sections.TryGetValue(section, out IniSection? iniSection))
                return iniSection.Remove(setting);

            return false;
        }

        /// <summary>
        /// Clears all sections, settings and comments.
        /// </summary>
        public void Clear()
        {
            Sections.Clear();
            Comments.Clear();
        }

        #endregion

    }
}
