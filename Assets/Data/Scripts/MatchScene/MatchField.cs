using System.Collections.Generic;
using UnityEngine;

public class MatchField : MonoBehaviour
{
    [SerializeField] private List<GemField> _fields;
    
    public int FiledsAmount => _fields.Count;
    public List<GemField> Fields => _fields;

    public GemField GetField(int x, int y)
    {
        return _fields.Find(field => field.X == x && field.Y == y);
    }

    public void SetField(Gem gem, int index)
    {
        if (index >= _fields.Count || index < 0) 
            return;

        _fields[index].SetGem(gem);
    }
}
