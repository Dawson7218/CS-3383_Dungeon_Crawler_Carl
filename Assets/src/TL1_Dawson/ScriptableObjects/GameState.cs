using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{
    public int tmp {  get; set; }

    public bool isPaused = false;
    public int score = 0;
    public float gameTimer = 0f;
    public int dabloons = 0;

    public List<HudElement> hudElements = new();

     
}
