// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Finds the first <c>true</c> word and first <c>false</c> word in the given
        /// word list, and returns their corresponding word values.
        /// </summary>
        /// <param name="words">List of <see cref="BoolWord"></see>s.</param>
        public static void GetTrueFalseWords(IEnumerable<BoolWord> words, out string trueString, out string falseString)
        {
            BoolWord boolWord;

            // Find 'true' word
            boolWord = words.FirstOrDefault(w => w.Value == true);
            if (boolWord == null)
                throw new Exception("Word list contains no words for 'true' values.");
            trueString = boolWord.Word;
            // Find 'false' word
            boolWord = words.FirstOrDefault(w => w.Value == false);
            if (boolWord == null)
                throw new Exception("Word list contains no words for 'false' values.");
            falseString = boolWord.Word;
        }
    }
}
