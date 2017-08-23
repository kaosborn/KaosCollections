### Documentation

For a project overview, see this project's [wiki page](https://github.com/kaosborn/KaosCollections/wiki).
For complete documentation, download the `.chm` file
(requires Microsoft Help Viewer to view).

### Project status and roadmap

This project is stable and code complete.
Work is in progress for:

- [ ] Additional tests for code coverage
- [ ] Documentation (including examples)
- [ ] Impact of .NET Standard 2.0 including serialization
- [ ] RankedBag<T> class

### Project layout

* The `Bench` folder contains console program projects that mostly target the .NET 4.62 library build.
Full library is preferred over Core due to dereased build times.
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
This library is Multi-targetted to .NET Standard, .NET 3.5 and .NET 4.
The documentation project (`.chm`) is included in this project as well.

* The `Collections462` folder contains a .NET 4.62 build of the class library.
This project is used for development and testing only.

* The `Help` folder contains a [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project that produces documentation based on embedded XML comments.
This project produces a Microsoft Help v1 file with a `.chm` extension.

* The `Source` folder contains all source code for KaosCollections.
All source is organized using shared projects which are referenced by the build projects.

* The `Test462` folder contains unit tests and some short running stress tests.
To show correct emulation, these tests may be run against either this library
or against the related Microsoft base class library (BCL).
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.