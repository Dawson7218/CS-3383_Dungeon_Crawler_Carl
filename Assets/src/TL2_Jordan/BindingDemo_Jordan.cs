using UnityEngine;

public class BindingDemo : MonoBehaviour​
{
Room current = new CombatRoom(); // declared: Enemy · actual: Dragon​

void OnGUI()
{
GUI.Label(new Rect(20, 20, 400, 30), current.printString());
if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
   current = (current is CombatRoom) ? new Room() : new CombatRoom();
}
}