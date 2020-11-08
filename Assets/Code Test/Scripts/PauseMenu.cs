using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClick);
        restartButton.onClick.AddListener(OnRestartButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnResumeButtonClick()
    {
        GameManager.Instance.TogglePause();
    }

    private void OnRestartButtonClick()
    {
        GameManager.Instance.Restart();
    }

    private void OnQuitButtonClick()
    {
        GameManager.Instance.Quit();
    }
}
