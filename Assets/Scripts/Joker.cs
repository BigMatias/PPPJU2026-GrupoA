public class Joker
{
    public JokerDataSO jokerData;

    public bool IsActive = true;

    public Joker(JokerDataSO data)
    {
        jokerData = data;
    }

    public void ModifyScore(ref int points, ref int mult)
    {
        if (!IsActive)
            return;

        if (jokerData.bonusPoints > 0)
            points += jokerData.bonusPoints;

        if (jokerData.bonusMultAdd > 0)
            mult += jokerData.bonusMultAdd;

        if (jokerData.bonusMultX > 0)
            mult *= jokerData.bonusMultX;
    }
}