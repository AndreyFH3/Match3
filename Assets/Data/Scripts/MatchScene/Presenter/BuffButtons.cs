using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffButtons : MonoBehaviour
{
    [Header("Buton")]
    [SerializeField] private Button _verticalRemoveButton;
    [SerializeField] private Button _horizontlalRemoveButton;
    [SerializeField] private Button _refreshButton;
    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI _verticalAmountText;
    [SerializeField] private TextMeshProUGUI _horizontalAmountText;
    [SerializeField] private TextMeshProUGUI _refreshAmountText;

    private BuffController _buffController;

    private void Start()
    {
        _verticalRemoveButton.onClick.AddListener(OnVerticalBuffButtonClicked);
        _horizontlalRemoveButton.onClick.AddListener(OnHorizontalBuffButtonClicked);
        _refreshButton.onClick.AddListener(OnReshuffleBuffButtonClicked);
        _buffController.OnBuffUsed += UpdateValues;
    }

    private void OnDestroy()
    {
        _verticalRemoveButton.onClick.RemoveListener(OnVerticalBuffButtonClicked);
        _horizontlalRemoveButton.onClick.RemoveListener(OnHorizontalBuffButtonClicked);
        _refreshButton.onClick.RemoveListener(OnReshuffleBuffButtonClicked);
        _buffController.OnBuffUsed -= UpdateValues;
    }

    private void UpdateValues()
    {
        _verticalAmountText.text = _buffController.VerticalAmount.ToString();
        _horizontalAmountText.text = _buffController.HorizontalAmount.ToString();
        _refreshAmountText.text = _buffController.RefreshAmount.ToString();

        _verticalRemoveButton.interactable = _buffController.VerticalAmount > 0;
        _horizontlalRemoveButton.interactable = _buffController.HorizontalAmount > 0;
        _refreshButton.interactable = _buffController.RefreshAmount > 0;

        _verticalRemoveButton.transform.localScale = Vector3.one;
        _horizontlalRemoveButton.transform.localScale = Vector3.one;
        _refreshButton.transform.localScale = Vector3.one;
    }

    public void Construct(BuffController buffController)
    {
        _buffController = buffController;
        UpdateValues();
    }   

    public void OnVerticalBuffButtonClicked()
    {
        if (_buffController.ActiveBuff == BuffController.BuffType.VerticalLine) 
        {
            _verticalRemoveButton.transform.localScale = Vector3.one;
            _buffController.DeactivateBuff();
            return;
        }
        _verticalRemoveButton.transform.localScale = Vector3.one * 1.1f;
        _horizontlalRemoveButton.transform.localScale = Vector3.one;
        _refreshButton.transform.localScale = Vector3.one;
        _buffController.ActivateVerticalLine();
    }

    public void OnHorizontalBuffButtonClicked()
    {
        if (_buffController.ActiveBuff == BuffController.BuffType.HorizontalLine)
        {
            _horizontlalRemoveButton.transform.localScale = Vector3.one;
            _buffController.DeactivateBuff();
            return;
        }
        _verticalRemoveButton.transform.localScale = Vector3.one;
        _horizontlalRemoveButton.transform.localScale = Vector3.one * 1.1f;
        _refreshButton.transform.localScale = Vector3.one;
        _buffController.ActivateHorizontalLine();
    }

    public void OnReshuffleBuffButtonClicked()
    {
        if (_buffController.ActiveBuff == BuffController.BuffType.Reshuffle)
        {
            _refreshButton.transform.localScale = Vector3.one; 
            _buffController.DeactivateBuff();
            return;
        }
        _verticalRemoveButton.transform.localScale = Vector3.one;
        _horizontlalRemoveButton.transform.localScale = Vector3.one;
        _refreshButton.transform.localScale = Vector3.one * 1.1f;
        _buffController.ActivateReshuffle();
    }
}
