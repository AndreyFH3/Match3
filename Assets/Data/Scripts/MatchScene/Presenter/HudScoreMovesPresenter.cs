using UnityEngine;
using TMPro;

public class HudScoreMovesPresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _movesText;
    private MatchController _controller;

    public void Construct(MatchController controller)
    {
        _controller = controller;
        _controller.OnScoreChanged += HandleScoreChanged;
        _controller.OnMovesChanged += HandleMovesChanged;
    }

    private void HandleScoreChanged(int value)
    {
        if (_scoreText != null)
            _scoreText.text = value.ToString();
    }

    private void HandleMovesChanged(int value)
    {
        if (_movesText != null)
            _movesText.text = value.ToString();
    }
}
