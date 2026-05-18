using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    /* Hiiii :3
     * This script is made to be a "bag" which allocates the current game to be played
     * idk if it's the best way to do it, but it sure is super readable
     * It's simple, the player clicks on the game he'll play, and this script takes it and loads it :p
     */

    public static GlobalManager instance;
    [NonSerialized] public GameDefinitionSO currentGame;
    [SerializeField] private string _sceneToLoadAfterLoading;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.LoadScene(_sceneToLoadAfterLoading);
    }

    private void OnEnable()
    {
        //EnemyTrigger.OnSwitchToFight += OnEnemyTriggered_SwitchToFightScene;
        // This should be a subscription to the player clicking the new game to load
    }

    private void OnDisable()
    {
        //EnemyTrigger.OnSwitchToFight -= OnEnemyTriggered_SwitchToFightScene;
        // Same as OnEnable but with -=
    }

    public void OnEnemyTriggered_SwitchToFightScene(GameDefinitionSO gameDefinitionSO)
    {
        currentGame = gameDefinitionSO;
    }
}