using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProiectTSSApplication;


namespace ProiectTSSTests
{
    public class DerAITests
    {
        private readonly Program _parser = new Program();

        [Fact]
        public void ShortForm_LengthBelow128_ReturnsValue()
        {
            byte firstByte = 0x4F; // 79 in short form
            var buffer = new byte[] { };
            int pos = 0;

            long result = _parser.GetDataLength(firstByte, buffer, ref pos);

            Assert.Equal(0x4F, result);
            Assert.Equal(0, pos);
        }

        [Fact]
        public void LongForm_SingleByteLength_ReturnsCorrectValue()
        {
            byte firstByte = 0x81; // Indicates 1 byte follows
            var buffer = new byte[] { 0x20 }; // 32
            int pos = 0;

            long result = _parser.GetDataLength(firstByte, buffer, ref pos);

            Assert.Equal(32, result);
            Assert.Equal(1, pos);
        }

        [Fact]
        public void LongForm_TwoByteLength_ReturnsCorrectValue()
        {
            byte firstByte = 0x82;
            var buffer = new byte[] { 0x01, 0x00 }; // 256
            int pos = 0;

            long result = _parser.GetDataLength(firstByte, buffer, ref pos);

            Assert.Equal(256, result);
            Assert.Equal(2, pos);
        }

        [Fact]
        public void InvalidPosition_ThrowsException()
        {
            byte firstByte = 0x81;
            var buffer = new byte[] { 0x01 };
            int pos = -1;

            var ex = Assert.Throws<Exception>(() => _parser.GetDataLength(firstByte, buffer, ref pos));
            Assert.Equal("Invalid position", ex.Message);
        }

        [Fact]
        public void IndefiniteLength_ThrowsException()
        {
            byte firstByte = 0x80;
            var buffer = new byte[] { };
            int pos = 0;

            var ex = Assert.Throws<Exception>(() => _parser.GetDataLength(firstByte, buffer, ref pos));
            Assert.Contains("Indefinite length not supported", ex.Message);
        }

        [Fact]
        public void LongForm_BufferTooShort_ThrowsException()
        {
            byte firstByte = 0x83;
            var buffer = new byte[] { 0x01 }; // Needs 3 bytes
            int pos = 0;

            var ex = Assert.Throws<Exception>(() => _parser.GetDataLength(firstByte, buffer, ref pos));
            Assert.Equal("Unexpected end of data", ex.Message);
        }

        [Fact]
        public void LongForm_MaxLengthValue_ParsesCorrectly()
        {
            byte firstByte = 0x84;
            var buffer = new byte[] { 0x00, 0x01, 0x00, 0x00 }; // 65536
            int pos = 0;

            long result = _parser.GetDataLength(firstByte, buffer, ref pos);

            Assert.Equal(65536, result);
            Assert.Equal(4, pos);
        }
    }
}
