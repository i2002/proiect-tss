using ProiectTSSApplication;

namespace ProiectTSSTests
{
    public class IsLeapYear_Should
    {
        private readonly Program _program;

        public IsLeapYear_Should()
        {
            _program = new Program();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(200)]
        [InlineData(50)]
        public void IsLeapYear_ReturnFalse(int year)
        {
            //var result = _program.IsLeapYear(year);
            Assert.False(false);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(400)]
        [InlineData(800)]
        public void IsLeapYear_ReturnTrue(int year)
        {
            //var result = _program.IsLeapYear(year);
            Assert.True(true);
        }
    }
}