// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCircuits.IniFileParser
{
    /// <summary>
    /// Options for reading Boolean setting values.
    /// </summary>
    public class BoolOptions
    {
        private Dictionary<string, bool> BoolStringLookup;
        private string TrueString = "true";
        private string FalseString = "false";

        /// <summary>
        /// Gets or sets whether any value that can be interpreted as a non-zero integer
        /// should be considered to be <c>true</c> when reading settings.
        /// </summary>
        public bool NonZeroNumbersAreTrue { get; set; }

        /// <summary>
        /// Constructs a <see cref="BoolOptions"></see> instance.
        /// </summary>
        /// <param name="comparer">Specifies the string comparer used to compare strings.
        /// If not supplied, <c>StringComparer.CurrentCultureIgnoreCase</c> is used.</param>
        public BoolOptions(StringComparer comparer = null)
        {
            BoolStringLookup = new Dictionary<string, bool>(comparer ?? StringComparer.CurrentCultureIgnoreCase)
            {
                [TrueString] = true,
                [FalseString] = false,
                ["yes"] = true,
                ["no"] = false,
                ["on"] = true,
                ["off"] = false,
                ["1"] = true,
                ["0"] = false,
            };
            NonZeroNumbersAreTrue = true;
        }

        /// <summary>
        /// Sets the words to be interpreted as Boolean values, replacing any
        /// existing Boolean words. Must include at least one <c>true</c> value
        /// and one <c>false</c> value.
        /// </summary>
        /// <param name="words">List of Boolean words and their corresponding value.</param>
        public void SetBoolWords(IEnumerable<BoolWord> words)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words));

            // Get default true word
            BoolWord word = words.FirstOrDefault(w => w.Value == true);
            if (word == null)
                throw new InvalidOperationException("Boolean word list contains no entry for 'true' values.");
            TrueString = word.Word;

            // Get default false word
            word = words.FirstOrDefault(w => w.Value == false);
            if (word == null)
                throw new InvalidOperationException("Boolean word list contains no entry for 'false' values.");
            FalseString = word.Word;

            // Store words in lookup table
            BoolStringLookup = words.ToDictionary(w => w.Word, w => w.Value);
        }

        /// <summary>
        /// Converts a Boolean value to a string.
        /// </summary>
        internal string ToString(bool value) => value ? TrueString : FalseString;

        /// <summary>
        /// Converts a string to a Boolean value.
        /// </summary>
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
                        // Non-zero = true; Zero = false
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
