using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishPresenter : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;

    [SerializeField] private TextMeshProUGUI _pointsText;
    private MatchController _controller;

    private void OnEnable()
    {
        _restartButton.onClick.AddListener(Restart);
        _menuButton.onClick.AddListener(Exit);
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveListener(Restart);
        _menuButton.onClick.RemoveListener(Exit);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Construct(MatchController controller)
    {
        _controller = controller;
        _controller.OnFinish += ShowFinalPoints;
    }

    public void ShowFinalPoints(int points)
    {
        gameObject.SetActive(true);
        if (_pointsText != null)
            _pointsText.text = points.ToString();
    }
}
