using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamBot.VBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamBot.VBot.Tests
{
    [TestClass()]
    public class UtilitiesTests
    {
        [TestMethod()]
        public void DoesMessageStartWithTest()
        {
            Assert.IsFalse(new Utilities().DoesMessageStartWith("this is my message", new string[] { }));
            Assert.IsFalse(new Utilities().DoesMessageStartWith("this is my message", new string[] { "a", "list", "of", "words" }));
            Assert.IsTrue(new Utilities().DoesMessageStartWith("this is my message", new string[] { "this" }));
            Assert.IsTrue(new Utilities().DoesMessageStartWith("this is my message", new string[] { "some", "words", "this" }));
        }
    }
}