// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.IniFileParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IniParser.Tests
{
    [TestClass]
    public class IniFileTests
    {
        private readonly string[] Sections =
        {
            "GrRkfUPlTn",
            "MHvqnvgjqL",
            "mSXWTrLnvh",
            "EYEkCmmnpH",
            "GMtwdWTGLi",
            "nOQdCPysMC",
            "aKZSjRkKyl",
            "oZtZhwJjBq",
            "PTKHCxEBbT",
            "PidCWOuCUy",
        };

        private readonly (string Name, string Value)[] StringValues = new (string Name, string Value)[]
        {
            ("gbmoAGoUAX", "nhsvoVCFeS"),
            ("XknXpFZwAn", "lWShEVKQja"),
            ("HDekUKYhQI", "JECcqkbsWj"),
            ("OMzEAOThDc", "AOLxPMBlys"),
            ("nLQfiLEUbC", "IwmKthVzgI"),
            ("pBUqOjUVrP", "KUlfghqlwM"),
            ("vMKHoVuDdp", "GLNbCnhGQR"),
            ("yhUWOpEMys", "hQlQolVhVy"),
            ("cxIygWRbgc", "jjYvzGlYEg"),
            ("iBJOskHHDX", "DazniZGfot"),
        };

        private readonly (string Name, int Value)[] IntValues = new (string Name, int Value)[]
        {
            ("RdxljKJfFz", 98423023),
            ("EnMeiBRqNg", 202612153),
            ("bUHJQbePEf", 386456548),
            ("nyvZolLMUc", 127322448),
            ("ynqwRVbRwF", 197385250),
            ("gKukfnfwLp", 106966128),
            ("tBlZXBkTlB", 67822807),
            ("HnonxtSYwE", 277714502),
            ("vejHyhMynk", 179483129),
            ("SXpdOXFNVr", 138444389),
        };

        private readonly (string Name, double Value)[] DoubleValues = new (string Name, double Value)[]
        {
            ("OwKRKjYfPH", 0.7080722530),
            ("FanKstopNJ", 0.5025695865),
            ("zEJOKrIoRN", 0.8479851980),
            ("XIIdmOtRLq", 0.4750674692),
            ("vPveQiqyTX", 0.1760267518),
            ("YXVkXUaAwS", 0.0971605970),
            ("VGhLoQaPqB", 0.8404616145),
            ("dIvDIbeEhZ", 0.2999469886),
            ("EZTmbnoMFN", 0.0680769511),
            ("TWfWqkMubh", 0.1385922434),
        };

        private readonly (string Name, bool Value)[] BoolValues = new (string Name, bool Value)[]
        {
            ("QKgfNDkkjI", true),
            ("dcciuRwwyD", true),
            ("pvKHvDqNCm", false),
            ("oOriwnnKni", true),
            ("aPnWFKvbzK", false),
            ("NeMmlJsCwa", true),
            ("wMfdfKYEKj", false),
            ("GLdUrFdAex", false),
            ("VsmGCAwcTp", true),
            ("KcNDDohDUb", false),
        };

        // Sum of all values lists
        private readonly int TotalItems;

        public IniFileTests()
        {
            TotalItems = StringValues.Length + IntValues.Length + DoubleValues.Length + BoolValues.Length;
        }

        [TestMethod]
        public void TestValues()
        {
            IniFile file = new();
            foreach (string section in Sections)
            {
                foreach ((string Name, string Value) in StringValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, int Value) in IntValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, double Value) in DoubleValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, bool Value) in BoolValues)
                    file.SetSetting(section, Name, Value);
            }
            byte[] buffer = SaveToBytes(file);

            file.Clear();
            Assert.AreEqual(0, file.GetSections().Count());

            LoadFromBytes(file, buffer);
            Assert.AreEqual(Sections.Length, file.GetSections().Count());
            foreach (string section in Sections)
            {
                Assert.AreEqual(TotalItems, file.GetSectionSettings(section).Count());
                foreach ((string Name, string Value) in StringValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, string.Empty));
                foreach ((string Name, int Value) in IntValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, 0));
                foreach ((string Name, double Value) in DoubleValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, 0.0));
                foreach ((string Name, bool Value) in BoolValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, false));
            }
        }

        [TestMethod]
        public void TestWhitespace()
        {
            StringBuilder builder = new();
            Random rand = new();

            builder.AppendLine("  ;  Comment");
            builder.AppendLine("  ;  Comment");
            builder.AppendLine("  ;  Comment");

            foreach (string section in Sections)
            {
                builder.AppendLine();
                string spaces = Spaces(rand);
                builder.AppendLine($"{spaces}[{spaces}{section}{spaces}{spaces}]{spaces}");
                foreach ((string Name, string Value) in StringValues)
                    builder.AppendLine($"{spaces}{Name}{spaces}={Value}");
                foreach ((string Name, int Value) in IntValues)
                    builder.AppendLine($"{spaces}{Name}{spaces}={Value}");
                foreach ((string Name, double Value) in DoubleValues)
                    builder.AppendLine($"{spaces}{Name}{spaces}={Value}");
                foreach ((string Name, bool Value) in BoolValues)
                    builder.AppendLine($"{spaces}{Name}{spaces}={Value}");
            }
            byte[] buffer = SaveToBytes(builder.ToString());

            IniFile file = new();
            LoadFromBytes(file, buffer);
            Assert.AreEqual(Sections.Length, file.GetSections().Count());
            foreach (string section in Sections)
            {
                Assert.AreEqual(TotalItems, file.GetSectionSettings(section).Count());
                foreach ((string Name, string Value) in StringValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, string.Empty));
                foreach ((string Name, int Value) in IntValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, 0));
                foreach ((string Name, double Value) in DoubleValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, 0.0));
                foreach ((string Name, bool Value) in BoolValues)
                    Assert.AreEqual(Value, file.GetSetting(section, Name, false));
            }
        }

        private static string Spaces(Random rand) => new(' ', rand.Next(2, 14));

        [TestMethod]
        public void TestStringComparer()
        {
            IniFile file = new(StringComparer.Ordinal);
            foreach (string section in Sections)
            {
                foreach ((string Name, string Value) in StringValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, int Value) in IntValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, double Value) in DoubleValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, bool Value) in BoolValues)
                    file.SetSetting(section, Name, Value);
            }
            byte[] buffer = SaveToBytes(file);

            file.Clear();
            Assert.AreEqual(0, file.GetSections().Count());

            LoadFromBytes(file, buffer);
            Assert.AreEqual(Sections.Length, file.GetSections().Count());
            foreach (string section in Sections)
            {
                Assert.AreEqual(TotalItems, file.GetSectionSettings(section).Count());
                foreach ((string Name, string Value) in StringValues)
                {
                    Assert.AreEqual(string.Empty, file.GetSetting(section, Name.ToUpper(), string.Empty));
                    Assert.AreEqual(string.Empty, file.GetSetting(section, Name.ToLower(), string.Empty));
                    Assert.AreEqual(Value, file.GetSetting(section, Name, string.Empty));
                }
                foreach ((string Name, int Value) in IntValues)
                {
                    Assert.AreEqual(0, file.GetSetting(section, Name.ToUpper(), 0));
                    Assert.AreEqual(0, file.GetSetting(section, Name.ToLower(), 0));
                    Assert.AreEqual(Value, file.GetSetting(section, Name, 0));
                }
                foreach ((string Name, double Value) in DoubleValues)
                {
                    Assert.AreEqual(0.0, file.GetSetting(section, Name.ToUpper(), 0.0));
                    Assert.AreEqual(0.0, file.GetSetting(section, Name.ToLower(), 0.0));
                    Assert.AreEqual(Value, file.GetSetting(section, Name, 0.0));
                }
                foreach ((string Name, bool Value) in BoolValues)
                {
                    Assert.AreEqual(false, file.GetSetting(section, Name.ToUpper(), false));
                    Assert.AreEqual(false, file.GetSetting(section, Name.ToLower(), false));
                    Assert.AreEqual(Value, file.GetSetting(section, Name, false));
                }
            }
        }

        [TestMethod]
        public void TestSection()
        {
            IniFile file = new();
            foreach (string section in Sections)
            {
                foreach ((string Name, string Value) in StringValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, int Value) in IntValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, double Value) in DoubleValues)
                    file.SetSetting(section, Name, Value);
                foreach ((string Name, bool Value) in BoolValues)
                    file.SetSetting(section, Name, Value);
            }
            byte[] buffer = SaveToBytes(file);

            file.Clear();
            Assert.AreEqual(0, file.GetSections().Count());

            LoadFromBytes(file, buffer);
            Assert.AreEqual(Sections.Length, file.GetSections().Count());
            var settings = file.GetSectionSettings(Sections[0]).ToArray();
            Assert.AreEqual(TotalItems, settings.Length);

            int i = 0, j;
            for (j = 0; j < StringValues.Length; j++)
            {
                Assert.AreEqual(StringValues[j].Name, settings[j + i].Name);
                Assert.AreEqual(StringValues[j].Value, settings[j + i].Value);
            }
            i += j;
            for (j = 0; j < IntValues.Length; j++)
            {
                Assert.AreEqual(IntValues[j].Name, settings[j + i].Name);
                Assert.AreEqual(IntValues[j].Value, int.Parse(settings[j + i].Value));
            }
            i += j;
            for (j = 0; j < DoubleValues.Length; j++)
            {
                Assert.AreEqual(DoubleValues[j].Name, settings[j + i].Name);
                Assert.AreEqual(DoubleValues[j].Value, double.Parse(settings[j + i].Value));
            }
            i += j;
            for (j = 0; j < BoolValues.Length; j++)
            {
                Assert.AreEqual(BoolValues[j].Name, settings[j + i].Name);
                Assert.AreEqual(BoolValues[j].Value, bool.Parse(settings[j + i].Value));
            }
        }

        [TestMethod]
        public void TestCommentCharacter()
        {
            List<(char c, string[] keys)> comments = new()
            {
                ('\0', new [] { "a", "#c", "@d" }),
                (';', new [] { "a", "#c", "@d" }),
                ('#', new [] { "a", ";b", "@d" }),
                ('@', new [] { "a", ";b", "#c" }),
            };

            string contents = @"[General]
a=0
;b=1
#c=2
@d=3
";
            byte[] contentBytes = SaveToBytes(contents);

            foreach (var (c, keys) in comments)
            {
                IniFile file = new();

                if (c != '\0')
                    file.CommentCharacter = c;
                LoadFromBytes(file, contentBytes);
                var settings = file.GetSectionSettings(IniFile.DefaultSectionName);
                CollectionAssert.AreEqual(keys, settings.Select(s => s.Name).ToList());
            }
        }

        private static readonly List<(string Setting, string Word, bool Value, bool CanRead)> BoolOptionData = new()
        {
            ("Setting1", "vraie", true, true),
            ("Setting2", "faux", false, true),
            ("Setting3", "oui", true, true),
            ("Setting4", "non", false, true),
            ("Setting5", "sur", true, true),
            ("Setting6", "de", false, true),
            ("Setting7", "Boolean", false, false),
            ("Setting8", "Double", false, false),
            ("Setting9", "Vraie", true, false),
            ("Setting10", "Faux", false, false),
            ("Setting11", "Oui", true, false),
            ("Setting12", "Non", false, false),
            ("Setting13", "Sur", true, false),
            ("Setting14", "De", false, false),
            ("Setting15", "1", true, false),
            ("Setting16", "0", false, false),
            ("Setting17", "True", true, false),
            ("Setting18", "False", false, false),
        };

        [TestMethod]
        public void TestBoolOptions()
        {
            string stringSection = "StringSection";
            string boolSection = "BooleanSection";
            BoolOptions options = new(StringComparer.Ordinal);
            options.SetBoolWords(new[] {
                new BoolWord("vraie", true),
                new BoolWord("faux", false),
                new BoolWord("oui", true),
                new BoolWord("non", false),
                new BoolWord("sur", true),
                new BoolWord("de", false),
            });
            options.NonZeroNumbersAreTrue = false;

            IniFile file = new(null, options);
            // Write as string values
            foreach ((string Setting, string Word, bool Value, bool CanRead) in BoolOptionData)
                file.SetSetting(stringSection, Setting, Word);
            // Write as bool values
            foreach ((string Setting, string Word, bool Value, bool CanRead) in BoolOptionData)
                file.SetSetting(boolSection, Setting, Value);
            byte[] buffer = SaveToBytes(file);

            file.Clear();
            Assert.AreEqual(0, file.GetSections().Count());
            LoadFromBytes(file, buffer);

            foreach ((string Setting, string Word, bool Value, bool CanRead) in BoolOptionData)
            {
                bool result = file.GetSetting(stringSection, Setting, !Value);
                if (CanRead)
                    Assert.AreEqual(Value, result);
                else
                    Assert.AreEqual(!Value, result);
            }

            foreach ((string Setting, string Word, bool Value, bool CanRead) in BoolOptionData)
            {
                bool result = file.GetSetting(boolSection, Setting, !Value);
                Assert.AreEqual(Value, result);
            }
        }

        [TestMethod]
        public void TestComments()
        {
            IniFile file = new();

            file.Comments.Add("Abc");
            file.Comments.Add("123");
            file.Comments.Add("");
            file.Comments.Add(null);

            file.SetSetting(IniFile.DefaultSectionName, "Test", "Abc");
            file.SetSetting(IniFile.DefaultSectionName, "Test2", "123");
            byte[] buffer = SaveToBytes(file);

            file.Clear();
            LoadFromBytes(file, buffer);

            Assert.AreEqual(@$"{file.CommentCharacter}Abc
{file.CommentCharacter}123
{file.CommentCharacter}
{file.CommentCharacter}

[{IniFile.DefaultSectionName}]
Test=Abc
Test2=123
", Encoding.UTF8.GetString(buffer));

            Assert.AreEqual(4, file.Comments.Count);
            Assert.AreEqual("Abc", file.Comments[0]);
            Assert.AreEqual("123", file.Comments[1]);
            Assert.AreEqual("", file.Comments[2]);
            Assert.AreEqual("", file.Comments[3]);

            Assert.AreEqual("Abc", file.GetSetting(IniFile.DefaultSectionName, "Test"));
            Assert.AreEqual("123", file.GetSetting(IniFile.DefaultSectionName, "Test2"));
        }

        private static byte[] SaveToBytes(IniFile file)
        {
            using MemoryStream stream = new();
            using StreamWriter writer = new(stream);
            file.Save(writer);
            writer.Flush();
            return stream.ToArray();
        }

        private static byte[] SaveToBytes(string contents)
        {
            using MemoryStream stream = new();
            using StreamWriter writer = new(stream);
            writer.Write(contents);
            writer.Flush();
            return stream.ToArray();
        }

        private static void LoadFromBytes(IniFile file, byte[] buffer)
        {
            using MemoryStream stream = new(buffer);
            using StreamReader reader = new(stream);
            file.Load(reader);
        }
    }
}
