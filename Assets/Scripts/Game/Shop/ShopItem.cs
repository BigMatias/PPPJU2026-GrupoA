using UnityEngine;

public class ShopItem : MonoBehaviour
{
    private ShopManager _shopManager;
    private ShopGauchoSlot _slot;

    public void Setup(ShopManager shopManager, ShopGauchoSlot slot)
    {
        _shopManager = shopManager;
        _slot = slot;
    }

    public void Buy() => _shopManager.BuyGaucho(_slot);
}