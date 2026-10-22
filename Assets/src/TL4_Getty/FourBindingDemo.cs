using UnityEngine;
public class FourBindingDemo : MonoBehaviour
{
// The variable's declared type is CurrencyTransaction; the actual object starts as PurchaseTransaction.
CurrencyTransaction current = new PurchaseTransaction(StatType.Health, 3);

    void OnGUI()
    {
        // A fresh, temporary object keeps this binding demo from touching real save data.
        // It starts with zero points, so the purchase override reports insufficient points.
        EconomySaveData candidate = new EconomySaveData();
        // Execute calls the override belonging to the object currently stored in current.
        GUI.Label(new Rect(20, 20, 400, 30), current.Execute(candidate));
        // Swap between a purchase object and a plain base object to compare their messages.
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is PurchaseTransaction) ? new CurrencyTransaction() : new PurchaseTransaction(StatType.Health, 3);
    }
}
