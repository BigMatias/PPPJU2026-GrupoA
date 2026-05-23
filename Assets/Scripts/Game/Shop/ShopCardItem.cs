using UnityEngine;
public class ShopCardItem : MonoBehaviour
{
    private ShopManager shopManager;
    private CardShopSlot slot;
    public void Setup(ShopManager shopManager, CardShopSlot slot)
    {
        this.shopManager = shopManager;
        this.slot = slot;
    }
    public void Buy() => shopManager.BuyCard(slot);
}