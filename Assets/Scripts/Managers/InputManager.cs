using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;


// ... Singleton class that manages all related to player controls... //
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    // ... Singleton Instance ... (cause it's a Jam, baby!)//
    public static InputManager Instance { get; private set; }

    private PlayerInput v_InputSystem;
    private InputAction v_AdvanceStory, v_PauseMenu;

    private bool v_DialogueControls;
    void Awake()
    {
        v_InputSystem = GetComponent<PlayerInput>();
        if (Instance != null)
        {
            Debug.LogWarning("More than one InputManager found in scene!");
        }
        if (Instance == null) Instance = this;

        v_AdvanceStory = v_InputSystem.actions["ContinueDialogue"];
        v_DialogueControls = false;
        v_PauseMenu = v_InputSystem.actions["PauseMenu"];
    }
    void Update()
    {
        if (v_DialogueControls)
        {
            if (v_AdvanceStory.WasPressedThisFrame() && !DialogueManager.Instance.P_IsCoroutineRunning && !DialogueManager.Instance.P_CanFastfowardText && DialogueManager.Instance.IsTextAdvancable)
            {
                DialogueManager.Instance.ContinueStory();
            }
            else if (v_AdvanceStory.WasPressedThisFrame() && DialogueManager.Instance.P_IsCoroutineRunning && DialogueManager.Instance.P_CanFastfowardText)
            {
                DialogueManager.Instance.SkipDialogueCoroutine();
            }
        }
        if (v_AdvanceStory.WasPressedThisFrame())
        {
           UIManager.Instance.DeselectElement();
        }
        if (v_PauseMenu.WasPressedThisFrame())
        {
            UIManager.Instance.InteractMenu();
        }

    }
    public void ActivateDialogueControls(bool active)
    {
        v_DialogueControls = active;    
    }
    public bool AreDialogueControlsActive()
    {
        return this.v_DialogueControls;
    }
}
