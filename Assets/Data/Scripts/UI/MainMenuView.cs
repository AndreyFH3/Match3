using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _exitGame;

    private void Start()
    {
        _startGameButton.onClick.AddListener(LoadGameScene);
        _exitGame.onClick.AddListener(ExitGame);
    }

    private void OnDestroy()
    {
        _startGameButton.onClick.RemoveListener(LoadGameScene);
        _exitGame.onClick.RemoveListener(ExitGame);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(); // На случай билда
#endif
    }
}
