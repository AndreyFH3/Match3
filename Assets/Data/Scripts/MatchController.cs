using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [SerializeField] private int _moves = 30;
    [SerializeField] private List<Gem> _allGems;

    private MatchField _field;
    private MatchAnimator _animator;
    private BuffController _buffController;
    private GemField _selectedField;
    private int _score;
    private bool _isProcessing;    
    private bool _isGameStarted;   

    public event Action<int> OnScoreChanged;
    public event Action<int> OnMovesChanged;
    public event Action<int> OnFinish;
    public void Construct(MatchField field, BuffController buffController)
    {
        _field = field;
        _buffController = buffController;
    }
    #region GAME INIT
    public void Init()
    {
        if (_animator == null)
        {
            _animator = FindObjectOfType<MatchAnimator>();
            if (_animator == null)
            {
                _animator = MatchAnimator.Instance;
            }
        }

        GameFieldInit();
        SubscribeToFields();
        OnScoreChanged?.Invoke(_score);
        OnMovesChanged?.Invoke(_moves);
    }

    private void GameFieldInit()
    {
        _isProcessing = true;
        _isGameStarted = false;

        InitBoardWithoutStartingMatches();

        _animator.CanPlayAnimations();
        _isProcessing = false;
        _isGameStarted = true;
    }

    private void SubscribeToFields()
    {
        foreach (var field in _field.Fields)
        {
            if (field == null)
                continue;

            GemField localField = field;
            localField.On = () => OnFieldClicked(localField);

        }
    }

    private void InitBoardWithoutStartingMatches()
    {
        int maxX = MatchFinder.GetMaxX(_field);
        int maxY = MatchFinder.GetMaxY(_field);

        foreach (var f in _field.Fields)
        {
            if (f != null && f.CurrentGem != null)
            {
                Destroy(f.CurrentGem.gameObject);
                f.RemoveGem();
            }
        }

        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                GemField field = _field.GetField(x, y);
                if (field == null)
                    continue;

                Gem placedGem = null;
                int safetyCounter = 0;

                while (true)
                {
                    safetyCounter++;
                    if (safetyCounter > 50)
                    {
                        Debug.LogWarning("InitBoardWithoutStartingMatches: safety break, can't find non-matching gem");
                        break;
                    }

                    Gem prefab = _allGems[UnityEngine.Random.Range(0, _allGems.Count)];
                    Gem newGem = Instantiate(prefab);
                    newGem.transform.localScale = Vector3.one;
                    
                    field.SetGem(newGem);

                    var matches = MatchFinder.FindAllMatches(_field);

                    if (matches == null || matches.Count == 0)
                    {
                        placedGem = newGem;
                        break;
                    }
                    else
                    {
                        field.RemoveGem();
                        Destroy(newGem.gameObject);
                    }
                }

                if (placedGem == null && field.IsEmpty() && _allGems.Count > 0)
                {
                    Gem fallback = Instantiate(_allGems[0]);
                    fallback.transform.localScale = Vector3.one;
                    field.SetGem(fallback);
                }
            }
        }
    }

    #endregion

    #region INPUT

    private void OnFieldClicked(GemField field)
    {
        if (!_isGameStarted || _isProcessing || field == null || field.CurrentGem == null || _moves <= 0)
            return;

        if (_buffController != null &&
        _buffController.TryUseBuffOnField(
            field,
            out var fieldsToClear,
            out bool reshuffleRequested,
            out BuffController.BuffType usedBuff))
        {

            if (reshuffleRequested)
            {
                StartCoroutine(HandleMatchesAndCascades(_field.Fields));
            }
            else if (fieldsToClear != null && fieldsToClear.Count > 0)
            {

                StartCoroutine(HandleMatchesAndCascades(fieldsToClear));
            }
            return;
        }

        if (_selectedField == null)
        {
            _selectedField = field;
            StartCoroutine(_animator.AnimateSelect(_selectedField));
        }
        else if (_selectedField == field)
        {
            StartCoroutine(_animator.AnimateDeselect(_selectedField));
            _selectedField = null;
        }
        else if (AreNeighbors(_selectedField, field))
        {
            StartCoroutine(_animator.AnimateDeselect(_selectedField));
            StartCoroutine(TrySwapGemsCoroutine(_selectedField, field));
            _selectedField = null;
        }
        else
        {
            StartCoroutine(_animator.AnimateDeselect(_selectedField));
            _selectedField = field;
            StartCoroutine(_animator.AnimateSelect(_selectedField));
        }
    }

    private bool AreNeighbors(GemField field1, GemField field2)
    {
        if (field1 == null || field2 == null)
            return false;

        int dx = Mathf.Abs(field1.X - field2.X);
        int dy = Mathf.Abs(field1.Y - field2.Y);

        bool isNeighbor = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);

        return isNeighbor;
    }

    #endregion

    #region SWAP & MATCH HANDLING
    private IEnumerator TrySwapGemsCoroutine(GemField field1, GemField field2)
    {
        _isProcessing = true;

        Gem gem1 = field1.RemoveGem();
        Gem gem2 = field2.RemoveGem();

        if (gem1 == null || gem2 == null)
        {
            if (gem1 != null) field1.SetGem(gem1);
            if (gem2 != null) field2.SetGem(gem2);
            _isProcessing = false;
            yield break;
        }

        Vector3 startPos1 = field1.transform.position;
        Vector3 startPos2 = field2.transform.position;

        gem1.transform.SetParent(null);
        gem2.transform.SetParent(null);
        gem1.transform.position = startPos1;
        gem2.transform.position = startPos2;

        yield return StartCoroutine(_animator.AnimateSwap(gem1, gem2, field1, field2));

        field2.SetGem(gem1);
        field1.SetGem(gem2);

        List<GemField> matches = MatchFinder.FindAllMatches(_field);

        if (matches.Count > 0)
        {
            _moves--;
            OnMovesChanged?.Invoke(_moves);
            yield return StartCoroutine(_animator.AnimateDeselect(field1));
            yield return StartCoroutine(_animator.AnimateDeselect(field2));
            yield return StartCoroutine(HandleMatchesAndCascades(matches));
        }
        else
        {
            yield return StartCoroutine(SwapBackCoroutine(field1, field2));
            _isProcessing = false;
        }
        if (_moves <= 0)
            OnFinish?.Invoke(_score);
        else
            HasAnyPossibleMove(_field);
    }

    private IEnumerator SwapBackCoroutine(GemField field1, GemField field2)
    {


        Gem gemFromField1 = field1.RemoveGem();
        Gem gemFromField2 = field2.RemoveGem(); 

        if (gemFromField1 != null)
            gemFromField1.transform.SetParent(null);

        if (gemFromField2 != null)
            gemFromField2.transform.SetParent(null);

        yield return StartCoroutine(_animator.AnimateSwapBack(gemFromField2, gemFromField1, field1, field2));

        field1.SetGem(gemFromField2);
        field2.SetGem(gemFromField1);

        yield return StartCoroutine(_animator.AnimateDeselect(field1));
        yield return StartCoroutine(_animator.AnimateDeselect(field2));
    }

    private IEnumerator HandleMatchesAndCascades(List<GemField> initialMatches)
    {
        _isProcessing = true;

        List<GemField> currentMatches = initialMatches;

        while (currentMatches != null && currentMatches.Count > 0)
        {
            currentMatches = currentMatches
                .Where(f => f != null)
                .Distinct()
                .ToList();

            if (_isGameStarted)
            {
                int points = CalculateScore(currentMatches);
                _score += points;
                OnScoreChanged?.Invoke(_score);
            }

            yield return StartCoroutine(RemoveMatchedGemsCoroutine(currentMatches));

            yield return StartCoroutine(ShiftGemsDownCoroutine());

            yield return StartCoroutine(FillEmptyFieldsWithNewGemsCoroutine());

            currentMatches = MatchFinder.FindAllMatches(_field);
        }

        _isProcessing = false;
    }

    private IEnumerator RemoveMatchedGemsCoroutine(List<GemField> matches)
    {
        List<Gem> gemsToDestroy = new List<Gem>();

        foreach (var field in matches)
        {
            if (field != null && field.CurrentGem != null)
            {
                Gem gem = field.CurrentGem;
                gemsToDestroy.Add(gem);
                field.RemoveGem();
                gem.transform.SetParent(null);
            }
        }

        if (gemsToDestroy.Count > 0)
        {
            yield return StartCoroutine(_animator.AnimateMultipleDestroy(gemsToDestroy));

            foreach (var gem in gemsToDestroy)
            {
                if (gem != null)
                {
                    Destroy(gem.gameObject);
                }
            }
        }
    }

    #endregion

    #region SHIFT & FILL
    private IEnumerator ShiftGemsDownCoroutine()
    {
        List<(Gem gem, Vector3 target)> moves = new List<(Gem, Vector3)>();

        int maxX = MatchFinder.GetMaxX(_field);
        int maxY = MatchFinder.GetMaxY(_field);

        for (int x = 0; x <= maxX; x++)
        {
            List<Gem> gemsInColumn = new List<Gem>();
            for (int y = maxY; y >= 0; y--)
            {
                GemField field = _field.GetField(x, y);
                if (field != null && field.CurrentGem != null)
                {
                    Gem gem = field.RemoveGem();
                    gem.transform.SetParent(null);
                    gemsInColumn.Add(gem);
                }
            }

            int gemIndex = 0;
            for (int y = maxY; y >= 0; y--)
            {
                GemField field = _field.GetField(x, y);
                if (field == null) continue;

                if (gemIndex < gemsInColumn.Count)
                {
                    Gem gem = gemsInColumn[gemIndex];
                    Vector3 targetPos = field.transform.position;
                    moves.Add((gem, targetPos));

                    field.SetGemWithoutCenter(gem);
                    gemIndex++;
                }
                else
                {
                    if (field.CurrentGem != null)
                    {
                        field.RemoveGem();
                    }
                }
            }
        }

        if (moves.Count > 0)
        {
            yield return StartCoroutine(_animator.AnimateMultipleMove(moves));

            foreach (var move in moves)
            {
                if (move.gem != null)
                {
                    move.gem.transform.position = move.target;
                    move.gem.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    private IEnumerator FillEmptyFieldsWithNewGemsCoroutine()
    {
        int maxX = MatchFinder.GetMaxX(_field);
        int maxY = MatchFinder.GetMaxY(_field);

        for (int x = 0; x <= maxX; x++)
        {
            List<(GemField field, int emptyIndex)> emptyFieldsInColumn = new List<(GemField, int)>();
            int emptyCount = 0;

            for (int y = maxY; y >= 0; y--)
            {
                GemField field = _field.GetField(x, y);
                if (field != null && field.IsEmpty())
                {
                    emptyCount++;
                    emptyFieldsInColumn.Add((field, emptyCount));
                }
            }

            for (int i = 0; i < emptyFieldsInColumn.Count; i++)
            {
                var (field, emptyIndex) = emptyFieldsInColumn[i];

                Gem selectedGem = _allGems[UnityEngine.Random.Range(0, _allGems.Count)];
                Gem newGem = Instantiate(selectedGem);

                float spawnOffset = emptyIndex * 1.2f;
                Vector3 spawnPos = field.transform.position + Vector3.up * spawnOffset;

                newGem.transform.position = spawnPos;

                field.SetGem(newGem);

                StartCoroutine(_animator.AnimateFallDown(newGem, field.transform.position));
            }
                yield return null;
        }
        float fallDuration = 1f / _animator.MoveSpeed * _animator.AnimationSpeed;
        yield return new WaitForSeconds(fallDuration);
    }

    #endregion

    #region CHECK MOVES
    public bool HasAnyPossibleMove(MatchField field)
    {
        int maxX = MatchFinder.GetMaxX(_field);
        int maxY = MatchFinder.GetMaxY(_field);

        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                GemField current = field.GetField(x, y);
                if (current == null || current.CurrentGem == null)
                    continue;

                if (x < maxX)
                {
                    GemField right = field.GetField(x + 1, y);
                    if (right != null && right.CurrentGem != null)
                    {
                        if (SwapCreatesMatch(field, current, right))
                            return true;
                    }
                }

                if (y < maxY)
                {
                    GemField up = field.GetField(x, y + 1);
                    if (up != null && up.CurrentGem != null)
                    {
                        if (SwapCreatesMatch(field, current, up))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    private bool SwapCreatesMatch(MatchField field, GemField field1, GemField field2)
    {
        if (field1 == null || field2 == null)
            return false;
        if (field1.CurrentGem == null || field2.CurrentGem == null)
            return false;

        Gem gem1 = field1.RemoveGem();
        Gem gem2 = field2.RemoveGem();

        if (gem1 == null || gem2 == null)
        {
            if (gem1 != null) field1.SetGem(gem1);
            if (gem2 != null) field2.SetGem(gem2);
            return false;
        }

        field1.SetGem(gem2);
        field2.SetGem(gem1);

        bool hasMatch = MatchFinder.FindAllMatches(field).Count > 0;

        Gem back2 = field1.RemoveGem(); 
        Gem back1 = field2.RemoveGem();

        field1.SetGem(back1);
        field2.SetGem(back2);

        return hasMatch;
    }

    #endregion

    #region SCORING

    private int CalculateScore(List<GemField> matches)
    {
        if (matches == null || matches.Count < 3)
            return 0;

        bool hasSquare = CheckForSquare(matches);
        bool hasCorner = CheckForCorner(matches);
        bool hasFourInRow = CheckForFourInRow(matches);

        int baseScore = matches.Count * 10;

        if (hasSquare)
            baseScore += 50;

        if (hasCorner)
            baseScore += 30;

        if (hasFourInRow)
            baseScore += 20;

        return baseScore;
    }

    private bool CheckForSquare(List<GemField> matches)
    {
        HashSet<(int, int)> positions = new HashSet<(int, int)>();
        foreach (var field in matches)
        {
            positions.Add((field.X, field.Y));
        }

        foreach (var field in matches)
        {
            int x = field.X;
            int y = field.Y;

            if (positions.Contains((x, y)) &&
                positions.Contains((x + 1, y)) &&
                positions.Contains((x, y + 1)) &&
                positions.Contains((x + 1, y + 1)))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckForCorner(List<GemField> matches)
    {
        if (matches.Count < 4)
            return false;

        HashSet<(int, int)> positions = new HashSet<(int, int)>();
        foreach (var field in matches)
        {
            positions.Add((field.X, field.Y));
        }

        foreach (var field in matches)
        {
            int x = field.X;
            int y = field.Y;

            if ((positions.Contains((x - 1, y)) && positions.Contains((x - 2, y)) &&
                 positions.Contains((x, y - 1)) && positions.Contains((x, y - 2))) ||
                (positions.Contains((x + 1, y)) && positions.Contains((x + 2, y)) &&
                 positions.Contains((x, y - 1)) && positions.Contains((x, y - 2))) ||
                (positions.Contains((x - 1, y)) && positions.Contains((x - 2, y)) &&
                 positions.Contains((x, y + 1)) && positions.Contains((x, y + 2))) ||
                (positions.Contains((x + 1, y)) && positions.Contains((x + 2, y)) &&
                 positions.Contains((x, y + 1)) && positions.Contains((x, y + 2))))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckForFourInRow(List<GemField> matches)
    {
        if (matches.Count < 4)
            return false;

        HashSet<(int, int)> positions = new HashSet<(int, int)>();
        foreach (var field in matches)
        {
            positions.Add((field.X, field.Y));
        }

        foreach (var field in matches)
        {
            int x = field.X;
            int y = field.Y;

            int horizontalCount = 1;
            for (int i = 1; i < 5; i++)
            {
                if (positions.Contains((x + i, y)))
                    horizontalCount++;
                else
                    break;
            }
            if (horizontalCount >= 4)
                return true;

            int verticalCount = 1;
            for (int i = 1; i < 5; i++)
            {
                if (positions.Contains((x, y + i)))
                    verticalCount++;
                else
                    break;
            }
            if (verticalCount >= 4)
                return true;
        }

        return false;
    }

    #endregion
}
