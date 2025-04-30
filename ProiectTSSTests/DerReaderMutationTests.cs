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
            var buffer = new byte[] { 0x82, 0x01 }; // Declares 2 bytes, provides only 1
            int pos = 0;

            Assert.Throws<Exception>(() => _reader.GetDataLength(buffer, ref pos));

        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, 0, 0)] // Valid zero length
        public void GetDataLength_ZeroLength_IsAccepted(byte[] buffer, int startPos, int expectedLength)
        {
            int pos = startPos;
            var span = new ReadOnlySpan<byte>(buffer);
            int length = _reader.GetDataLength(span, ref pos);

            Assert.Equal(expectedLength, length);
            Assert.Equal(startPos + 1, pos);
        }

        [Fact]
        public void GetDataLength_BufferTooShort_Throws()
        {
            var buffer = new byte[] { 0x83, 0x01 }; // Says 3 bytes, only gives 1
            int pos = 0;

            Assert.Throws<Exception>(() => _reader.GetDataLength(buffer, ref pos));

        }

        [Fact]
        public void GetDataLength_OversizedLongForm_ParsesCorrectly()
        {
            var buffer = new byte[] { 0x83, 0x00, 0x10, 0x00 }; // Length = 4096
            int pos = 0;
            int length = _reader.GetDataLength(buffer, ref pos);

            Assert.Equal(4096, length);
            Assert.Equal(4, pos);
        }


        [Fact]
        public void GetDataLength_ZeroLength_ReturnsZero()
        {
            var buffer = new byte[] { 0x00 }; // Short-form, zero-length
            int pos = 0;
            int length = _reader.GetDataLength(buffer, ref pos);

            Assert.Equal(0, length);
        }


    }
}
