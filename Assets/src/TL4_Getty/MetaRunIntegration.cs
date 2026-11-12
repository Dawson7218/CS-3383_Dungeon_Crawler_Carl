using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Connects the shop to gameplay without changing another team's scenes or scripts.
public class MetaRunIntegration : MonoBehaviour
{
    public static MetaRunIntegration Instance { get; private set; }

    private readonly HashSet<BossController> bosses = new HashSet<BossController>();
    private bool running;
    private bool payoutPending;
    private int kills;
    private bool bossKilled;
    private float seconds;
    private double startedAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance != null) return;
        var owner = new GameObject("Meta run integration");
        Instance = owner.AddComponent<MetaRunIntegration>();
        DontDestroyOnLoad(owner);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        EnemyController.OnEnemyDefeated += EnemyDefeated;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        EnemyController.OnEnemyDefeated -= EnemyDefeated;
        ClearBosses();
        if (Instance == this) Instance = null;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "DeathScene")
        {
            if (running) EndRun();
            else TrySavePayout();
            return;
        }

        // A menu/shop visit is not a completed run. Retry any failed save there.
        TrySavePayout();
        if (scene.name != "GameScene" && scene.name != "MVPScene")
        {
            if (mode == LoadSceneMode.Single)
            {
                running = false;
                ClearBosses();
            }
            return;
        }

        StartCoroutine(BeginRun());
    }

    private IEnumerator BeginRun()
    {
        // Let player and HUD initialization finish before applying upgraded health.
        yield return null;
        var player = FindAnyObjectByType<PlayerController>();
        var economy = EconomyManager.Instance;
        if (running || player == null || economy == null) yield break;
        if (payoutPending)
        {
            Debug.LogError("Previous run payout could not be saved. Check the economy save before starting another run.");
            yield break;
        }

        var stats = player.GetStats();
        economy.ApplyUpgrades(stats);
        foreach (var hud in FindObjectsByType<HUDManager>())
            if (hud.gameState != null) hud.gameState.playerHealth = stats.currentHP;
        foreach (var manager in FindObjectsByType<GameStateManager>())
            if (manager.gameState != null) manager.gameState.playerHealth = stats.currentHP;

        kills = 0;
        bossKilled = false;
        startedAt = Time.timeAsDouble;
        running = true;
        WatchBosses();
    }

    private void Update()
    {
        // Rooms can create their boss after the run has already started.
        if (running) WatchBosses();
    }

    private void WatchBosses()
    {
        foreach (var boss in FindObjectsByType<BossController>(FindObjectsInactive.Include))
            if (bosses.Add(boss)) boss.OnBossDefeated += BossDefeated;
    }

    private void EnemyDefeated(Enemy enemy)
    {
        if (running) kills++;
    }

    private void BossDefeated()
    {
        if (running) bossKilled = true;
    }

    // Other teams can call this before changing scenes when a run is completed.
    // DeathScene calls it automatically. Repeated calls never pay the same run twice.
    public void EndRun()
    {
        if (!running) return;
        seconds = (float)(Time.timeAsDouble - startedAt);
        running = false;
        payoutPending = true;
        ClearBosses();
        TrySavePayout();
    }

    private void TrySavePayout()
    {
        if (!payoutPending) return;
        var economy = EconomyManager.Instance;
        // The gameplay manager is scene-owned and is gone on arrival at DeathScene.
        if (economy == null)
            economy = new GameObject("Run payout economy").AddComponent<EconomyManager>();
        int points = economy.calculateRunPoints(kills, bossKilled, seconds);
        if (economy.TryCreditRunEnd(points, seconds))
        {
            payoutPending = false;
            Debug.Log("Run complete: " + points + " meta points saved.");
        }
        else Debug.LogError("Run payout was not saved: " + economy.LastError);
    }

    private void ClearBosses()
    {
        foreach (var boss in bosses)
            if (boss != null) boss.OnBossDefeated -= BossDefeated;
        bosses.Clear();
    }
}

