using UnityEngine;

/* ----- FACU -----
 * RunManager lo cree porque no entiendo por ahora donde meter esto en el gameManager :p
 * Lo hago singleton, pero solo durante la escena de la run, porque sino guardara valores de runs anteriores y we don't want that
 * Despues, habria que agregarle todos los managers que usemos asi en global, porque su funcion basicamente es:
 * RunManager.Instance.AbcManager.DoSmth();
 * Osea, en vez de mil singletons de managers, el run nomas que tenga todos :>
 * Este script deberia ir en el mismo GO que los managers, asi hace getComponent o addComponent de todos
*/

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    public GauchoContext GauchoContext { get; private set; }
    public GameEvents GameEvent { get; private set; }

    // MANAGERS
    public Deck Deck { get; private set; }
    public CardSwapManager CardSwapManager { get; private set; }
    public GauchosManager Gauchos { get; private set; }
    public ShopManager ShopManager { get; private set; }
    public MoneySystem MoneySystem { get; private set; }
    public RoundManager RoundManager { get; private set; }
    public ScoreManager ScoreManager { get; private set; }
    public GameManager GameManager { get; private set; }
    public TrucoManager TrucoManager { get; private set; }
    public EnvidoManager EnvidoManager { get; private set; }
    public PauseManager PauseManager { get; private set; }

    private void Awake()
    {
        Instance = this;

        GauchoContext = new();
        GameEvent = GameEvents.None;
        Deck = FindFirstObjectByType<Deck>();
        Gauchos = GetComponent<GauchosManager>();
        ShopManager = GetComponent<ShopManager>();
        MoneySystem = GetComponent<MoneySystem>();
        RoundManager = GetComponent<RoundManager>();
        ScoreManager = GetComponent<ScoreManager>();
        CardSwapManager = GetComponent<CardSwapManager>();
        GameManager = GetComponent<GameManager>();
        TrucoManager = GetComponent<TrucoManager>();
        EnvidoManager = GetComponent<EnvidoManager>();
        PauseManager = GetComponent<PauseManager>();
    }

    public void UpdateGameEvent(GameEvents gameEvent)
    {
        GameEvent = gameEvent;
        Gauchos.Trigger(GameEvent);
    }
}