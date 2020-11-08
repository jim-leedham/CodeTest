using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animation animationController;
    [SerializeField] private AnimationClip fadeOutAnimation;
    [SerializeField] private AnimationClip fadeInAnimation;
    [SerializeField] private Camera menuCamera;

    public MenuFadeComplete OnMenuFadeComplete;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    public void FadeOut()
    {
        animationController.Stop();
        animationController.clip = fadeOutAnimation;
        animationController.Play();

        menuCamera.gameObject.SetActive(false);
    }

    public void OnFadeOutComplete()
    {
        OnMenuFadeComplete?.Invoke(true);
    }

    public void FadeIn()
    {
        animationController.Stop();
        animationController.clip = fadeInAnimation;
        animationController.Play();
    }

    public void OnFadeInComplete()
    {
        OnMenuFadeComplete?.Invoke(false);
        menuCamera.gameObject.SetActive(true);
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        if(previousState == GameState.BOOT)
        {
            if(newState == GameState.RUNNING)
            {
                FadeOut();
            }
        }
        else
        {
            if (newState == GameState.BOOT)
            {
                FadeIn();
            }
        }
    }
}
