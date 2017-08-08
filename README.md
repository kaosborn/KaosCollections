### Documentation

For a project overview, see this project's wiki page.
For complete documentation, download the `.chm` file
(requires Microsoft Help Viewer to view).

### Project status

This project is stable and code complete.
Work is still in progress for:

* Additional tests for code coverage
* Documentation
* Future .NET Standard 2 implications

### Project layout

The `Bench` folder contains console program projects mostly target the .NET 4 library build.
The .NET 4 build is preferred over the primary build for decreased build times.
These purpose of these programs is to:

* exercise classes in this library
* provide examples for documentation
* benchmark performance versus Microsoft classes
* benchmark performance for different inputs
* Stress test
* Show breadth first tree charts for operation sequences

The `Collections` folder contains the primary build of the class library.
Building the Release configuration of the project contained in this folder
will produce a .nuget file that will be released on NuGet.org.
This library is Multi-targetted to .NET Standard, .NET 3.5 and .NET 4.
The documentation project (`.chm`) is included in this project as well.

The `Collections400` folder contains a .NET 4 build of the class library.
This project is for development use only.

The `Help` folder contains a Sandcastle Help File Builder project
that produces documentation based on embedded XML comments.
This project produces a Microsoft Help v1 file with an extension of `.chm`.

The `Source` folder contains all source code for the distributed project.

The `TestCore` folder contains unit tests and is built against .NET Core v1.0.
These tests are all designed to run against either this library
or against the Microsoft equivalent classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.