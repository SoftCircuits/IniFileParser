// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Represents an entire INI file section.
    /// </summary>
    internal class IniSection : Dictionary<string, IniSetting>
    {
        public string Name { get; set; }

        public IniSection(string name, StringComparer comparer)
            : base(comparer)
        {
            Name = name;
        }
    }
}
