using System.Collections.Generic;
using UnityEngine;

public class JokerManager : MonoBehaviour
{
    public static JokerManager Instance; // singleton my beloved

    [Header("Jokers data")]
    [SerializeField] private List<JokerDataSO> _jokersDataList;

    private List<Joker> _jokersList = new List<Joker>(); // todos los jokers posibles in-game
    private List<Joker> _ownedJokers = new List<Joker>(); // solo los jokers q tengo en mano

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PoolJokers();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void PoolJokers()
    {
        foreach (JokerDataSO data in _jokersDataList)
        {
            GameObject go = Instantiate(data.prefab, transform);
            Joker joker = go.GetComponent<Joker>();
            joker.Initialize(data);
            _jokersList.Add(joker);
            Debug.Log("Joker created: " + joker.jokerData.name);
        }
    }

    public List<Joker> GetJokersList()
    {
        return _jokersList;
    }

    public void JokerBoughtAndAddedToHand(Joker joker)
    {
        _ownedJokers.Add(joker);
    }

    public void ApplyScoreModifiers(float chips, float mult)
    {
        foreach (Joker joker in _ownedJokers)
        {
            joker.ModifyPoints(chips);
            joker.ModifyMult(mult);
        }
    }
}