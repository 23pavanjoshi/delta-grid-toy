using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        yield return null;

        if (SaveManager.HasSave())
        {
            var saveData = SaveManager.Load();

            if (saveData == null || saveData.cardStates.Count == 0)
            {
                BoardManager.Instance.StartNewGame();
                yield break;
            }

            yield return BoardManager.Instance.RestoreState(saveData);
        }
        else
        {
            BoardManager.Instance.StartNewGame();
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (!paused) return;

        var saveData = BoardManager.Instance.CaptureState();
        SaveManager.Save(saveData);
    }

    private void OnApplicationQuit()
    {
        Debug.Log($"[GameManager] OnApplicationQuit ==> {BoardManager.Instance.IsGameOver}");
        if (BoardManager.Instance.IsGameOver)
        {
            SaveManager.DeleteSave();
        }
        else
        {
            var saveData = BoardManager.Instance.CaptureState();
            SaveManager.Save(saveData);   
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackButton();
        }
    }

    private void HandleBackButton()
    {
        var saveData = BoardManager.Instance.CaptureState();
        SaveManager.Save(saveData);
        Application.Quit();
    }
}