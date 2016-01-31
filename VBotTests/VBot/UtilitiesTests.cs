using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SteamBot.VBot.Tests
{
    [TestClass()]
    public class UtilitiesTests
    {
        [TestMethod()]
        public void GivenANormalMessage_AndAnEmptyListOfMatches_ItCannotStartWithAnyOfThoseMatches()
        {
            Assert.IsFalse(new Utilities().DoesMessageStartWith("this is my message", new string[] { }));
        }

        [TestMethod()]
        public void GivenANormalMessage_AndAnListOfMatchesThatItDoesNotStartWith_ItCannotStartWithAnyOfThoseMatches()
        {
            Assert.IsFalse(new Utilities().DoesMessageStartWith("this is my message", new string[] { "a", "list", "of", "words" }));
        }

        [TestMethod()]
        public void GivenANormalMessage_AndASingleMessageThatItMatches_ItDoesStartWithThatMatch()
        {
            Assert.IsTrue(new Utilities().DoesMessageStartWith("this is my message", new string[] { "this" }));
        }

        [TestMethod()]
        public void GivenANormalMessage_AndSomeWordsOfWhichAtLeastOneMatches_ItDoesStartWithThatMatch()
        {
            Assert.IsTrue(new Utilities().DoesMessageStartWith("this is my message", new string[] { "some", "words", "this" }));
        }

        [TestMethod()]
        public void GivenAMessageThatStartsWithACommand_HoweverTheresNoSpace_ItShouldNotMatch()
        {
            Assert.IsFalse(new Utilities().DoesMessageStartWith("!addreply something", new string[] { "!add" }));
        }
    }
}