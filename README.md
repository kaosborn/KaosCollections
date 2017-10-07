![logo](Images/KaosCollections-218.png)
# KaosCollections

### Overview

KaosCollections is a .NET library that provides generic collection classes
for storing elements that are both sorted and indexed.
By using an advanced underlying data structure, these classes provide
superior performance to their Microsoft Base Class Library (BCL) counterparts
while delivering greatly enhanced capability.

The primary classes provided are:

* `RankedDictionary<TKey,TValue>` - for collections of key/value pairs with distinct keys that can be accessed in sort order or by index.
* `RankedSet<T>` - for collections of distinct items that can be accessed in sort order or by index.
* `RankedBag<T>` - for collections of items that can be accessed in sort order or by index. Also known as multisets.

Both `RankedDictionary` and `RankedSet` closely emulate the API of their BCL counterparts
(`SortedDictionary` and `SortedSet`) while `RankedBag` has no BCL counterpart.
All three classes include indexing capabilities such determining the index of an element or getting elements by index.

This library is built as a .NET Standard project with multitargeting to:

* .NET Standard 1.0.
* .NET Framework 4.5.
* .NET Framework 4.0.
* .NET Framework 3.5.

### Library download and installation

1. Click **Manage NuGet Packages**.
2. Select package source of **nuget.org**.
3. Click **Browse** and input **Kaos.Collections**.
4. The package should appear. Click **Install**.
As a multitargeted package, the appropriate binary will be selected for your program.

Direct downloads are also available at NuGet.org and GitHub.com:

https://www.nuget.org/packages/Kaos.Collections/

https://github.com/kaosborn/KaosCollections/releases/

As archives, individual binaries may be extracted from the `.nuget` package for specific platforms.
A project may then reference the extracted platform-specific `.dll` directly.

### Documentation

Installing as a NuGet package will provide IntelliSense and object browser documentation as a `.xml` file.
For complete documentation, see:

https://kaosborn.github.io/help/KaosCollections/

An offline version of this documentation is also provided as a `.chm` file:

https://github.com/kaosborn/KaosCollections/releases/

Benchmarks and examples may be viewed here:

https://github.com/kaosborn/KaosCollections/wiki/

### Status

This project is stable and code complete.

### Build environment

Complete source code with embedded XML documentation is hosted at GitHub.com.
Building the solution requires Visual Studio 2017 Community Edition or greater.
Building documentation requires Sandcastle Help File Builder.

### Repository layout

This repository is a single Visual Studio solution with additional files in the root.

* The `Bench` folder contains console program projects that mostly target the .NET 4.62 library build.
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

* The `Images` folder contains the logo `.svg` file and its `.png` conversions.

* The `Help` folder contains a [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project that produces documentation from embedded XML comments.
To build this project, first build `Collections462` and build in Release configuration.

* The `Source` folder contains all source code for KaosCollections.
All source is organized using shared projects which are referenced by the build projects.

* The `Test462` folder contains unit tests and some short running stress tests.
Code coverage is 99%.
To verify correct emulation, these tests may be run against either this library
or against the emulated BCL classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.
Testing against the full framework is preferred over Core due to:

  * Decreased build times
  * Full API support
  * Support for code coverage analysis
