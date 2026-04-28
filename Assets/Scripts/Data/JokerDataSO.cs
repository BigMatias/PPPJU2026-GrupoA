using UnityEngine;

[CreateAssetMenu(fileName = "JokerData", menuName = "CardsData/JokerData")]

public class JokerDataSO : ScriptableObject
{
    public string jokerName;

    [TextArea]
    public string description;

    public int cost;

    public Rarity rarity;

    public int bonusPoints;
    public int bonusMultAdd;
    public float bonusMultX;
    public float bonusMultXAdditive;
    public float bonusMultXAdditiveTotal;

    public Sprite sprite;

    public virtual void Execute() { }
    
}

public class CardDependantJoker : JokerDataSO
{
    public override void Execute()
    {
        base.Execute();
    }
}