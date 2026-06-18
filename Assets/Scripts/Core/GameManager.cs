using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _pausePanel;
    private bool _isPaused = false;
    
    [SerializeField] private GameObject _resultPanel;
    
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
            if (!BoardManager.Instance.IsGameOver)
            {
                HandleBackButton();
            }
        }
    }

    private void HandleBackButton()
    {
        if (_isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        _isPaused = true;
        _pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        _isPaused = false;
        _pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void OnRestartGame()
    {
        _isPaused = false;
        _pausePanel.SetActive(false);
        _resultPanel.SetActive(false);
        Time.timeScale = 1f;
        BoardManager.Instance.RestartGame();
    }

    public void ResultDeclared()
    {
        _resultPanel.SetActive(true);
    }
    
    public void OnQuitGameClick()
    {
        var saveData = BoardManager.Instance.CaptureState();
        SaveManager.Save(saveData);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}