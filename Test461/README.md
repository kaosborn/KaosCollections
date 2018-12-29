### Test usage notes

* To verify correct emulation, these tests may be run against either this library
or against the emulated BCL classes.
To run the test suite against the Microsoft classes,
add the `TEST_BCL` compilation symbol to the test project build properties.

* Stress tests may be executed in either a short running or long running mode.
The default mode is short mode which is sufficient for coverage analysis.
To execute stress tests in long running mode,
add the `STRESS` compilation symbol to the test project build properties.

* Debug builds conditionally include intrinsic structural checks
that are not included in Release builds.
