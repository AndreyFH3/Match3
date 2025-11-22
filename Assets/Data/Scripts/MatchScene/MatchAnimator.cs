using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _animationSpeed = 1f;
    [SerializeField] private float _swapSpeed = 5f;
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _appearSpeed = 10f;
    [SerializeField] private float _destroySpeed = 12f;
    [SerializeField] private float _selectSpeed = 15f;
    
    [Header("Scale Settings")]
    [SerializeField] private Vector3 _appearStartScale = Vector3.zero;
    [SerializeField] private Vector3 _appearEndScale = Vector3.one;
    [SerializeField] private Vector3 _selectedScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private Vector3 _normalScale = Vector3.one;
    
    [Header("Destroy Settings")]
    [SerializeField] private Vector3 _destroyEndScale = Vector3.zero;
    private bool _canPlay = false;

    public float AnimationSpeed => _animationSpeed;
    public float MoveSpeed => _moveSpeed;
    public Vector3 AppearStartScale => _appearStartScale;

    
    private static MatchAnimator _instance;
    public static MatchAnimator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MatchAnimator>();
                if (_instance == null)
                {
                    GameObject animatorObject = new GameObject("MatchAnimator");
                    _instance = animatorObject.AddComponent<MatchAnimator>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void CanPlayAnimations() => _canPlay = true;
    public IEnumerator AnimateSwap(Gem gem1, Gem gem2, GemField field1, GemField field2)
    {
        if (gem1 == null || gem2 == null || field1 == null || field2 == null || !_canPlay)
            yield break;
        
        Vector3 startPos1 = gem1.transform.position;
        Vector3 startPos2 = gem2.transform.position;
        Vector3 targetPos1 = field2.transform.position;
        Vector3 targetPos2 = field1.transform.position;
        
        float elapsed = 0f;
        float duration = 1f / _swapSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem1 != null) gem1.transform.position = Vector3.Lerp(startPos1, targetPos1, curve);
            if (gem2 != null) gem2.transform.position = Vector3.Lerp(startPos2, targetPos2, curve);
            
            yield return null;
        }
        
        if (gem1 != null) gem1.transform.position = targetPos1;
        if (gem2 != null) gem2.transform.position = targetPos2;
    }
    
    public IEnumerator AnimateSwapBack(Gem gem1, Gem gem2, GemField field1, GemField field2)
    {
        if (gem1 == null || gem2 == null || field1 == null || field2 == null || !_canPlay)
            yield break;
        
        Vector3 startPos1 = gem1.transform.position;
        Vector3 startPos2 = gem2.transform.position;
        Vector3 targetPos1 = field1.transform.position;
        Vector3 targetPos2 = field2.transform.position;
        
        float elapsed = 0f;
        float duration = 1f / _swapSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem1 != null) gem1.transform.position = Vector3.Lerp(startPos1, targetPos1, curve);
            if (gem2 != null) gem2.transform.position = Vector3.Lerp(startPos2, targetPos2, curve);
            
            yield return null;
        }
        
        if (gem1 != null) gem1.transform.position = targetPos1;
        if (gem2 != null) gem2.transform.position = targetPos2;
    }
    
    public IEnumerator AnimateDestroy(Gem gem)
    {
        if (gem == null || !_canPlay) yield break;
        
        Vector3 startScale = gem.transform.localScale;
        float elapsed = 0f;
        float duration = 1f / _destroySpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem != null)
            {
                gem.transform.localScale = Vector3.Lerp(startScale, _destroyEndScale, curve);
            }
            
            yield return null;
        }
        
        if (gem != null)
        {
            gem.transform.localScale = _destroyEndScale;
        }
    }
    
    public IEnumerator AnimateAppear(Gem gem)
    {
        if (gem == null || !_canPlay) yield break;
        
        gem.transform.localScale = _appearStartScale;
        
        float elapsed = 0f;
        float duration = 1f / _appearSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem != null)
            {
                gem.transform.localScale = Vector3.Lerp(_appearStartScale, _appearEndScale, curve);
            }
            
            yield return null;
        }
        
        if (gem != null)
        {
            gem.transform.localScale = _appearEndScale;
        }
    }
    
    public IEnumerator AnimateMoveToPosition(Gem gem, Vector3 targetPosition)
    {
        if (gem == null || !_canPlay) yield break;
        
        Vector3 startPosition = gem.transform.position;
        float elapsed = 0f;
        float duration = 1f / _moveSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem != null)
            {
                gem.transform.position = Vector3.Lerp(startPosition, targetPosition, curve);
            }
            
            yield return null;
        }
        
        if (gem != null)
        {
            gem.transform.position = targetPosition;
        }
    }
    
    public IEnumerator AnimateFallDown(Gem gem, Vector3 targetPosition)
    {
        if (gem == null || !_canPlay) yield break;
        
        Vector3 startPosition = gem.transform.position;
        Vector3 startScale = gem.transform.localScale;
        float elapsed = 0f;
        float duration = 1f / _moveSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem != null)
            {
                gem.transform.position = Vector3.Lerp(startPosition, targetPosition, curve);
                
                float scaleCurve = Mathf.SmoothStep(0f, 1f, t);
                gem.transform.localScale = Vector3.Lerp(startScale, _appearEndScale, scaleCurve);
            }
            
            yield return null;
        }
        
        if (gem != null)
        {
            gem.transform.position = targetPosition;
            gem.transform.localPosition = Vector3.zero;
            gem.transform.localScale = _appearEndScale;
        }
    }
    
    public IEnumerator AnimateSelect(GemField field)
    {
        if (field?.CurrentGem == null || !_canPlay) yield break;
        
        Gem gem = field.CurrentGem;
        Vector3 startScale = gem.transform.localScale;
        Vector3 targetScale = _selectedScale;
        
        float elapsed = 0f;
        float duration = 1f / _selectSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem != null && field.CurrentGem == gem)
            {
                gem.transform.localScale = Vector3.Lerp(startScale, targetScale, curve);
            }
            
            yield return null;
        }
        
        if (gem != null && field.CurrentGem == gem)
        {
            gem.transform.localScale = targetScale;
        }
    }
    
    public IEnumerator AnimateDeselect(GemField field)
    {
        if (field?.CurrentGem == null || !_canPlay) yield break;
        
        Gem gem = field.CurrentGem;
        Vector3 startScale = gem.transform.localScale;
        Vector3 targetScale = _normalScale;
        
        float elapsed = 0f;
        float duration = 1f / _selectSpeed * _animationSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = Mathf.SmoothStep(0f, 1f, t);
            
            if (gem != null && field.CurrentGem == gem)
            {
                gem.transform.localScale = Vector3.Lerp(startScale, targetScale, curve);
            }
            
            yield return null;
        }
        
        if (gem != null && field.CurrentGem == gem)
        {
            gem.transform.localScale = targetScale;
        }
    }
    
    public IEnumerator AnimateMultipleDestroy(List<Gem> gems)
    {
        if(!_canPlay)
            yield break;    
        List<IEnumerator> animations = new List<IEnumerator>();
        
        foreach (var gem in gems)
        {
            if (gem != null)
            {
                animations.Add(AnimateDestroy(gem));
            }
        }
        
        foreach (var animation in animations)
        {
            StartCoroutine(animation);
        }
        
        float maxDuration = 1f / _destroySpeed * _animationSpeed;
        yield return new WaitForSeconds(maxDuration);
    }
    
    public IEnumerator AnimateMultipleMove(List<(Gem gem, Vector3 target)> moves)
    {
        if (!_canPlay)
            yield break;
        List<IEnumerator> animations = new List<IEnumerator>();
        
        foreach (var move in moves)
        {
            if (move.gem != null)
            {
                animations.Add(AnimateMoveToPosition(move.gem, move.target));
            }
        }
        
        foreach (var animation in animations)
        {
            StartCoroutine(animation);
        }
        
        float maxDuration = 1f / _moveSpeed * _animationSpeed;
        yield return new WaitForSeconds(maxDuration);
    }
    
    public IEnumerator AnimateMultipleAppear(List<Gem> gems)
    {
        if (!_canPlay)
            yield break;
        List<IEnumerator> animations = new List<IEnumerator>();
        
        foreach (var gem in gems)
        {
            if (gem != null)
            {
                animations.Add(AnimateAppear(gem));
            }
        }
        
        foreach (var animation in animations)
        {
            StartCoroutine(animation);
        }
        
        float maxDuration = 1f / _appearSpeed * _animationSpeed;
        yield return new WaitForSeconds(maxDuration);
    }
}

