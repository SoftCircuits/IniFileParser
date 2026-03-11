// Copyright (c) 2019-2026 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Represents the name/value pair of an INI-file setting.
    /// </summary>
    public class IniSetting
    {
        /// <summary>
        /// The name of this INI setting.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The value of this INI setting.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Converts this <see cref="IniSetting"></see> to a string.
        /// </summary>
        /// <remarks>
        /// Returns this setting formatted appropriately for writing to an INI file.
        /// </remarks>
        public override string ToString() => $"{Name}={Value}";
    }
}
