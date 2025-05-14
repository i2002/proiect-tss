using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ProiectTSSApplication;

namespace ProiectTSSTests
{
    public class DerReaderMutationTests
    {
        private readonly Program _reader = new Program();

        [Fact]
        public void GetDataLength_InsufficientData_ThrowsException()
        {
            byte first_byte = 0x82;
            var buffer = new byte[] { 0x01 }; // Declares 2 bytes, provides only 1
            int pos = 0;

            Assert.Throws<Exception>(() => _reader.GetDataLength(first_byte, buffer, ref pos));

        }

        [Theory]
        [InlineData(0x00, new byte[] { }, 0, 0)] // Valid zero length
        public void GetDataLength_ZeroLength_IsAccepted(byte first_byte, byte[] buffer, int startPos, int expectedLength)
        {
            int pos = startPos;
            var span = new ReadOnlySpan<byte>(buffer);
            long length = _reader.GetDataLength(first_byte, span, ref pos);

            Assert.Equal(expectedLength, length);
            Assert.Equal(startPos, pos);
        }

        [Fact]
        public void GetDataLength_BufferTooShort_Throws()
        {
            byte first_byte = 0x83;
            var buffer = new byte[] { 0x01 }; // Says 3 bytes, only gives 1
            int pos = 0;

            Assert.Throws<Exception>(() => _reader.GetDataLength(first_byte, buffer, ref pos));

        }

        [Fact]
        public void GetDataLength_OversizedLongForm_ParsesCorrectly()
        {
            byte first_byte = 0x83;
            var buffer = new byte[] { 0x00, 0x10, 0x00 }; // Length = 4096
            int pos = 0;
            long length = _reader.GetDataLength(first_byte, buffer, ref pos);

            Assert.Equal(4096, length);
            Assert.Equal(3, pos);
        }


        [Fact]
        public void GetDataLength_ZeroLength_ReturnsZero()
        {
            byte first_byte = 0x00;
            var buffer = new byte[] { }; // Short-form, zero-length
            int pos = 0;
            long length = _reader.GetDataLength(first_byte, buffer, ref pos);

            Assert.Equal(0, length);
        }


    }
}
