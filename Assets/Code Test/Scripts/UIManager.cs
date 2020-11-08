using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private PauseMenu pauseMenu;

    public MenuFadeComplete OnMenuFadeComplete;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        mainMenu.OnMenuFadeComplete += OnMainMenuFadeComplete;
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        pauseMenu.gameObject.SetActive(newState == GameState.PAUSED);
        Cursor.visible = newState == GameState.PAUSED;
    }

    private void OnMainMenuFadeComplete(bool fadeOut)
    {
        OnMenuFadeComplete?.Invoke(fadeOut);
    }
}
