using System;
using Xunit;
using ProiectTSSApplication;

namespace ProiectTSSTests
{
    public class DerLengthParserBoundaryValueAnalysis
    {
        [Theory(DisplayName = "C_1_1 - Short form length = 0")]
        [InlineData(0x00, new byte[] { }, 0)]
        public void C_1_1(byte first_byte, byte[] buffer, long expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_1_2 - Short form length = 1")]
        [InlineData(0x01, new byte[] { }, 1)]
        public void C_1_2(byte first_byte, byte[] buffer, long expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_1_3 - Short form length = 127")]
        [InlineData(0x7F, new byte[] { }, 127)]
        public void C_1_3(byte first_byte, byte[] buffer, long expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_1 - Long form, length byte = 0 (invalid)")]
        [InlineData(0x80, new byte[] { })]
        public void C_2_1(byte first_byte, byte[] buffer)
        {
            var parser = new Program();
            int position = 0;
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        [Theory(DisplayName = "C_2_2_1 - Long form, 1 byte declared, none provided")]
        [InlineData(0x81, new byte[] { })]
        public void C_2_2_1(byte first_byte, byte[] buffer)
        {
            var parser = new Program();
            int position = 0;
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }

        [Theory(DisplayName = "C_2_2_2 - Long form, 1 byte value = 0xF0 (240)")]
        [InlineData(0x81, new byte[] { 0xF0 }, 240)]
        public void C_2_2_2(byte first_byte, byte[] buffer, long expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_2_3 - Declares 1-byte length 160, 127 bytes follow")]
        [InlineData(0x81, new byte[] { 0xA0, /* 127 x A0 */ }, 160)]
        public void C_2_2_3(byte first_byte, byte[] bufferPrefix, long expectedLength)
        {
            byte[] buffer = new byte[1 + 127];
            buffer[0] = bufferPrefix[0];

            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = 0xA0;
            }

            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_2_4 - Declares 1-byte length 160, 128 bytes follow")]
        [InlineData(0x81, new byte[] { 0xA0 }, 160)]
        public void C_2_2_4(byte first_byte, byte[] bufferPrefix, long expectedLength)
        {
            byte[] buffer = new byte[1 + 128];

            buffer[0] = bufferPrefix[0];
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = 0xA0;
            }

            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_1 - Declares 127, but no bytes follow")]
        [InlineData(0x81, new byte[] { 0x7F }, 127)]
        public void C_2_3_1(byte first_byte, byte[] buffer, long expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_2 - Declares 127, only 1 byte follows")]
        [InlineData(0x81, new byte[] { 0x7F, 0xF0 }, 127)]
        public void C_2_3_2(byte first_byte, byte[] buffer, long expectedLength)
        {
            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_3 - Declares 127, 127 bytes follow")]
        [InlineData(0x81, new byte[] { 0x7F }, 127)]
        public void C_2_3_3(byte first_byte, byte[] bufferPrefix, long expectedLength)
        {
            byte[] buffer = new byte[1 + 127];

            buffer[0] = bufferPrefix[0];
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = 0xA0;
            }

            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_3_4 - Declares 127, 128 bytes follow")]
        [InlineData(0x81, new byte[] { 0x7F }, 127)]
        public void C_2_3_4(byte first_byte, byte[] bufferPrefix, long expectedLength)
        {
            byte[] buffer = new byte[1 + 128];

            buffer[0] = bufferPrefix[0];
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = 0xA0;
            }

            var parser = new Program();
            int position = 0;
            long length = parser.GetDataLength(first_byte, buffer, ref position);
            Assert.Equal(expectedLength, length);
        }

        [Theory(DisplayName = "C_2_4_1 - Long form, 1 byte value = 0xF0 (240), negative position")]
        [InlineData(0x81, new byte[] { 0xF0 }, -1)]
        public void C_2_4_1(byte first_byte, byte[] buffer, int position)
        {
            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }

        [Theory(DisplayName = "C_2_4_2 - Long form, 1 byte value = 0xF0 (240), position outside buffer")]
        [InlineData(0x82, new byte[] { 0xF0 }, 1)]
        public void C_2_4_2(byte first_byte, byte[] buffer, int position)
        {
            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }

        [Theory(DisplayName = "C_2_3_4 - Declares 127, 127 bytes follow, negative position")]
        [InlineData(0xFF, new byte[] { 0x7F }, -1)]
        public void C_2_4_3(byte first_byte, byte[] bufferPrefix, int position)
        {
            byte[] buffer = new byte[127];

            buffer[0] = bufferPrefix[0];
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = 0xA0;
            }

            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }

        [Theory(DisplayName = "C_2_4_4 - Declares 127, 127 bytes follow, position outside buffer")]
        [InlineData(0xFF, new byte[] { 0x7F }, 127)]
        public void C_2_4_4(byte first_byte, byte[] bufferPrefix, int position)
        {
            byte[] buffer = new byte[127];

            buffer[0] = bufferPrefix[0];
            for (int i = 1; i < buffer.Length; i++)
            {
                buffer[i] = 0xA0;
            }

            var parser = new Program();
            var ex = Assert.Throws<Exception>(() => parser.GetDataLength(first_byte, buffer, ref position));
            Assert.Contains("Invalid position", ex.Message);
        }
    }
}