# Testing a C# Application Using xUnit

The purpose of this project is testing a C# application using one of the available tools, in order to get familiar with the basic testing concepts, such as unit testing, mutation testing and more. In order to get started, a unit testing tool needs to be decided upon. Since there are multiple tools to choose from, the three best options available are the following: xUnit, NUnit and MSTest. A better understanding of each of them is required before deciding on the one that shall be used further in our project.

By consulting [medium.com](https://medium.com/@robertdennyson/xunit-vs-nunit-vs-mstest-choosing-the-right-testing-framework-for-net-applications-b6b9b750bec6), these tables come across as relevant.  

## Framework Comparison

| Framework | Description |
|----------|-------------|
| **xUnit** | A modern unit testing framework for .NET applications, designed with extensibility, dependency injection, and best practices in mind. It's the default choice for many .NET Core projects. |
| **NUnit** | One of the oldest and most widely-used unit testing frameworks in the .NET ecosystem, known for its extensive attributes and compatibility with a wide range of tools. |
| **MSTest** | Microsoft’s built-in testing framework, well-integrated with Visual Studio. It is often the default for legacy .NET projects and provides solid integration with Microsoft tools. |

## Comparison of Key Features

| Feature | xUnit | NUnit | MSTest |
|--------|-------|-------|--------|
| **Ease of Setup** | High | High | Moderate |
| **Attributes** | Minimal | Rich set | Limited |
| **Parallel Test Execution** | Built-in | Built-in | Limited |
| **Dependency Injection** | Best support | Moderate | Limited |
| **IDE Integration** | Full support in VS/Rider | Full support in VS/Rider | Best with Visual Studio |
| **Mock Framework Support** | Full | Full | Full |
| **BDD Support** | Compatible | Compatible | Limited |


xUnit is a newer technology, being the new standard when it comes to application testing, easily integrable in Visual Studio. It seems to be useful for cross-platform projects and for those including dependency injection. NUnit seems to be appropriate for large, extendable projects, since it provides a wide range of test attributes. MSTest appears to be the legacy model used in .NET testing, being mostly used in older applications, often being integrated into Visual Studio.

For the purpose of this project, we will integrate xUnit into Visual Studio in order to get familiar with testing, as well as being able to keep in touch with the continuously evolving standards. We chose xUnit since it is modern, easy to use, works well with today’s .NET tools, and makes writing and running tests simple, especially when using features like dependency injection and cross-platform support.


Other websites that were used for our documentation are the following:
- https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-csharp-with-xunit
- https://learn.microsoft.com/en-us/dotnet/core/testing/
- https://xunit.net/
- https://www.browserstack.com/guide/nunit-vs-xunit-vs-mstest
- https://www.headspin.io/blog/nunit-vs-xunit-vs-mstest
- https://medium.com/@robertdennyson/xunit-vs-nunit-vs-mstest-choosing-the-right-testing-framework-for-net-applications-b6b9b750bec6
- https://daily.dev/blog/nunit-vs-xunit-vs-mstest-net-unit-testing-framework-comparison

## Writing equivalence classes  

First of all, we shall understand the use and need of equivalence classes. In testing, equivalence classes represent partitioning the inputs into groups also known as classes, where we assume the method would have a similar behaviour for every input group. For the purpose of this project, we will consider the GetDataLength() method and we will write the equivalence classes for it.  

For this method, the input is a but sequence starting at some position, The behaviour depends on how the first byte looks, especially the highest bit. Let's have a look at the method: 

```
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

```

Let's analyse the GetDataLength() method. It is responsible for parsing the length field of a DER-encoded  ( Distinguished Encoding Rules) data atructure from  given byte buffer. This method interprets the Length field according to DER rules and determines how many bytes of value data will come after the length. It also handles errors, throwing an exception if an indefinite length is encountered, since DER forbids it. Furthermore, it also covers the case when there isn't enough data left in the buffer to read the declared length bytes. Let's see how different input data will behave. In order to make the information more accessible, we will present it into a table, in order to be easier to read.

| **ID** | **Class Description** | **Input Condition** | **Representative Example(s)** | **Expected Behavior** | **Justification** | **Explanation** |
|:------|:-----------------------|:--------------------|:-------------------------------|:----------------------|:------------------|:----------------|
| EC1 | **Short form length** | First byte `< 0x80` (highest bit 0) | `[0x00]`, `[0x05]`, `[0x7F]` | Return the first byte value directly (the first if condition returns true). | Short form is common for small data (0–127 bytes) and must be handled correctly. | No extra bytes are read. Length fits in the first byte. |
| EC2 | **Long form length (1 byte)** | First byte `0x81`, and next 1 byte exists | `[0x81, 0x10]` (length = 16) | Read 1 next byte and return as the length. | Needed to support data >127 bytes, requires reading extra byte correctly. | MSB is 1, low bits = 1. requires one byte of actual length value. |
| EC3 | **Long form length (2+ bytes)** | First byte `0x82` to `0x8F`, and corresponding bytes exist | `[0x82, 0x01, 0xF4]` (length = 500), `[0x83, 0x00, 0x10, 0x00]` (length = 4096) | Read N next bytes, combine big endian to get length. | Critical for handling larger structures like certificates, blobs etc. | Multiple bytes are combined left to right (big endian) to form large length. |
| EC4 | **Indefinite length (forbidden)** | First byte = `0x80` | `[0x80]` | Throw `Exception: "Indefinite length not supported"` | DER strictly forbids indefinite length, must reject it immediately. | Indefinite lengths are BER feature, DER must have explicit, finite lengths. |
| EC5 | **Insufficient buffer for long form** | Long form detected but insufficient subsequent bytes | `[0x82, 0x01]` (expects 2 bytes but only 1 present) | Throw `Exception: "Unexpected end of data"` | Prevents reading out of bounds, critical for memory safety. | Detects buffer underflow conditions when claimed length bytes are missing. |
| EC6 (optional) | **Oversized long form** | Long form requiring many bytes (`0x87` etc.) | `[0x87, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07]` | Read many bytes and compute large length. | Tests resilience against absurdly large but valid inputs. | Defends against potential denial-of-service or memory abuse attacks. |
| EC7 (optional) | **Zero-length short form** | First byte = `0x00` | `[0x00]` | Return 0 (no value data follows) | Some objects (for example, NULL) legally have zero length. This case still must must be handled. | No error: a valid case where the encoded object has no value content. |
