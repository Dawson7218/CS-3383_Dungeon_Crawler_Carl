using UnityEngine;
public class FourBindingDemo : MonoBehaviour
{
CurrencyTransaction current = new PurchaseTransaction(StatType.Health, 3); // declared: Enemy · actual: Dragon​

    void OnGUI()
    {
        EconomySaveData candidate = "new EconomySaveData()";
        GUI.Label(new Rect(20, 20, 400, 30), current.Execute(candidate));
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is PurchaseTransaction) ? new CurrencyTransaction() : new PurchaseTransaction(StatType.Health, 3);
    }
}