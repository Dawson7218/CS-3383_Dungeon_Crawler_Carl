using System;
using System.IO;
using UnityEngine;

// Load the economy before scripts with Unity's default execution order use it.
[DefaultExecutionOrder(-100)]
public class EconomyManager : MonoBehaviour
{
    // Other scripts can find the active manager without a reference in the Inspector.
    // "private set" means only this class can change which manager is active.
    public static EconomyManager Instance { get; private set; }

    // SerializeField exposes these private settings in the Inspector.
    // Min sets the lowest value the Inspector accepts.
    // Starting points are used only when the player does not already have a save.
    [SerializeField, Min(0)] private int startingPoints = 100;
    // Every stat uses the same price formula and level cap.
    [SerializeField, Min(1)] private int baseCost = 25;
    [SerializeField, Min(0)] private int costPerLevel = 25;
    [SerializeField, Min(1)] private int maxLevel = 10;
    // These are the bonuses granted by ONE purchased level of each stat.
    [SerializeField, Min(1)] private int healthBonus = 10;
    [SerializeField, Min(1)] private int defenseBonus = 1;
    [SerializeField, Min(1)] private int attackBonus = 2;

    // The player's currently accepted point balance and upgrade levels.
    private EconomySaveData state;
    // If loading fails, block transactions so a bad save is not overwritten.
    private bool saveAvailable = true;
    // Unity gives each player a save folder that also works in the executable.
    // Path.Combine adds the file name with the correct directory separator.
    private string SavePath => Path.Combine(Application.persistentDataPath, "economy.json");
    // The UI can read the latest error, but only the manager can change it.
    public string LastError { get; private set; } = "";

    // Awake runs when Unity initializes this scene's manager.
    private void Awake()
    {
        // Keep one active manager if the scene accidentally contains duplicates.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // This manager belongs to its scene. Saved data carries progress between scenes.
        Instance = this;
        Load();
    }

    private void OnDestroy()
    {
        // Do not leave Instance pointing at a manager whose scene has been unloaded.
        // A discarded duplicate must not clear the real manager's reference.
        if (Instance == this) Instance = null;
    }

    // Keep the original accessor so existing scripts can still call GetInstance().
    public static EconomyManager GetInstance() => Instance;
    // These getters read progress without letting the caller edit the save object.
    // An expression-bodied method (=>) returns the expression after the arrow.
    public int GetBalance() => state.points;
    // A switch expression selects the saved level matching the requested stat.
    public int GetLevel(StatType stat) => stat switch
    {
        StatType.Health => state.healthLevel,
        StatType.Defense => state.defenseLevel,
        StatType.Attack => state.attackLevel,
        _ => throw new ArgumentOutOfRangeException(nameof(stat))
    };

    public int GetBonus(StatType stat)
    {
        // Pick the bonus for one level, then multiply it by the purchased level count.
        // The conditional operator reads: condition ? valueIfTrue : valueIfFalse.
        int bonus = stat == StatType.Health ? healthBonus : stat == StatType.Defense ? defenseBonus : attackBonus;
        // long gives the multiplication more room. Clamp before converting back to int.
        // This returns a bonus for future gameplay code; it does not change player stats.
        return (int)Math.Min(int.MaxValue, (long)GetLevel(stat) * Mathf.Max(1, bonus));
    }

    // Mathf.Max protects the calculation even if settings are changed from code.
    public bool IsMaxLevel(StatType stat) => GetLevel(stat) >= Mathf.Max(1, maxLevel);
    // Default costs are 25 at level 0, 50 at level 1, 75 at level 2, and so on.
    public int GetNextCost(StatType stat) => (int)Math.Min(int.MaxValue,
        Mathf.Max(1, baseCost) + (long)Mathf.Max(0, costPerLevel) * GetLevel(stat));
    // All three conditions must pass: saving works, the stat is below its cap,
    // and the player has enough points for the next level.
    public bool CanPurchase(StatType stat) => saveAvailable && !IsMaxLevel(stat) && GetBalance() >= GetNextCost(stat);

    public bool TryPurchase(StatType stat)
    {
        // Return false without spending anything if the upgrade is unavailable.
        if (!CanPurchase(stat)) return false;
        // The transaction describes the purchase; Commit handles saving and accepting it.
        return Commit(new PurchaseTransaction(stat, GetNextCost(stat)));
    }

    public void CreditRunEnd(int pointsEarned, bool bossKilled, float timeAlive)
    {
        // Future run/death code can call this with the completed run's payout.
        // pointsEarned is already the final amount. bossKilled is reserved for later;
        // neither bossKilled nor timeAlive currently adds a scoring multiplier.
        // NaN and Infinity are invalid floating-point time values.
        if (timeAlive < 0 || float.IsNaN(timeAlive) || float.IsInfinity(timeAlive)) return;
        Commit(new RunEndPayoutTransaction(pointsEarned));
    }

    private bool Commit(CurrencyTransaction transaction)
    {
        if (!saveAvailable) return false;
        // Work on a separate copy first. Failed transactions leave state unchanged.
        EconomySaveData candidate = state.Clone();
        // Execute uses the subclass override, whether this is a purchase or a payout.
        // Its returned message explains a rejection; Succeeded tells us if it worked.
        LastError = transaction.Execute(candidate);
        if (!transaction.Succeeded) return false;

        try
        {
            // Create the player's save folder if this is the first save.
            Directory.CreateDirectory(Application.persistentDataPath);
            // JsonUtility turns the candidate's public fields into JSON text.
            // Write a temporary file before replacing the previous save.
            string temporaryPath = SavePath + ".tmp";
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(candidate));
            // Replace an existing save, or move the first temporary save into place.
            // The null argument means no separate backup file is created.
            if (File.Exists(SavePath)) File.Replace(temporaryPath, SavePath, null);
            else File.Move(temporaryPath, SavePath);
            // Accept the new balance/levels only AFTER the file has been saved.
            state = candidate;
            LastError = "";
            return true;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
        {
            // A disk or permission failure must not spend the player's in-memory points.
            // Show a short UI message and put the detailed error in Unity's Console.
            LastError = "Could not save. Your points were kept.";
            Debug.LogWarning(exception.Message, this);
            return false;
        }
    }

    private void Load()
    {
        // Begin with a new player's starting points and zero upgrade levels.
        state = new EconomySaveData { points = Mathf.Max(0, startingPoints) };
        try
        {
            // No file means this is a new save; keep the starting values above.
            if (!File.Exists(SavePath)) return;
            // Read the JSON file and turn its fields back into an EconomySaveData object.
            EconomySaveData loaded = JsonUtility.FromJson<EconomySaveData>(File.ReadAllText(SavePath));
            // Reject a missing object or negative balance/levels before using the save.
            if (loaded == null || !loaded.IsValid()) throw new InvalidDataException("Invalid economy save.");
            state = loaded;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is ArgumentException)
        {
            // Preserve an unreadable/invalid save for investigation instead of overwriting it.
            saveAvailable = false;
            LastError = "Could not load saved upgrades. Check the Console.";
            Debug.LogWarning(exception.Message, this);
        }
    }
}
