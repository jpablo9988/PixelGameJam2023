using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInput v_InputSystem;
    [SerializeField]
    private GameObject m_TextMeshPro;
    private InputAction v_AdvanceStory;
    [SerializeField]
    private GameObject loadingScreen;

    private void Awake()
    {
        v_AdvanceStory = v_InputSystem.actions["ContinueDialogue"];
    }
    void Update()
    {
        if (GameManager.Instance.IsDoneLoading)
        {
            if (!m_TextMeshPro.activeInHierarchy)
                m_TextMeshPro.SetActive(true);
            if (v_AdvanceStory.WasPressedThisFrame())
            {
                loadingScreen.SetActive(false);
            }
        }
    }
}
