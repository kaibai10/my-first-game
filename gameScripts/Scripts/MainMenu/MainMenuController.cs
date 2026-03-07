using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public void StartGame() 
    {
        SceneManager.LoadScene("OrganizeTeamScreen");
    }

    public void ExitGame() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#else
        Application.Quit();
#endif
    }
}
