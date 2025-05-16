using System;
using Xunit;
using ProiectTSSApplication;

namespace ProiectTSSTests
{
    public class DerLengthParserEquivalenceClasses
    {
        [Theory(DisplayName = "C_1 - Short form length")]
        [InlineData(0x02, new byte[] { }, 0, 2)]
        public void C_1(byte first_byte, byte[] buffer, int position, int expectedLength)
        {
            var parser = new Program();
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory(DisplayName = "C_2 - Indefinite length (not supported)")]
        [InlineData(0x80, new byte[] { }, 0)] // 10000000 = indefinite form
        public void C_2_1(byte first_byte, byte[] buffer, int position)
        {
            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        [Theory(DisplayName = "C_3_1 - Long form, Invalid position")]
        [InlineData(0x83, new byte[] { 0x23, 0x44}, -2)]
        public void C_3_1(byte first_byte, byte[] buffer, int position)
        {
            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }

        [Theory(DisplayName = "C_3_2 - Long form, Invalid position")]
        [InlineData(0x84, new byte[] { 0x24, 0x11}, 3)]
        public void C_3_2(byte first_byte, byte[] buffer, int position)
        {
            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }

        [Theory(DisplayName = "C_3_3_1 - Long form, valid 3-byte length")]
        [InlineData(0x83, new byte[] { 0xA1, 0xB2, 0xC3 }, 0, 0xA1B2C3)] // 10000011 = length uses 3 bytes
        public void C_3_3_1(byte first_byte, byte[] buffer, int position, long expectedLength)
        {
            var parser = new Program();
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory(DisplayName = "C_3_3_2 - Long form, length bytes too few")]
        [InlineData(0x86, new byte[] { 0xFF, 0x00 }, 0)] // length declared on 6 bytes, but only 2 given
        public void C_3_3_2(byte first_byte, byte[] buffer, int position)
        {
            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Unexpected end of data", ex.Message);
        }
    }
}