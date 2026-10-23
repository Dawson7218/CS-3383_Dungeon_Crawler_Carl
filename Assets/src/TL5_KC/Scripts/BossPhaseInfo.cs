public readonly struct BossPhaseInfo
{
    public string PhaseName { get; }
    public int CurrentHealth { get; }
    public int MaximumHealth { get; }
    public bool IsDefeated { get; }

    public BossPhaseInfo(
        string phaseName,
        int currentHealth,
        int maximumHealth,
        bool isDefeated)
    {
        PhaseName = phaseName;
        CurrentHealth = currentHealth;
        MaximumHealth = maximumHealth;
        IsDefeated = isDefeated;
    }
}