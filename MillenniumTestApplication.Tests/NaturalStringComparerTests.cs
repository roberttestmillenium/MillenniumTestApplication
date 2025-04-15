using MillenniumTestApplication.Shared.Comparers;

namespace MillenniumTestApplication.Tests
{
    public class NaturalStringComparerTests
    {
        [Theory]
        [InlineData("ACTION2", "ACTION10", -1)]
        [InlineData("ACTION10", "ACTION2", 1)]
        [InlineData("ACTION5", "ACTION5", 0)]
        public void Compare_ShouldOrderNaturally(string a, string b, int expected)
        {
            // Arrange
            var comparer = new NaturalStringComparer();

            // Act
            var result = comparer.Compare(a, b);

            // Assert
            Assert.Equal(expected, Math.Sign(result));
        }
    }
}
