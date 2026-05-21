using UnityEngine;

/* ----- FACU -----
 * Este script se encarga de colocar y decir que el joker asignado fue comprado
 * Está separado para un flujo más comprensible en el que pueda ir desde la UI hasta aca y de aca al shopManager
*/

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