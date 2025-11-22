using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartChecker : MonoBehaviour
{
    private void Start()
    {
        if(Game.IsInitialized == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Bootstrap");
        }
    }
}
