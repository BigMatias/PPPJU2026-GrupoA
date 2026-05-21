using UnityEngine;

/* ----- FACU -----
 * RunManager lo creé porque no entiendo por ahora donde meter esto en el gameManager :p
 * Lo hago singleton, pero solo durante la escena de la run, porque sino guardaría valores de runs anteriores y we don't want that
 * Después, habría que agregarle todos los managers que usemos así en global, porque su función básicamente es:
 * RunManager.Instance.AbcManager.DoSmth();
 * Osea, en vez de mil singletons de managers, el run nomás que tenga todos :>
 * Este script debería ir en el mismo GO que los managers, así hace getComponent de todos
*/

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    public GauchoContext GauchoContext { get; private set; }
    public GameEvents GameEvent { get; private set; }

    // MANAGERS
    public GauchosManager Gauchos { get; private set; }
    public ShopManager ShopManager { get; private set; }

    private void Awake()
    {
        Instance = this;

        GauchoContext = new();
        GameEvent = GameEvents.None;
        Gauchos = GetComponent<GauchosManager>();
        ShopManager = GetComponent<ShopManager>();
    }

    public void UpdateGameEvent(GameEvents gameEvent)
    {
        GameEvent = gameEvent;
        Gauchos.Trigger(GameEvent);
    }
}