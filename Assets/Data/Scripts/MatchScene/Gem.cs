using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private GemType _type;
    public GemType Type => _type;
}
