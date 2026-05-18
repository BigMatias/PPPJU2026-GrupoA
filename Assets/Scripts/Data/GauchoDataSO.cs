using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GauchoData", menuName = "Gauchos/GauchoData")]

/* ----- FACU -----
 * La info de cada gaucho
 */

public class GauchoDataSO : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprite;

    public string jokerName;

    [TextArea] public string description;

    public int cost;

    public Rarity rarity;

    public List<GauchoEffectSO> effects;
}