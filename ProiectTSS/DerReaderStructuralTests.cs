using ProiectTSSApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectTSSTests
{
	public class DerReaderStructuralTests
	{
		private readonly Program _reader = new Program();


		/// <summary>
		/// Statement & branch coverage: verifies single-byte length parsing when MSB is unset.
		/// Ensures that the method reads exactly one byte and returns its value.
		/// </summary>
		[Theory]
		[InlineData(new byte[] { 0x00, 0x00 }, 0, 0)] 
		[InlineData(new byte[] { 0x7F, 0x00 }, 0, 0x7F)] 
		public void GetDataLength_SingleByte_ReturnsFirstByte(byte[] buffer, int startPos, int expectedLength)
		{
			int pos = startPos;
			var span = new ReadOnlySpan<byte>(buffer);
			int length = _reader.GetDataLength(span, ref pos);

			Assert.Equal(expectedLength, length);
			Assert.Equal(startPos + 1, pos);
		}

		/// <summary>
		/// Branch & decision coverage: checks that when MSB is set but numBytes == 0,
		/// the method throws the expected exception for indefinite length.
		/// </summary>
		[Fact]
		public void GetDataLength_ZeroNumBytes_ThrowsException()
		{
			var buffer = new byte[] { 0x80 };
			int pos = 0;

			var ex = Assert.Throws<Exception>(() => _reader.GetDataLength(buffer, ref pos));
			Assert.Contains("Indefinite length not supported", ex.Message);
		}

		/// <summary>
		/// Multiple-condition & circuit coverage: verifies multi-byte length parsing,
		/// combining each subsequent byte correctly into the final length value.
		/// </summary>
		[Theory]
		[InlineData(new byte[] { 0x81, 0x05 }, 0, 0x05, 2)] 
		[InlineData(new byte[] { 0x82, 0x01, 0x02 }, 0, 0x0102, 3)]
		public void GetDataLength_MultiByte_ReturnsCombinedLength(byte[] buffer, int startPos, int expectedLength, int expectedPos)
		{
			int pos = startPos;
			int length = _reader.GetDataLength(buffer, ref pos);

			Assert.Equal(expectedLength, length);
			Assert.Equal(expectedPos, pos);
		} 
	}
}
