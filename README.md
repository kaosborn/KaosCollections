### Documentation

For a project overview, see this project's [wiki page](https://github.com/kaosborn/KaosCollections/wiki).
For complete documentation, download the `.chm` file
(requires Microsoft Help Viewer to view).

### Project status

This project is stable and code complete.
Work is still in progress for:

- [ ] Additional tests for code coverage
- [ ] Documentation (including examples)
- [ ] Impact of .NET Standard v2 including serialization

### Project layout

* The `Bench` folder contains console program projects that mostly target the .NET 4 library build.
The .NET 4 build is preferred over the primary build for decreased build times.
The purpose of these programs is to:

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

* The `Collections400` folder contains a .NET 4.0 build of the class library.
This project is for development use only.

* The `Help` folder contains a [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB)
project that produces documentation based on embedded XML comments.
This project produces a Microsoft Help v1 file with a `.chm` extension.

* The `Source` folder contains all source code for KaosCollections.
All source is organized using shared projects which are referenced by the build projects.

* The `TestCore` folder contains unit tests and is built against .NET Core v1.0.
These tests are all designed to run against either this library
or against the Microsoft equivalent classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.