namespace ProiectTSSApplication
{
    public class Program
    {
        static void Main(string[] args)
        {

        }

        /// <summary>
        /// Parses the DER-encoded length field from the provided buffer, advancing the position accordingly.
        /// Handles both short form (1 byte) and long form (multiple bytes) lengths as defined by ASN.1 DER rules.
        /// </summary>
        /// <param name="buffer">The byte span containing DER-encoded data.</param>
        /// <param name="position">A reference to the current read position in the buffer. It will be updated after parsing.</param>
        /// <returns>The length of the subsequent value in bytes.</returns>
        /// <exception cref="Exception">
        /// Thrown if the length is encoded using an indefinite form (not allowed in DER),
        /// or if there is insufficient data in the buffer to read the full length
        /// or if invalid position in the buffer
        /// </exception>
        public long GetDataLength(byte firstByte, ReadOnlySpan<byte> buffer, ref int position)
        {
            // Validate parameter position
            if (position < 0 || (position == buffer.Length && buffer.Length != 0) || position > buffer.Length)
            {
                throw new Exception("Invalid position");
            }

            // Check if single byte length 
            if ((firstByte & 0x80) == 0)
            {
                return firstByte;
            }

            // Get the number of bytes that compose the length
            int numBytes = firstByte & 0x7F;
            if (numBytes == 0)
            {
                throw new Exception("Indefinite length not supported in DER.");
            }

            // Add each byte to the length
            long length = 0;
            for (int i = 0; i < numBytes; i++)
            {
                if (position < buffer.Length)
                {
                    length = (length << 8) | buffer[position++];
                }
                else
                {
                    throw new Exception("Unexpected end of data");
                }
            }

            return length;
        }
    }
}
