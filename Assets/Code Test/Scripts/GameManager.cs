using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; private set; } = GameState.BOOT;

    public event GameStateChanged OnGameStateChanged;

    private string currentLevelName = "";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        UIManager.Instance.OnMenuFadeComplete += OnMenuFadeComplete;
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation asyncLevelLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if(asyncLevelLoad != null)
        {
            asyncLevelLoad.completed += OnLevelLoadCompleted;
            currentLevelName = levelName;
        }
    }

    private void OnLevelLoadCompleted(AsyncOperation asyncLevelLoad)
    {
        UpdateState(GameState.RUNNING);
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation asyncLevelUnload = SceneManager.UnloadSceneAsync(levelName);
        if(asyncLevelUnload != null)
        {
            asyncLevelUnload.completed += OnLevelUnloadCompleted;
        }
    }

    private void OnLevelUnloadCompleted(AsyncOperation asyncLevelUnload)
    {
        //TODO
    }

    private void UpdateState(GameState newState)
    {
        GameState previousGameState = State;
        State = newState;
        switch(State)
        {
            case GameState.BOOT:
                {
                    Time.timeScale = 1.0f;
                    break;
                }
            case GameState.RUNNING:
                {
                    Time.timeScale = 1.0f;
                    break;
                }
            case GameState.PAUSED:
                {
                    Time.timeScale = 0.0f;
                    break;
                }
        }

        OnGameStateChanged?.Invoke(previousGameState, State);
    }

    public void StartGame()
    {
        LoadLevel("Main");
    }

    public void Restart()
    {
        UpdateState(GameState.BOOT);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TogglePause()
    {
        UpdateState(State == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
    }

    void Update()
    {
        if (State == GameState.BOOT)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    private void OnMenuFadeComplete(bool fadeOut)
    {
        if(!fadeOut)
        {
            UnloadLevel(currentLevelName);
        }
    }
}
