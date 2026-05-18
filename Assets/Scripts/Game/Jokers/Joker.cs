using UnityEngine;

public class Joker : MonoBehaviour
{
    public JokerDataSO jokerData { get; private set; }

    public bool isActive = false;
    public bool isVisibleInShop = false;

    public virtual void Initialize(JokerDataSO data) => jokerData = data;

    public virtual void Execute() { }

    public void ToggleJokerOnShop(bool isOn)
    {
        isVisibleInShop = isOn;
    }

    public float ModifyPoints(float points)
    {
        if (!isActive)
            return 0f;

        if (jokerData.bonusPoints > 0)
            points += jokerData.bonusPoints;

        return points;
    }

    public float ModifyMult(float mult)
    {
        if (!isActive)
            return 0f;

        if (jokerData.bonusMultAdd > 0)
        {
            mult += jokerData.bonusMultAdd;
            // Pasar a la UI
        }

        if (jokerData.bonusMultX > 0)
        {
            mult *= jokerData.bonusMultX;
            // Pasar a la UI
        }

        if (jokerData.bonusMultX > 0)
        {
            mult *= jokerData.bonusMultX;
            // Pasar a la UI
        }

        if (jokerData.bonusMultXAdditiveTotal > 0)
        {
            mult *= jokerData.bonusMultX;
            // Pasar a la UI
        }

        return mult;
    }
}