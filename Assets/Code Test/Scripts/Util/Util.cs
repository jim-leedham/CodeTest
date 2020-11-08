using UnityEngine;

// -------- Enums --------
public enum GameState
{
    BOOT,
    RUNNING,
    PAUSED
}

public enum PickUpType
{
    ABILITY,
    HEALTH
}

public enum AbilityType
{
    WALLGRAB,
    DOUBLEJUMP,
    ATTACK,
    COUNT
}

// ------ Delegates ------
public delegate void GameStateChanged(GameState previousState, GameState newState);
public delegate void MenuFadeComplete(bool fadeOut);

// ------- Classes -------
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }

    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.Log("[Singleton] Attempting to instantiate a duplicate of a singleton!");
        }
        else
        {
            instance = (T)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
