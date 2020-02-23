![logo](Images/KaosCollections-218.png)

<a href="https://github.com/kaosborn/KaosCollections/blob/master/.github/workflows/Build.yml">
<img src="https://github.com/kaosborn/KaosCollections/workflows/Build/badge.svg"></a>
<a href="https://github.com/kaosborn/KaosCollections/blob/master/.github/workflows/Test.yml">
<img src="https://github.com/kaosborn/KaosCollections/workflows/Test/badge.svg"></a>

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

This library is multitargeted to:

* .NET Standard 2.1 (v4.2 and prior targeted .NET Standard 1.0)
* .NET Framework 4.5
* .NET Framework 4.0
* .NET Framework 3.5 (ends with v4.2 of this library)

### Library installation

#### To install using Package Manager:

* **`Install-Package Kaos.Collections -Version 4.2.0`**

#### To install using the .NET CLI:

* **`dotnet add package Kaos.Collections --version 4.2.0`**

#### To install using Visual Studio gallery:

1. Click *Manage NuGet Packages*.
2. Select package source of *nuget.org*.
3. Click *Browse* and input **Kaos.Collections**.
4. The package should appear. Click *Install*.
As a multitargeted package, the appropriate binary will be installed for your program.

#### To install using a direct reference to a `.dll` binary:

1. Download the `.nuget` package from either:

   * https://www.nuget.org/packages/Kaos.Collections/
   * https://github.com/kaosborn/KaosCollections/releases/

2. As archives, individual binaries may be extracted from the NuGet package for specific platforms.
This may require changing the file extension from `.nuget` to `.zip` to access the contents.
A project may then reference the extracted platform-specific `.dll` directly.

### Documentation

Installing as a NuGet package will provide IntelliSense and object browser documentation from the `.xml` file.
For complete documentation, see:

* https://kaosborn.github.io/help/KaosCollections/

Identical documentation is available as a Microsoft Help v1 file from the link below.
This downloaded `.chm` file may require unblocking thru its file properties dialog.

* https://github.com/kaosborn/KaosCollections/releases/

Finally, examples are repeated in the repository wiki:

* https://github.com/kaosborn/KaosCollections/wiki/

### Roadmap

[X] Add serialization to .NET Standard build target
[X] Remove .NET 3.5 build target; change .NET Standard build target to v2.1
[ ] Add GAC installer

Latest library builds are posted as workflow artifacts to:

https://github.com/kaosborn/KaosCollections/actions?query=workflow%3ABuild

### Repository top-level folders

This repository is a single Visual Studio solution with additional files in the root.

* `Bench` - Contains console programs that:

  * Provide examples for documentation.
  * Exercise classes in this library.
  * Benchmark performance versus Microsoft classes.
  * Benchmark performance for tuning.
  * Stress test.
  * Show breadth first tree charts for operation sequences.

* Collections - Contains the `.nuget` library build of KaosCollections.
This library is multitargeted to .NET Standard 2.1, .NET 4.5 and .NET 4.0.

* `Help` - Contains [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project that produces documentation from embedded XML comments.
Output is a Microsoft Help v1 file with a `.chm` extension and (optionally) a static web site.

* `Images` - Contains SVG files with renderings.

* `Source` - Contains source code in shared projects by namespace by class.

* `TestCore` - Contains MSTest unit tests and some short running stress tests.
Line and branch coverage is 100%.
To verify correct emulation,
the same tests may be run against either this library or against the emulated BCL classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.

### Build requirements

All links in this section are to free software.

* https://github.com/kaosborn/KaosCollections/ - Complete source is hosted at GitHub.

* https://www.visualstudio.com/downloads/ - Building the solution requires Visual Studio 2017 Community Edition or greater.

* https://github.com/EWSoftware/SHFB/releases/ - Building `.chm` or web documentation requires Sandcastle Help File Builder.
