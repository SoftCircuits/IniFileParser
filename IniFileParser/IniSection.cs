// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Represents an INI file section.
    /// </summary>
    internal class IniSection : Dictionary<string, IniSetting>
    {
        /// <summary>
        /// The name of this INI section.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="IniSection"></see> instance.
        /// </summary>
        /// <param name="name">Name of this INI section.</param>
        /// <param name="comparer"><see cref="StringComparer"></see> used to
        /// look up setting names.</param>
        public IniSection(string name, StringComparer comparer)
            : base(comparer)
        {
            Name = name;
        }
    }
}
