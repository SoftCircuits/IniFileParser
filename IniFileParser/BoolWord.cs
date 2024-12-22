// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Defines a single word that can be interpreted as a <see cref="System.Boolean"/> value.
    /// </summary>
    public class BoolWord
    {
        /// <summary>
        /// Specifies a word that can be interpreted as a Boolean value.
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// Specifies the Boolean value of the associated word.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Constructs a <see cref="BoolWord"></see> instance.
        /// </summary>
        /// <param name="word">A word that can be interpreted as a Boolean value.</param>
        /// <param name="value">The Boolean value of the associated word.</param>
        public BoolWord(string word, bool value)
        {
            Word = word;
            Value = value;
        }
    }
}
