using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public Material[] materials;
    // light, dark, light, dark . . .
    // light = 2*(int)Flavor, dark = 2*(int)Flavor+1
    public LayerMask terrainLayers;
    public float gravity;
    public Sprite[] flavorIcons;
    public Sprite[] healthBars;
    public Sprite[] healthIcons;
    public Sprite[] manaBars;
    public Sprite[] manaIcons;
}
