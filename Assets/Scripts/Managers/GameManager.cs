using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
//   game manager - singleton class. //
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private GameObject loadingScreenAcused;
    [SerializeField]
    private GameObject m_TextMeshPro;
    public bool IsDoneLoading;
    private void Awake()
    { 
        if (Instance != null)
        {
            Debug.LogWarning("There are multiple Game Manager instances!");
        }
        else Instance = this;
        SceneManager.LoadSceneAsync((int)SceneIndex.TITLE_SCREEN, LoadSceneMode.Additive);
        IsDoneLoading = false;
    }
    List<AsyncOperation> scenesLoading = new();
    public void LoadGame()
    {
        loadingScreen.SetActive(true);
        AudioManager.Instance.StopMusic();
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.TITLE_SCREEN));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.THE_SHIP, LoadSceneMode.Additive));
        StartCoroutine(GetProgressLoadScene());
    }
    public void EndGame()
    {
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.THE_SHIP));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.THE_END, LoadSceneMode.Additive));
    }
    public void MainMenuEnd()
    {
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.THE_END));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.TITLE_SCREEN, LoadSceneMode.Additive));
    }
    public void MainMenuShip()
    {
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.THE_SHIP));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.TITLE_SCREEN, LoadSceneMode.Additive));
    }
    public IEnumerator GetProgressLoadScene()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                yield return null;
            }
        }
        IsDoneLoading = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
