using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIOptionManager : MonoBehaviour
{
    public void HandleRestart()
    {
        SceneManager.LoadScene("TutorialScene");
        Time.timeScale = 1f;
    }

    public void HandleExit()
    {
        Application.Quit();
        Time.timeScale = 1f;
    }
}
