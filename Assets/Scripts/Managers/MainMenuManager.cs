using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static void StartGame()
    {
        GameManager.Instance.LoadGame();
    }
    public static void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
