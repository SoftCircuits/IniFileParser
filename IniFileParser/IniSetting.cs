// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
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

        private static readonly string Null = "(null)";

        public override string ToString() => $"{Name ?? Null}={(Value != null ? $"\"{Value}\"" : Null)}";
    }
}
