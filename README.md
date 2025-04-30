# Testing a C# Application Using xUnit

The purpose of this project is testing a C# application using one of the available tools, in order to get familiar with the basic testing concepts, such as unit testing, mutation testing and more. In order to get started, a unit testing tool needs to be decided upon. Since there are multiple tools to choose from, the three best options available are the following: xUnit, NUnit and MSTest. A better understanding of each of them is required before deciding on the one that shall be used further in our project.

By consulting [medium.com](https://medium.com/@robertdennyson/xunit-vs-nunit-vs-mstest-choosing-the-right-testing-framework-for-net-applications-b6b9b750bec6), these tables come across as relevant.

## Framework Comparison

| Framework  | Description                                                                                                                                                                                |
| ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **xUnit**  | A modern unit testing framework for .NET applications, designed with extensibility, dependency injection, and best practices in mind. It's the default choice for many .NET Core projects. |
| **NUnit**  | One of the oldest and most widely-used unit testing frameworks in the .NET ecosystem, known for its extensive attributes and compatibility with a wide range of tools.                     |
| **MSTest** | Microsoft’s built-in testing framework, well-integrated with Visual Studio. It is often the default for legacy .NET projects and provides solid integration with Microsoft tools.          |

## Comparison of Key Features

| Feature                     | xUnit                    | NUnit                    | MSTest                  |
| --------------------------- | ------------------------ | ------------------------ | ----------------------- |
| **Ease of Setup**           | High                     | High                     | Moderate                |
| **Attributes**              | Minimal                  | Rich set                 | Limited                 |
| **Parallel Test Execution** | Built-in                 | Built-in                 | Limited                 |
| **Dependency Injection**    | Best support             | Moderate                 | Limited                 |
| **IDE Integration**         | Full support in VS/Rider | Full support in VS/Rider | Best with Visual Studio |
| **Mock Framework Support**  | Full                     | Full                     | Full                    |
| **BDD Support**             | Compatible               | Compatible               | Limited                 |

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

## Structural testing

### Transforming the code into an oriented graph

![My diagram](./resources/oriented_graph_code.svg)

**Structural Explanation**

- **Statement coverage**  
  Every line in `GetDataLength` is executed by at least one test.

- **Branch coverage**  
  For `if ((firstByte & 0x80) == 0)` and for `if (numBytes == 0)` we have covered both the “true” and “false” branches.

- **Condition coverage & Condition/Decision coverage**  
  Each individual bitwise condition (`firstByte & 0x80`, `firstByte & 0x7F`) takes both `true` and `false` values, and each overall decision also evaluates to both outcomes.

- **Multiple condition coverage**  
  We test all combinations of MSB set/unset and `numBytes` values (0, 1, >1).

- **MC/DC Coverage**  
  The four required scenarios are covered by:

  - `GetDataLength_SingleByte`: `MSB=0`.
  - `GetDataLength_ZeroNumBytes`: `MSB=1 + numBytes=0`.
  - `GetDataLength_MultiByte` (with `numBytes=1`): `MSB=1 + numBytes=1`.
  - `GetDataLength_MultiByte` (with `numBytes=2`): `MSB=1 + numBytes>1`.

- **Circuit and Path Coverage**  
  All 4 linearly independent paths (cyclomatic complexity = 4) are tested, including single-byte, exception, and multi-byte cases.
