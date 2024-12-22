// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Represents an INI file section.
    /// </summary>
    /// <remarks>
    /// Constructs a new <see cref="IniSection"></see> instance.
    /// </remarks>
    /// <param name="name">Name of this INI section.</param>
    /// <param name="comparer"><see cref="StringComparer"></see> used to
    /// look up setting names.</param>
    internal class IniSection(string name, StringComparer comparer) : Dictionary<string, IniSetting>(comparer)
    {
        /// <summary>
        /// The name of this INI section.
        /// </summary>
        public string Name { get; private set; } = name;
    }
}
