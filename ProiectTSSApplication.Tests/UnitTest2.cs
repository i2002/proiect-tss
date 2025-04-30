using System;
using Xunit;
using ProiectTSSApplication;

namespace ProiectTSSApplication.Tests
{
    public class DerLengthParserBoundaryValueAnalysis
    {
        [Theory(DisplayName = "C_1_1 - Short form length = 0")]
        [InlineData(new byte[] { 0x00 }, 0)]
        public void C_1_1(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_1_2 - Short form length = 1")]
        [InlineData(new byte[] { 0x01 }, 1)]
        public void C_1_2(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_1_3 - Short form length = 127")]
        [InlineData(new byte[] { 0x7F }, 127)]
        public void C_1_3(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_1 - Long form, length byte = 0 (invalid)")]
        [InlineData(new byte[] { 0x80 })]
        public void C_2_1(byte[] buffer)
        {
            var parser = new Program();
            int position = 0;
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(buffer, ref position));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        [Theory(DisplayName = "C_2_2_1 - Long form, 1 byte declared, none provided")]
        [InlineData(new byte[] { 0x81 })]
        public void C_2_2_1(byte[] buffer)
        {
            var parser = new Program();
            int position = 0;
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(buffer, ref position));
            Assert.Contains("Unexpected end of data", ex.Message);
        }

        [Theory(DisplayName = "C_2_2_2 - Long form, 1 byte value = 0xF0 (240)")]
        [InlineData(new byte[] { 0x81, 0xF0 }, 240)]
        public void C_2_2_2(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_2_3 - Declares 1-byte length 160, 127 bytes follow")]
        [InlineData(new byte[] { 0x81, 0xA0, /* 127 x A0 */ }, 160)]
        public void C_2_2_3(byte[] bufferPrefix, int expectedLength)
        {
            byte[] buffer = new byte[2 + 127];
            buffer[0] = bufferPrefix[0];
            buffer[1] = bufferPrefix[1];
            for (int i = 2; i < buffer.Length; i++) buffer[i] = 0xA0;

            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_2_4 - Declares 1-byte length 160, 128 bytes follow")]
        [InlineData(new byte[] { 0x81, 0xA0 }, 160)]
        public void C_2_2_4(byte[] bufferPrefix, int expectedLength)
        {
            byte[] buffer = new byte[2 + 128];
            buffer[0] = bufferPrefix[0];
            buffer[1] = bufferPrefix[1];
            for (int i = 2; i < buffer.Length; i++) buffer[i] = 0xA0;

            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_1 - Declares 127, but no bytes follow")]
        [InlineData(new byte[] { 0x81, 0x7F }, 127)]
        public void C_2_3_1(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_2 - Declares 127, only 1 byte follows")]
        [InlineData(new byte[] { 0x81, 0x7F, 0xF0 }, 127)]
        public void C_2_3_2(byte[] buffer, int expectedLength)
        {
            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_3 - Declares 127, 127 bytes follow")]
        [InlineData(new byte[] { 0x81, 0x7F }, 127)]
        public void C_2_3_3(byte[] bufferPrefix, int expectedLength)
        {
            byte[] buffer = new byte[2 + 127];
            buffer[0] = bufferPrefix[0];
            buffer[1] = bufferPrefix[1];
            for (int i = 2; i < buffer.Length; i++) buffer[i] = 0xA0;

            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_4 - Declares 127, 128 bytes follow")]
        [InlineData(new byte[] { 0x81, 0x7F }, 127)]
        public void C_2_3_4(byte[] bufferPrefix, int expectedLength)
        {
            byte[] buffer = new byte[2 + 128];
            buffer[0] = bufferPrefix[0];
            buffer[1] = bufferPrefix[1];
            for (int i = 2; i < buffer.Length; i++) buffer[i] = 0xA0;

            var parser = new Program();
            int position = 0;
            int length = parser.GetDataLength(buffer, ref position);
            Assert.Equal(expectedLength, length);
        }
    }
}