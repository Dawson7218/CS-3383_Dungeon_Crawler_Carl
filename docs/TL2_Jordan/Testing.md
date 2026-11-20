# Jordan's dungeon tests

The new Edit Mode suite contains **47 individually named `[Test]` methods**. These are 47 standalone tests, not a smaller number expanded using `[TestCase]` parameters. They meet a 35-test requirement without relying on the old toy tests.

| Fixture | Standalone tests | Coverage |
| --- | ---: | --- |
| DungeonGenerationTests | 35 | Room counts and edge cases, connectivity, reciprocal/cardinal connections, boss constraints, random seeds, room probabilities, regeneration, events, and transitions |
| RoomConnectionTests | 5 | Neighbor lookup, direction isolation, separate room state, replacing connections, and preserving connections when metadata changes |
| RoomDoorwayTests | 7 | Closed/open doors, four-way junctions, reconfiguration, null input, and actual prefab configuration |
| **New suite total** | **47** | |

The existing BindingTest, RoomTrackingTests, CombatRoomEncounterTests, and stress-test scene remain in their original folder. The stress-test scene is a manual performance experiment, not an automated unit test. No production scripts, scenes, or teammate files were intentionally edited for this addition.

## Run the new tests in Unity

1. Open the project and wait for script compilation/import to finish.
2. Open **Window > General > Test Runner**.
3. Choose **EditMode**.
4. Select **TL2_Jordan_EditModeTests**, then run the selected suite.
5. Confirm that Unity discovers 47 tests and inspect any red failures. Save/export the results if your instructor wants evidence.

The new assembly is Editor-only so these tests do not ship with the game. Edit Mode tests do not require starting GameScene. Existing tests in the original assembly are separate; the new suite does not depend on them.

## What the assertions check

Tests arrange their own objects/settings, call the real public game APIs, and assert observable results. Each fixture cleans up its temporary GameObjects. Generation distance/connectivity tests traverse the resulting room graph with an independent breadth-first search instead of copying the generator's reserved-path algorithm. Seed tests compare map contents, not a brittle hardcoded map snapshot. Probability boundary tests use settings of zero or one, making their expectations deterministic.

Reflection is used only to set private serialized generator fields as the Inspector would. Production prefab tests load the actual room prefabs read-only. The tests that consume Unity's global random generator restore its state afterward.

## Limits

These tests exercise logic, component state, and prefab wiring. They do not demonstrate that combat feels good, that physics behaves correctly over time, that UI looks right, or that a complete run can be finished. Those still require Play Mode/gameplay testing. The new suite also does not claim to validate independently owned player, loot, economy, or boss implementations beyond their room integration.

## New test inventory
### DungeonGenerationTests

- BeforeGenerationThereIsNoCurrentRoom
- TransitionBeforeGenerationFailsWithoutADestination
- RequestedRoomCountIsPreservedForSmallAndLargeRuns
- ZeroRequestedRoomsProducesOneValidRoom
- NegativeRequestedRoomsProducesOneValidRoom
- StartingRoomIsAtTheGridOrigin
- MultiRoomRunStartsInACorridor
- GeneratedRoomsNeverShareAGridPosition
- EveryRoomIsReachableFromTheStart
- DungeonContainsExactlyOneBossRoom
- BossRoomHasExactlyOneEntrance
- BossMeetsTheDefaultShortestPathDistance
- ConfiguredBossDistanceIsHonored
- SmallDungeonClampsImpossibleBossDistance
- SingleRoomDungeonIsABossWithoutEntrances
- EveryConnectionHasAReciprocalConnection
- ConnectionsLeadToTheCorrectAdjacentGridCell
- AllOccupiedAdjacentCellsAreConnectedAndEmptyCellsAreNot
- SameSeedReproducesPositionsAndRoomTypes
- UnityRandomActivityDoesNotChangeASeededMap
- DifferentSeedsCanProduceDifferentLayouts
- ZeroTreasureChanceProducesNoTreasureRooms
- ZeroTreasureLimitOverridesGuaranteedTreasureChance
- TreasureRoomsNeverExceedTheirConfiguredLimit
- GuaranteedTreasureCannotReplaceStartOrBoss
- GuaranteedCombatFillsEligibleNonSpecialRooms
- ZeroCombatAndTreasureChancesLeaveCorridorsAndBoss
- LogicalRoomTypeMatchesItsConcreteSubclass
- RegenerationReplacesOldRoomsInsteadOfAccumulatingThem
- RoomGeneratedEventReportsEveryRoomExactlyOnce
- GenerationSubscribersSeeTheFullyConnectedMap
- StartingRoomEventFollowsAllGenerationEvents
- BlockedTransitionDoesNotChangeRoomOrRaiseAnEvent
- ValidTransitionNotifiesOnceWithTheDestination
- TransitionThroughTheOppositeDoorReturnsToStart

### RoomConnectionTests

- MissingNeighborReturnsNullAndIsNotReportedAsConnected
- AddingOneNeighborDoesNotConnectOtherDirections
- SeparateRoomsDoNotShareTheirNeighborCollections
- ReplacingANeighborPreservesTheOtherConnections
- ChangingRoomMetadataDoesNotEraseItsConnections

### RoomDoorwayTests

- RoomWithoutNeighborsClosesAllFourDoorways
- EachConnectionOpensOnlyItsMatchingDoorway
- FourWayJunctionDisablesAllDoorwayWallsAndTheirCollisions
- ReconfigurationClosesPreviouslyOpenDoors
- NullRoomDoesNotChangeExistingDoorwayStates
- ProductionRoomVariantsHaveTheCorrectLogicalTypes
- BaseRoomContainsFourSolidDoorwayBlockers

## Verified Unity result

Executed the new assembly using Unity 6000.5.9f1 in Edit Mode: **47 passed, 0 failed, 0 skipped**. See [the saved NUnit XML report](EditMode-TestResults.xml). This result applies to the new 47-test assembly; the existing tests were not included in this particular run.
