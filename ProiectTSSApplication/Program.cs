namespace ProiectTSSApplication
{
    public enum TagClass
    {
        Universal = 0x00,
        Application = 0x01,
        ContextSpecific = 0x02,
        Private = 0x03
    }

    public enum TagType
    {
        Primitive = 0x00,
        Constructed = 0x01
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        public void GetTagInformation(ReadOnlySpan<byte> buffer, ref int position, out string tagInfo)
        {
            ParseTagHeader(buffer, ref position, out TagClass tagClass, out TagType tagType, out byte tagNumber, out int dataLen);

            string tagTypeStr = tagType == TagType.Constructed ? "constructed" : "primitive";

            switch (tagClass)
            {
                case TagClass.Universal:
                    // TODO: parsing for some common universal types: STRUCTURE, INTEGER, BITFIELD, OBJECT
                    break;
                case TagClass.Application:
                    tagInfo = $"Application {tagTypeStr} tag: [{tagNumber}], with data length {dataLen}";
                    break;
                case TagClass.ContextSpecific:
                    tagInfo = $"Context specific {tagTypeStr} tag: [{tagNumber}], with data length {dataLen}";
                    break;
                case TagClass.Private:
                    tagInfo = $"Private {tagTypeStr} tag: {tagNumber}, with data length {dataLen}";
                    break;
            }
        }

        public string ParseObjectData(ReadOnlySpan<byte> buffer, ref int position)
        {
            // To be implemented
        }

        /// <summary>
        /// Parses the DER tag header from the given buffer and extracts the tag class, tag type, tag number, and the data length.
        /// </summary>
        /// <param name="buffer">
        /// A read-only span containing the DER-encoded data, starting from the position of the tag header.
        /// This buffer should contain the tag byte and the length of the associated data.
        /// </param>
        /// <param name="position">
        /// A reference to the current position in the buffer, starting from the position of the tag header.
        /// It is updated during the execution of the method to reflect the position after the tag byte and
        /// length bytes are processed.
        /// </param>
        /// <param name="tagClass">
        /// An output parameter that will hold the tag's class. This will be set to one of the following values from the <see cref="TagClass"/> enum:
        /// <list type="bullet">
        /// <item><description><see cref="TagClass.Universal"/> = 0x00</description></item>
        /// <item><description><see cref="TagClass.Application"/> = 0x01</description></item>
        /// <item><description><see cref="TagClass.ContextSpecific"/> = 0x02</description></item>
        /// <item><description><see cref="TagClass.Private"/> = 0x03</description></item>
        /// </list>
        /// </param>
        /// <param name="tagType">
        /// An output parameter that will hold the tag's type. This will be set to one of the following values from the <see cref="TagType"/> enum:
        /// <list type="bullet">
        /// <item><description><see cref="TagType.Primitive"/> = 0x00</description></item>
        /// <item><description><see cref="TagType.Constructed"/> = 0x01</description></item>
        /// </list>
        /// </param>
        /// <param name="tagNumber">
        /// An output parameter that will hold the tag number. This will be a value between 0 and 31, unless high-tag-number form is used.
        /// In case of a high-tag-number, an exception is thrown.
        /// </param>
        /// <param name="dataLen">
        /// An output parameter that will hold the data length (in bytes) for the encoded data following the tag. 
        /// This value is calculated using the <see cref="GetDataLength"/> function.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown if the tag number is 31 (0x1F), indicating the use of high-tag-number form, which is unsupported
        /// or if there is insufficient data in the buffer to read the full length.
        /// </exception>
        /// <remarks>
        /// This method extracts information about the tag class, tag type, and tag number from the first byte of the tag header, 
        /// and calculates the length of the encoded data using the following bytes. 
        /// It ensures that the tag number is between 0 and 31, throwing an exception if the high-tag-number form is detected.
        /// </remarks>
        public void ParseTagHeader(ReadOnlySpan<byte> buffer, ref int position, out TagClass tagClass, out TagType tagType, out byte tagNumber, out int dataLen)
        {
            // Read first byte (tag)
            byte tagByte = ReadByte(buffer, ref position);

            // Extract Tag Class (bits 8-7)
            tagClass = (TagClass)((tagByte >> 6) & 0x03);

            // Extract Tag Type (Primitive / Constructed bit 6)
            tagType = (TagType)((tagByte >> 5) & 0x01);

            // Extract Tag Number (bits 5-1)
            tagNumber = (byte)(tagByte & 0x1F);

            // High-tag-number form not supported
            if (tagNumber == 0x1F)
            {
                throw new Exception("High-tag-number form not supported");
            }

            // Get the length of the encoded data
            dataLen = GetDataLength(buffer, ref position); 
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
        /// or if there is insufficient data in the buffer to read the full length.
        /// </exception>
        public int GetDataLength(ReadOnlySpan<byte> buffer, ref int position)
        {
            byte firstByte = ReadByte(buffer, ref position);

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
            int length = 0;
            for (int i = 0; i < numBytes; i++)
            {
                length = (length << 8) | ReadByte(buffer, ref position);
            }
            return length;
        }

        /// <summary>
        /// Reads a single byte from the buffer at the specified position and advances the position.
        /// </summary>
        /// <param name="buffer">The byte span containing DER-encoded data.</param>
        /// <param name="position">A reference to the current read position in the buffer. It will be incremented by one.</param>
        /// <returns>The byte at the current position.</returns>
        /// <exception cref="Exception">Thrown if the position is beyond the end of the buffer.</exception>
        public byte ReadByte(ReadOnlySpan<byte> buffer, ref int position)
        {
            if (buffer.Length < position)
            {
                return buffer[position++];
            }

            throw new Exception("Unexpected end of data");
        }
    }
}
