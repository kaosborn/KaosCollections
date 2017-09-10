### Documentation

For complete project documentation, see:

https://kaosborn.github.io/help/KaosCollections/index.html

Benchmarks are available here:

https://github.com/kaosborn/KaosCollections/wiki/Benchmarks

### Project status and roadmap

This project is stable and code complete.
Work is in nearing completion for:

- [X] Additional tests for code coverage
- [ ] Documentation (including examples)
- [X] Impact analysis of .NET Standard 2.0 including serialization
- [X] RankedBag<T> class

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
This library is Multi-targetted to .NET Standard 1.0, .NET 3.5 and .NET 4.

* The `Collections462` folder contains a .NET 4.62 build of the class library.
This project is used for development and testing only.

* The `Help` folder contains a [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project that produces documentation from embedded XML comments.
Output is a Microsoft Help v1 file with a `.chm` extension and a static web site.
The help file is distributed for off-line use.
The web site is https://kaosborn.github.io/help/KaosCollections/index.html

* The `Source` folder contains all source code for KaosCollections.
All source is organized using shared projects which are referenced by the build projects.

* The `Test462` folder contains unit tests and some short running stress tests.
Code coverage is greater than 99%.
To verify correct emulation, these tests may be run against either this library
or against the emulated base class library (BCL) classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.