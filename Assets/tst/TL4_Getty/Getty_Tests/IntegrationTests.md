# TL4 PlayMode tests

All 48 TL4 cases belong to Getty_Tests. In Unity, open Window > General > Test Runner,
select PlayMode, select Getty_Tests, and click Run Selected. To run only the 16
integration cases, select MetaRunIntegrationTests inside that assembly instead.
The runner enters and exits Play Mode automatically. Passing cases receive green checks.

Integration coverage includes upgraded combat stats and full health, save/reload,
run payout calculation, duplicate prevention, menu exits, boss notifications,
failed-save retries, invalid inputs, and resets between runs.

Each save test uses a unique temporary directory, never the player's economy.json.
Fixtures use temporary runtime scenes and await their cleanup. Private lifecycle
methods are invoked for controlled setup; these tests do not replace a full gameplay
playtest of the bootstrap, real gameplay scene unloading, or the boss countdown.
