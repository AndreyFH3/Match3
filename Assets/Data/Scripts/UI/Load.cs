using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Load : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private float _emitateLoadTime = 5f;
    private float _currentEmitateTime;

    private void Start()
    {
        _currentEmitateTime = _emitateLoadTime;
    }

    void Update()
    {
        _currentEmitateTime -= Time.deltaTime;
        _slider.value = 1 - (_currentEmitateTime / _emitateLoadTime);
        if(_currentEmitateTime <= 0)
        {
            Game.InitGame();
        }
        if(Game.IsInitialized)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
