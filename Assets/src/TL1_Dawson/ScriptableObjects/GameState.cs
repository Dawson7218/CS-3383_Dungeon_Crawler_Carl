using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{
    public int tmp { get; set; }

    public bool isPaused { get; set; } = true;
    public bool bcMode { get; set; } = false;
    public int score { get; set; } = 0;
    public float gameTimer { get; set; } = 0f;
    public int dabloons { get; set; } = 0;
    public int playerHealth { get; set; } = 75;

    //public List<HudElement> hudElements = new();

     
}
