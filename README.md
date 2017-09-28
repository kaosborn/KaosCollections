![logo](Images/KaosCollections-218.png)
# KaosCollections

### Overview

KaosCollections is a .NET library that provides generic collection classes
for storing elements that are both sorted and indexed.
By using an advanced underlying data structure, these classes provide
both superior performance and capability to their Microsoft Base Class Library equivalents.

The primary classes provided are:

* `RankedDictionary<TKey,TValue>` - for sorted dictionaries.
* `RankedSet<T>` - for sorted sets.
* `RankedBag<T>` - for sorted multisets.

Both `RankedDictionary` and `RankedSet` closely emulate the API of their BCL counterparts
(`SortedDictionary` and `SortedSet`) while `RankedBag` has no BCL counterpart.

This library is built as a .NET Standard project with multi-targeting to:

* .NET Standard 1.0.
* .NET Framework 4.5.
* .NET Framework 4.0.
* .NET Framework 3.5.

Building this project requires Visual Studio 2017.

### Documentation

For complete documentation, see:

https://kaosborn.github.io/help/KaosCollections/

An offline version of this documentation is also provided as a `.chm` file:

https://github.com/kaosborn/KaosCollections/releases

Benchmarks may be viewed here:

https://github.com/kaosborn/KaosCollections/wiki/Benchmarks

### Distribution

Future versions will be released on Nuget.org.
Current beta versions are available on GitHub.com.

### Project status and roadmap

This project is stable and code complete.
Work is in nearing completion for:

- [X] Additional tests for code coverage
- [ ] Documentation (including examples)
- [X] Impact analysis of .NET Standard 2.0 including serialization
- [X] RankedBag<T> class
- [X] Add RemoveRange methods

Release of version 4 is planned real soon now.

### Project layout

* The `Bench` folder contains console program projects that mostly target the .NET 4.62 library build.
Compiling against the full library is preferred over Core due to decreased build times.
These programs exist to:

  * Provide examples for documentation
  * Exercise classes in this library
  * Benchmark performance versus Microsoft classes
  * Benchmark performance for tuning
  * Stress test
  * Show breadth first tree charts for operation sequences

* The `Collections` folder contains the primary build of the class library.
Building the Release configuration of the project contained in this folder
will produce a `.nuget` file for distribution.

* The `Collections462` folder contains a .NET 4.62 build of the class library.
This project is used for development and testing only.

* The `Help` folder contains a [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project that produces documentation from embedded XML comments.

* The `Source` folder contains all source code for KaosCollections.
All source is organized using shared projects which are referenced by the build projects.

* The `Test462` folder contains unit tests and some short running stress tests.
Code coverage is 99%.
To verify correct emulation, these tests may be run against either this library
or against the emulated BCL classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.