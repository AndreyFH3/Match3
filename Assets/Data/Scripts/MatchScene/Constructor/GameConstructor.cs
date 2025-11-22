using UnityEngine;

public class GameConstructor : MonoBehaviour
{
    [SerializeField] private MatchController _matchController;
    [SerializeField] private HudScoreMovesPresenter movesScorePresenter;
    [SerializeField] private FinishPresenter _finishPresenter;
    [SerializeField] private MatchField _matchField;
    [SerializeField] private BuffButtons _buffButtons;
    
    public void Awake()
    {
        var BuffController = new BuffController(_matchField);   
        
        _matchController.Construct(_matchField, BuffController);
        _buffButtons.Construct(BuffController);
        movesScorePresenter.Construct(_matchController);
        _finishPresenter.Construct(_matchController);
        _matchController.Init();
    }   
}
