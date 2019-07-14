// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Represents an entire INI file section.
    /// </summary>
    internal class IniSection
    {
        public string Name { get; set; }
        public Dictionary<string, IniSetting> Settings { get; private set; }

        public IniSection(StringComparer comparer)
        {
            Settings = new Dictionary<string, IniSetting>(comparer);
        }
    }

    /// <summary>
    /// Class to create and read INI files.
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// Section used for settings not under any section header (within [])
        /// </summary>
        public const string DefaultSectionName = "General";

        private Dictionary<string, IniSection> Sections;
        private StringComparer StringComparer;

        /// <summary>
        /// Initializes a new IniFile instance.
        /// </summary>
        /// <param name="comparer"><c>StringComparer</c> used to compare section and setting names.
        /// If not specified, <c>StringComparer.OrdinalIgnoreCase</c> is used (i.e. names are not
        /// case-sensitive).</param>
        public IniFile(StringComparer comparer = null)
        {
            StringComparer = comparer ?? StringComparer.OrdinalIgnoreCase;
            Sections = new Dictionary<string, IniSection>(StringComparer);
        }

        #region File functions

        /// <summary>
        /// Loads an INI settings file.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        public void Load(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads an INI settings file.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads an INI settings file.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public void Load(string path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads an INI settings file.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the
        /// beginning of the file.</param>
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            using (StreamReader reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads an INI settings file.
        /// </summary>
        /// <param name="reader">The <c>StreamReader</c> to load the settings from.</param>
        public void Load(StreamReader reader)
        {
            Sections.Clear();

            // Default section
            IniSection section = null;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.TrimStart();
                if (line.Length > 0)
                {
                    if (line[0] == ';')
                    {
                        // Ignore comments
                    }
                    else if (line[0] == '[')
                    {
                        // Parse section header
                        int pos = line.IndexOf(']', 1);
                        if (pos == -1)
                            pos = line.Length;
                        string name = line.Substring(1, pos - 1).Trim();
                        if (name.Length > 0)
                        {
                            if (!Sections.TryGetValue(name, out section))
                            {
                                section = new IniSection(StringComparer) { Name = name };
                                Sections.Add(section.Name, section);
                            }
                        }
                    }
                    else
                    {
                        // Parse setting name and value
                        string name, value;

                        int pos = line.IndexOf('=');
                        if (pos == -1)
                        {
                            name = line.Trim();
                            value = string.Empty;
                        }
                        else
                        {
                            name = line.Substring(0, pos).Trim();
                            value = line.Substring(pos + 1);
                        }

                        if (name.Length > 0)
                        {

                            if (section == null)
                            {
                                section = new IniSection(StringComparer) { Name = DefaultSectionName };
                                Sections.Add(section.Name, section);
                            }


                            if (section.Settings.TryGetValue(name, out IniSetting setting))
                            {
                                // Override previously read value
                                setting.Value = value;
                            }
                            else
                            {
                                // Create new setting
                                setting = new IniSetting { Name = name, Value = value };
                                section.Settings.Add(name, setting);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the file to write to.</param>
        public void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                Save(writer);
            }
        }

        /// <summary>
        /// Saves the current settings to an INI file. If the file already exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="path">Path of the file to write to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public void Save(string path, Encoding encoding)
        {
            using (StreamWriter writer = new StreamWriter(path, false, encoding))
            {
                Save(writer);
            }
        }

        /// <summary>
        /// Writes the current settings to an INI file. If the file already exists, it is overwritten.
        /// </summary>
        /// <param name="filename">Path of file to write to.</param>
        public void Save(StreamWriter writer)
        {
            bool firstLine = true;
            foreach (IniSection section in Sections.Values)
            {
                if (firstLine)
                    firstLine = false;
                else
                    writer.WriteLine();

                if (section.Settings.Any())
                {
                    writer.WriteLine("[{0}]", section.Name);
                    foreach (IniSetting setting in section.Settings.Values)
                        writer.WriteLine("{0}={1}", setting.Name, setting.Value);
                }
            }
        }

        #endregion

        #region Read values

        /// <summary>
        /// Returns the value of an INI setting.
        /// </summary>
        /// <param name="section">The INI file section.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found.</param>
        /// <returns>Returns the specified setting value.</returns>
        public string GetSetting(string section, string setting, string defaultValue = null)
        {
            if (Sections.TryGetValue(section, out IniSection iniSection))
            {
                if (iniSection.Settings.TryGetValue(setting, out IniSetting iniSetting))
                    return iniSetting.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of an INI setting as an integer value.
        /// </summary>
        /// <param name="section">The INI file section.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a integer value.</param>
        /// <returns>Returns the specified setting value as an integer value.</returns>
        public int GetSetting(string section, string setting, int defaultValue)
        {
            if (int.TryParse(GetSetting(section, setting), out int value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of an INI setting as a double value.
        /// </summary>
        /// <param name="section">The INI file section.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a double value.</param>
        /// <returns>Returns the specified setting value as a double value.</returns>
        public double GetSetting(string section, string setting, double defaultValue)
        {
            if (double.TryParse(GetSetting(section, setting), out double value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Returns the value of an INI setting as a Boolean value.
        /// </summary>
        /// <param name="section">The INI file section.</param>
        /// <param name="setting">The INI setting name.</param>
        /// <param name="defaultValue">The value to return if the setting was not found,
        /// or if it could not be converted to a Boolean value.</param>
        /// <returns>Returns the specified setting value as a Boolean.</returns>
        public bool GetSetting(string section, string setting, bool defaultValue)
        {
            if (BooleanTryParse(GetSetting(section, setting), out bool value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Returns the name of all sections in the current INI file.
        /// </summary>
        /// <returns>A list of all section names.</returns>
        public IEnumerable<string> GetSections()
        {
            foreach (KeyValuePair<string, IniSection> section in Sections)
                yield return section.Key;
        }

        /// <summary>
        /// Returns all settings in the given INI section.
        /// </summary>
        /// <param name="section">The section that contains the settings to be retrieved.</param>
        /// <returns>Returns the settings in the given INI section.</returns>
        public IEnumerable<IniSetting> GetSectionSettings(string section)
        {
            if (Sections.TryGetValue(section, out IniSection iniSection))
            {
                foreach (var setting in iniSection.Settings)
                    yield return setting.Value;
            }
        }

        #endregion

        #region Write values

        /// <summary>
        /// Sets an INI file setting. The setting is not written to disk until
        /// <see cref="Save"/> is called.
        /// </summary>
        /// <param name="section">The INI-file section.</param>
        /// <param name="setting">The name of the INI-file setting.</param>
        /// <param name="value">The value of the INI-file setting</param>
        public void SetSetting(string section, string setting, string value)
        {
            if (!Sections.TryGetValue(section, out IniSection iniSection))
            {
                iniSection = new IniSection(StringComparer) { Name = section };
                Sections.Add(iniSection.Name, iniSection);
            }
            if (!iniSection.Settings.TryGetValue(setting, out IniSetting iniSetting))
            {
                iniSetting = new IniSetting { Name = setting };
                iniSection.Settings.Add(iniSetting.Name, iniSetting);
            }
            iniSetting.Value = value;
        }

        /// <summary>
        /// Sets an INI file setting with an integer value.
        /// </summary>
        /// <param name="section">The INI-file section.</param>
        /// <param name="setting">The name of the INI-file setting.</param>
        /// <param name="value">The value of the INI-file setting</param>
        public void SetSetting(string section, string setting, int value)
        {
            SetSetting(section, setting, value.ToString());
        }

        /// <summary>
        /// Sets an INI file setting with a double value.
        /// </summary>
        /// <param name="section">The INI-file section.</param>
        /// <param name="setting">The name of the INI-file setting.</param>
        /// <param name="value">The value of the INI-file setting</param>
        public void SetSetting(string section, string setting, double value)
        {
            SetSetting(section, setting, value.ToString());
        }

        /// <summary>
        /// Sets an INI file setting with a Boolean value.
        /// </summary>
        /// <param name="section">The INI-file section.</param>
        /// <param name="setting">The name of the INI-file setting.</param>
        /// <param name="value">The value of the INI-file setting</param>
        public void SetSetting(string section, string setting, bool value)
        {
            SetSetting(section, setting, value.ToString());
        }

        #endregion

        #region Boolean parsing

        private string[] TrueStrings = { "true", "yes", "on" };
        private string[] FalseStrings = { "false", "no", "off" };

        private bool BooleanTryParse(string s, out bool value)
        {
            if (TrueStrings.Any(s2 => string.Compare(s, s2, true) == 0))
            {
                value = true;
                return true;
            }
            if (FalseStrings.Any(s2 => string.Compare(s, s2, true) == 0))
            {
                value = false;
                return true;
            }
            if (int.TryParse(s, out int i))
            {
                value = (i != 0);
                return true;
            }
            value = false;
            return false;
        }

        #endregion

        /// <summary>
        /// Clears all settings and setting sections.
        /// </summary>
        public void Clear() => Sections.Clear();

        //public void Dump()
        //{
        //    foreach (IniSection section in Sections.Values)
        //    {
        //        Debug.WriteLine(string.Format("[{0}]", section.Name));
        //        foreach (IniSetting setting in section.Settings.Values)
        //            Debug.WriteLine("[{0}]=[{1}]", setting.Name, setting.Value);
        //    }
        //}

    }
}
