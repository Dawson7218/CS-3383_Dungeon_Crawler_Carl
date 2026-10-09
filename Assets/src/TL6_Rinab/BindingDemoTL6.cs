using UnityEngine;

public class BindingDemoTL6 : MonoBehaviour
{
PlayerState current = new PlayerIdleState();

void OnGUI()
{
GUI.Label(new Rect(20, 20, 400, 30), current.Update());
if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
   current = (current is PlayerIdleState) ? new PlayerState() : new PlayerIdleState();
}
}