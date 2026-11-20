Jordan's automated tests
=======================

New suite: EditMode/TL2_Jordan_EditModeTests.asmdef
47 individually named tests, all verified passing in Unity.

- DungeonGenerationTests: 35 tests
- RoomConnectionTests: 5 tests
- RoomDoorwayTests: 7 tests

To run: open Window > General > Test Runner in Unity, select EditMode,
then select TL2_Jordan_EditModeTests and run that suite.

Full coverage guide and test inventory:
  docs/TL2_Jordan/Testing.md (relative to the repository root)

Saved Unity test report:
  docs/TL2_Jordan/EditMode-TestResults.xml
  Result: Passed; total 47; passed 47; failed 0; skipped 0.

The original TL1_Jordan_Tests folder is preserved, including the binding
example, room tracking tests, encounter tests, and stress-test scene.
The manual stress-test scene is not counted as an automated unit test.
The new 47 tests alone exceed the requirement of 35.