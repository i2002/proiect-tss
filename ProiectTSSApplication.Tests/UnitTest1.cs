using System;
using Xunit;
using ProiectTSSApplication;

namespace ProiectTSSApplication.Tests
{
    public class DerLengthParserEquivalenceClasses
    {
        [Theory(DisplayName = "C_1_1 - Short form length = 0")]
        [InlineData(new byte[] { 0x00 }, 0)]
        public void C_1_1(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory(DisplayName = "C_1_2 - Short form length = 22")]
        [InlineData(new byte[] { 0x16 }, 22)]  // 00010110 = 22
        public void C_1_2(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory(DisplayName = "C_2_1 - Indefinite length (not supported)")]
        [InlineData(new byte[] { 0x80 })] // 10000000 = indefinite form
        public void C_2_1(byte[] buffer)
        {
            var parser = new Program();
            int position = 0;
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(buffer, ref position));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        [Theory(DisplayName = "C_2_2_1 - Long form, length bytes too few")]
        [InlineData(new byte[] { 0x86, 0xFF, 0x00 })] // length declared on 6 bytes, but only 2 given
        public void C_2_2_1(byte[] buffer)
        {
            var parser = new Program();
            int position = 0;
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(buffer, ref position));
            Assert.Contains("Unexpected end of data", ex.Message);
        }

        [Theory(DisplayName = "C_2_2_2 - Long form, valid 3-byte length")]
        [InlineData(new byte[] { 0x83, 0xA1, 0xB2, 0xC3 }, 0xA1B2C3)] // 10000011 = length uses 3 bytes
        public void C_2_2_2(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }
    }
}