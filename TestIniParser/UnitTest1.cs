// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//

using IniFileParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace TestIniParser
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = Path.Combine(path, "TestIniParser");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, "Test.ini");

            IniFile file = new IniFile();
            file.SetSetting(IniFile.DefaultSectionName, "String", "abc");
            file.SetSetting(IniFile.DefaultSectionName, "Integer", 123);
            file.SetSetting(IniFile.DefaultSectionName, "Double", 123.45);
            file.SetSetting(IniFile.DefaultSectionName, "Boolean1", true);
            file.SetSetting(IniFile.DefaultSectionName, "Boolean2", false);
            file.Save(path);

            file.Clear();

            file.Load(path);
            Assert.AreEqual("abc", file.GetSetting(IniFile.DefaultSectionName, "String", string.Empty));
            Assert.AreEqual(123, file.GetSetting(IniFile.DefaultSectionName, "Integer", 0));
            Assert.AreEqual(123.45, file.GetSetting(IniFile.DefaultSectionName, "Double", 0.0));
            Assert.AreEqual(true, file.GetSetting(IniFile.DefaultSectionName, "Boolean1", false));
            Assert.AreEqual(false, file.GetSetting(IniFile.DefaultSectionName, "Boolean2", true));

            var settings = file.GetSectionSettings(IniFile.DefaultSectionName).ToArray();
            Assert.AreEqual(5, settings.Length);
            Assert.AreEqual("String", settings[0].Name);
            Assert.AreEqual("abc", settings[0].Value);
            Assert.AreEqual("Integer", settings[1].Name);
            Assert.AreEqual(123.ToString(), settings[1].Value);
            Assert.AreEqual("Double", settings[2].Name);
            Assert.AreEqual(123.45.ToString(), settings[2].Value);
            Assert.AreEqual("Boolean1", settings[3].Name);
            Assert.AreEqual(true.ToString(), settings[3].Value);
            Assert.AreEqual("Boolean2", settings[4].Name);
            Assert.AreEqual(false.ToString(), settings[4].Value);

            File.Delete(path);
        }
    }
}
