using Xunit;

namespace Wordle.Tests
{
    public class WordListTests
    {
        [InlineData("aaaaa")]
        [InlineData("abcde")]
        [InlineData("train")]
        [Theory]
        public void WordRoundTrip(string word)
        {
            var l = WordList.ToLong(word);
            var o = WordList.FromLong(l);

            Assert.Equal(word, o);
        }
    }
}