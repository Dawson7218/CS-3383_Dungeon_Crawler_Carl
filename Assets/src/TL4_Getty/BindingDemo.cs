using UnityEngine;
public class BindingDemo : MonoBehaviour
{
CurrencyTransaction current = new PurchaseTransaction(); // declared: Enemy · actual: Dragon​

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), current.Execute());
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is PurchaseTransaction) ? new CurrencyTransaction() : new PurchaseTransaction();
    }
}