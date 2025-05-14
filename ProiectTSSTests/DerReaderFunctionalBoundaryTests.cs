using System;
using Xunit;
using ProiectTSSApplication;

namespace ProiectTSSTests
{
    public class DerReaderFunctionalBoundaryTests
    {
        [Theory]
        [InlineData(0x00, new byte[] { 0x00 }, 0)]
        [InlineData(0x05, new byte[] { 0x05 }, 5)]
        [InlineData(0x7F, new byte[] { 0x7F }, 127)]
        public void GetDataLength_EC1(byte first_byte, byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory]
        // 0x10 = 16
        [InlineData(0x81, new byte[] { 0x10 }, 16)]
        public void GetDataLength_EC2(byte first_byte, byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory]
        // 0x01F4 = 500
        // 0x001000 = 4096
        [InlineData(0x82, new byte[] { 0x01, 0xF4 }, 500)]          
        [InlineData(0x83, new byte[] { 0x00, 0x10, 0x00 }, 4096)]  
        public void GetDataLength_EC3(byte first_byte, byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        [Theory]
        // indefinite length
        [InlineData(0x80, new byte[] { })]
        public void GetDataLength_EC4(byte first_byte, byte[] buffer)
        {
            var parser = new Program();
            int position = 0;

            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        // expected 2 bytes, just one available
        [Theory]
        [InlineData(0x82, new byte[] { 0x01 })]  
        public void GetDataLength_EC5(byte first_byte, byte[] buffer)
        {
            var parser = new Program();
            int position = 0;

            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Unexpected end of data", ex.Message);
        }

        // very big length
        [Theory]
        [InlineData(0x87, new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }, 16777216)]
        public void GetDataLength_EC6(byte first_byte, byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }

        // length is 0
        [Theory]
        [InlineData(0x00, new byte[] { }, 0)]
        public void GetDataLength_EC7(byte first_byte, byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long result = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, result);
        }
    }
}
