// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Represents one name/value pair in an INI file.
    /// </summary>
    public class IniSetting
    {
        /// <summary>
        /// The name of this INI setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of this INI setting.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Overrides <see cref="ToString"></see>. Used to write the setting line to file.
        /// </summary>
        public override string ToString() => $"{Name ?? "(null)"}={Value ?? string.Empty}";
    }
}
