using UnityEngine;
public class TLFiveBindingDemo : MonoBehaviour
{
BossPhase current = new Phase1_RangedBarrage(); // declared: Enemy · actual: Dragon​

void OnGUI()
{
GUI.Label(new Rect(20, 20, 400, 30), current.CanHandle());
if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
   current = (current is Phase1_RangedBarrage) ? new BossPhase() : new Phase1_RangedBarrage();
}
}