using UnityEngine;
using UnityEngine.EventSystems;

public class GemField : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;

    private Gem _gem;
    public int X => _x;
    public int Y => _y;
    public Gem CurrentGem => _gem;
    public System.Action On;
    public void OnPointerClick(PointerEventData eventData)
    {
        On?.Invoke();
    }
    public void SetFieldSelected(Vector3 scale)
    {
        if (CurrentGem != null)
        {
            CurrentGem.transform.localScale = scale;
        }
    }

    public void SetGem(Gem gem) 
    {
        _gem = gem;
        if (_gem != null)
        {
            _gem.transform.SetParent(transform);
            _gem.transform.localPosition = Vector3.zero;
        }
    }

    public void SetGemWithoutCenter(Gem gem)
    {
        _gem = gem;
        if (_gem != null)
        {
            _gem.transform.SetParent(transform);
        }
    }

    public Gem RemoveGem()
    {
        Gem gem = _gem;
        _gem = null;
        return gem;
    }

    public void DestroyGem()
    {
        if (_gem != null)
        {
            Destroy(_gem.gameObject);
            _gem = null;
        }
    }

    public bool IsEmpty()
    {
        return _gem == null;
    }
}
