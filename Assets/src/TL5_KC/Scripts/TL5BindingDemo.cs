using UnityEngine;

public class TLFiveBindingDemo : MonoBehaviour
{
    [SerializeField] private BossController boss;

    private void OnGUI()
    {
        if (boss == null)
        {
            GUI.Label(
                new Rect(20, 20, 500, 30),
                "Assign FinalBoss to the demo's Boss field.");
            return;
        }

        BossPhaseInfo info = boss.GetCurrentPhase();

        GUI.Label(
            new Rect(20, 20, 500, 30),
            "Phase: " + info.PhaseName);

        GUI.Label(
            new Rect(20, 55, 500, 30),
            "Boss HP: " + info.CurrentHealth + " / " + info.MaximumHealth);

        if (!info.IsDefeated &&
            GUI.Button(new Rect(20, 95, 180, 35), "Deal 100 Boss Damage"))
        {
            boss.TakeDamage(100);
        }

        if (info.IsDefeated)
        {
            GUI.Label(
                new Rect(20, 140, 500, 30),
                "Boss defeated!");
        }
    }
}