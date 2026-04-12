public class Joker
{
    public JokerDataSO jokerData;

    public Joker(JokerDataSO data)
    {
        jokerData = data;
    }

    private enum Rarity
    {
        None = -1,
        Common,
        Uncommon,
        Rare,
        Legendary
    }


    public void CardInitializer()
    {

    }

    public void CardDestructor()
    {

    }
}