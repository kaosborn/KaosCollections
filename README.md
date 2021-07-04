![logo](Images/KaosCollections-218.png)

[![Test](https://github.com/kaosborn/KaosCollections/workflows/Test/badge.svg)](https://github.com/kaosborn/KaosCollections/blob/master/.github/workflows/Test.yml)
[![Build](https://github.com/kaosborn/KaosCollections/workflows/Build/badge.svg)](https://github.com/kaosborn/KaosCollections/blob/master/.github/workflows/Build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/kaosborn/KaosCollections/blob/master/LICENSE)

# KaosCollections

KaosCollections is a .NET library that provides generic collection classes
for storing elements that are both sorted and indexed.
Based on order statistic B+ trees,
the classes that emulate Microsoft's SortedDictionary and SortedSet
provide greater capability than their counterparts while outperforming them for large collection sizes.
Also included is a bag class and a map class.
All classes provide getting elements by index, getting the index of an element, range removal by index, range enumeration, and more.

Primary types provided are:

* `RankedDictionary<TKey,TValue>` - for collections of key/value pairs with distinct keys that can be accessed in sort order or by index.
* `RankedSet<T>` - for collections of distinct items that can be accessed in sort order or by index.
* `RankedMap<TKey,TValue>` - for collections of key/value pairs with nondistinct keys that can be accessed in sort order or by index.
* `RankedBag<T>` - for collections of nondistinct items that can be accessed in sort order or by index. Also known as multisets.

The current build of this library targets .NET Standard 2.0.
This provides support for .NET Framework 4.6.1 and greater or .NET Core 3.0 and greater.

The most recently published version 4.2.0 multitargeted .NET Standard 2.1, .NET 3.5, .NET 4.0, and .NET 4.5.

### Library installation

#### To install using Package Manager:

* **`Install-Package Kaos.Collections --version 4.2.0`**

#### To install using the .NET CLI:

* **`dotnet add package Kaos.Collections --version 4.2.0`**

#### To install using Visual Studio gallery:

1. Click *Manage NuGet Packages*.
2. Select package source of *nuget.org*.
3. Click *Browse* and input **Kaos.Collections**.
4. The package should appear. Click *Install*.
As a multitargeted package, the appropriate binary will be installed for your program.

#### To install by source code reference:

Rather than referencing a compiled library, the shared project source code may be referenced.
Do this by adding a project reference to the `KaosCollections.shproj` file.

### Documentation

Installing as a NuGet package will provide IntelliSense and object browser documentation from the `.xml` file.
For complete documentation, see:

* https://kaosborn.github.io/help/KaosCollections/

Identical documentation is available as a Microsoft Help v1 file from the link below.
This downloaded `.chm` file may require unblocking thru its file properties dialog.

* https://github.com/kaosborn/KaosCollections/releases/

Finally, examples are repeated in the repository wiki:

* https://github.com/kaosborn/KaosCollections/wiki/

### Repository top-level folders

This repository holds a single Visual Studio solution plus a few more files.
Folders referenced by the solution are:

* `Bench` - Contains console programs that:

  * Provide examples for documentation.
  * Exercise classes in this library.
  * Benchmark performance versus corresponding Microsoft classes.
  * Benchmark performance for tuning.
  * Stress test.
  * Show breadth first tree charts of operation sequences.

* `Collections` - Builds the `.nuget` package.
The current build of this NuGet library targets .NET Standard 2.0.

* `Help` - Contains a Sandcastle Help File Builder project that produces documentation from embedded XML comments.
Output is a Microsoft Help v1 file with a `.chm` extension and (optionally) a static web site.
Building this project requires Visual Studio.

* `Images` - Contains SVG files with renderings.

* `Source` - Contains source code in shared projects by namespace, by class.

* `TestCore` - Contains MSTest unit tests and some short running stress tests.
Line and branch coverage is 100%.
To verify correct Base Class Library emulation,
the same tests may be run against either this library or against the emulated BCL classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.

### Build requirements

All links in this section are to free software.

* https://github.com/kaosborn/KaosCollections/ - Complete source is hosted at GitHub.

* https://www.visualstudio.com/downloads/ - Building the solution requires Visual Studio 2017 Community Edition or greater.

* https://github.com/EWSoftware/SHFB/releases/ - Building `.chm` or web documentation requires Sandcastle Help File Builder.

### License

All work here falls under the [MIT License](/LICENSE).
