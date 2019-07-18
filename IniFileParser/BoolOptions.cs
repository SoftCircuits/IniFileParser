// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;

namespace SoftCircuits.IniFileParser
{
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
        /// Initializes an instance of BoolWord.
        /// </summary>
        /// <param name="word">A word that can be interpreted as a Boolean value.</param>
        /// <param name="value">The Boolean value of the associated word.</param>
        public BoolWord(string word, bool value)
        {
            Word = word;
            Value = value;
        }
    }

    /// <summary>
    /// Options for reading Boolean setting values
    /// </summary>
    public class BoolOptions
    {
        /// <summary>
        /// If <c>True</c>, any value that can be interpreted as a non-zero integer
        /// should be considered to be true when getting settings.
        /// </summary>
        public bool NonZeroNumbersAreTrue { get; set; }

        private readonly Dictionary<string, bool> BoolStringLookup;

        /// <summary>
        /// Initializes a <c>BoolOptions</c> instance.
        /// </summary>
        /// <param name="comparer">Specifies the string comparer used to compare strings.
        /// If not supplied, <c>StringComparer.CurrentCultureIgnoreCase</c> is used.</param>
        public BoolOptions(StringComparer comparer = null)
        {
            NonZeroNumbersAreTrue = true;
            BoolStringLookup = new Dictionary<string, bool>(comparer ?? StringComparer.CurrentCultureIgnoreCase)
            {
                ["true"] = true,
                ["false"] = false,
                ["yes"] = true,
                ["no"] = false,
                ["on"] = true,
                ["off"] = false,
                ["1"] = true,
                ["0"] = false,
            };
        }

        /// <summary>
        /// Sets the words to be interpreted as Boolean values, replacing any
        /// existing words.
        /// </summary>
        /// <param name="words">List of Boolean words and their corresponding value.</param>
        public void SetBoolWords(IEnumerable<BoolWord> words)
        {
            BoolStringLookup.Clear();
            foreach (BoolWord word in words)
                BoolStringLookup.Add(word.Word, word.Value);
        }

        internal bool TryParse(string s, out bool value)
        {
            if (s != null)
            {
                if (BoolStringLookup.TryGetValue(s, out bool b))
                {
                    value = b;
                    return true;
                }
                if (NonZeroNumbersAreTrue)
                {
                    if (int.TryParse(s, out int i))
                    {
                        // Non-zero integer = true; Zero = false
                        value = (i != 0);
                        return true;
                    }
                }
            }
            value = false;
            return false;
        }
    }
}
