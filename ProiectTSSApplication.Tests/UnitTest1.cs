using System;
using Xunit;
using ProiectTSSApplication;

namespace ProiectTSSApplication.Tests
{
    public class DerLengthParserTests
    {
        [Theory]
        [InlineData(new byte[] { 0x00 }, 0)]
        [InlineData(new byte[] { 0x05 }, 5)]
        [InlineData(new byte[] { 0x7F }, 127)]
        [InlineData(new byte[] { 0x81, 0x10 }, 272)]

        public void GetDataLength_First_Bit_0(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory]
        // 0x10 = 16
        [InlineData(new byte[] { 0x81, 0x10 }, 16)]
        public void GetDataLength_EC2(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory]
        // 0x01F4 = 500
        // 0x001000 = 4096
        [InlineData(new byte[] { 0x82, 0x01, 0xF4 }, 500)]          
        [InlineData(new byte[] { 0x83, 0x00, 0x10, 0x00 }, 4096)]  
        public void GetDataLength_EC3(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory]
        // indefinite length
        [InlineData(new byte[] { 0x80 })]
        public void GetDataLength_EC4(byte[] buffer)
        {
            var parser = new Program();
            int position = 0;

            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(buffer, ref position));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        // expected 2 bytes, just one available
        [Theory]
        [InlineData(new byte[] { 0x82, 0x01 })]  
        public void GetDataLength_EC5(byte[] buffer)
        {
            var parser = new Program();
            int position = 0;

            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(buffer, ref position));
            Assert.Contains("Unexpected end of data", ex.Message);
        }

        // very big length
        [Theory]
        [InlineData(new byte[] { 0x87, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }, 16777216)]
        public void GetDataLength_EC6(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        // length is 0
        [Theory]
        [InlineData(new byte[] { 0x00 }, 0)]
        public void GetDataLength_EC7(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int result = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, result);
        }
    }
}
