using System;
using System.Collections.Generic;

public class BuffController
{
    public event Action OnBuffUsed;

    public enum BuffType
    {
        None,
        VerticalLine,
        HorizontalLine,
        Reshuffle
    }

    private int _horizontalAmount = 3;
    private int _verticalAmount = 3;
    private int _refreshAmount = 3;
    
    private BuffType _activeBuff = BuffType.None;
    private readonly MatchField _field;

    public BuffController(MatchField field)
    {
        _field = field;
    }

    public bool HasActiveBuff => _activeBuff != BuffType.None;
    public BuffType ActiveBuff => _activeBuff;

    public int HorizontalAmount => _horizontalAmount;
    public int VerticalAmount => _verticalAmount;
    public int RefreshAmount => _refreshAmount;

    #region ACTIVATION

    public void ActivateVerticalLine()
    {
        _activeBuff = BuffType.VerticalLine;
    }

    public void ActivateHorizontalLine()
    {
        _activeBuff = BuffType.HorizontalLine;
    }

    public void ActivateReshuffle()
    {
        _activeBuff = BuffType.Reshuffle;
    }
    
    public void DeactivateBuff()
    {
        _activeBuff = BuffType.None;
    }

    #endregion
    #region BUFF
    public bool TryUseBuffOnField(
        GemField target,
        out List<GemField> fieldsToClear,
        out bool reshuffleRequested,
        out BuffType usedBuff)
    {
        fieldsToClear = null;
        reshuffleRequested = false;
        usedBuff = BuffType.None;

        if (_activeBuff == BuffType.None || target == null)
            return false;

        usedBuff = _activeBuff;
        switch (_activeBuff)
        {
            case BuffType.VerticalLine:
                if(_verticalAmount > 0)
                {
                    fieldsToClear = GetColumnFields(target.X);
                    _verticalAmount--;
                }
                break;

            case BuffType.HorizontalLine:
                if(_horizontalAmount > 0)
                {
                    fieldsToClear = GetRowFields(target.Y);
                    _horizontalAmount--;
                }
                break;

            case BuffType.Reshuffle:
                if(_refreshAmount>0)
                { 
                    reshuffleRequested = true;
                    _refreshAmount--;
                }
                break;
        }

        OnBuffUsed?.Invoke();
        _activeBuff = BuffType.None;
        return true;
    }
    #endregion
    #region HELP

    private List<GemField> GetColumnFields(int x)
    {
        List<GemField> result = new List<GemField>();

        int maxY = MatchFinder.GetMaxY(_field);
        for (int y = 0; y <= maxY; y++)
        {
            GemField field = _field.GetField(x, y);
            if (field != null && field.CurrentGem != null)
            {
                result.Add(field);
            }
        }

        return result;
    }

    private List<GemField> GetRowFields(int y)
    {
        List<GemField> result = new List<GemField>();

        int maxX = MatchFinder.GetMaxX(_field);
        for (int x = 0; x <= maxX; x++)
        {
            GemField field = _field.GetField(x, y);
            if (field != null && field.CurrentGem != null)
            {
                result.Add(field);
            }
        }

        return result;
    }

    #endregion
}
