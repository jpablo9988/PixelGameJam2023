using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private bool hasSelectedElement = false;
    [SerializeField]
    private bool isHovering = false;
    [SerializeField]
    private GameObject currentSelected = null;
    [SerializeField]
    private GameObject delayedElement = null;
    [SerializeField]
    private CanvasGroup worldCanvas;
    [SerializeField]
    private GameObject dialogueScreen;
    [SerializeField]
    private GameObject pauseMenu;

    private bool dialogueControlsActive = false;
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
            Debug.LogWarning("Duplicate instance of UIManager has been found");
        }
    }
    public void ShowPopup(GameObject referingPopup)
    {
        if (!hasSelectedElement)
        {
            currentSelected = referingPopup;
            delayedElement = referingPopup;
            referingPopup.SetActive(true);
            CanvasGroup cg = referingPopup.GetComponent<CanvasGroup>();
            cg.interactable = false;
            cg.alpha = 0;
            LeanTween.alphaCanvas(cg, 1, 0.1f).setOnComplete(delegate ()
            {
                cg.interactable = true;
            });

        }
        isHovering = true;
    }
    public void ActivateWorldCanvas(bool data)
    {
        worldCanvas.interactable = data;
    }
    public void HidePopup(GameObject referingPopup = null)
    {
        if (currentSelected == null && referingPopup != null)
        {
            currentSelected = referingPopup;
            //delayedElement = referingPopup;
        }
        if (!hasSelectedElement && (delayedElement != null))
        {
            GameObject objectToHid = referingPopup;
            CanvasGroup cg = delayedElement.GetComponent<CanvasGroup>();
            LeanTween.alphaCanvas(cg, 0, 0.1f).setOnComplete(delegate ()
            {
                cg.interactable = false;
                //delayedElement.SetActive(false);
                delayedElement = currentSelected;
            });
        }
        isHovering = false;
    }
    public void ShowDialogueScreen()
    {
        dialogueScreen.SetActive(true);
        LeanTween.alpha(dialogueScreen, 1, 0.1f);
    }
    public void HideDialogueScreen()
    {
        dialogueScreen.SetActive(false);
        LeanTween.alpha(dialogueScreen, 0, 0.1f);
    }
    public void SelectElement(GameObject referingPopup)
    {

        if (currentSelected == null)
        {
            currentSelected = referingPopup;
            delayedElement = referingPopup;
        }
        //isHovering = true;
        if ((referingPopup.name.Equals(currentSelected.name)) && hasSelectedElement)
        {
            return;
        }
        UIButtonManager ubm = currentSelected.GetComponent<UIButtonManager>();
        if (hasSelectedElement)
        {
             ubm.DeactivateButtons();
             hasSelectedElement = false;
             HidePopup(currentSelected);
        }
        // -- Make sure popup is activated. --
        ubm = referingPopup.GetComponent<UIButtonManager>();
        CanvasGroup cg = referingPopup.GetComponent<CanvasGroup>();
        ConcreteLocation cl = ubm.GetConcreteLocation();
        currentSelected = referingPopup;
        hasSelectedElement = true;
        currentSelected.SetActive(true);
        cg.interactable = true;
        cg.alpha = 1;
        // -- Show buttons: -- //
        if (cl.ContaintsCorpse)
        {
            ubm.ActivateMurderRoomButtons();
        }
        else
        {
            ubm.ActivateNormalRoomButtons();
            ubm.ChangeButtonText("Check " + cl.GetCurrentCharacter().GetName(), 0);
            ubm.ChangeButtonText("Talk to " + cl.GetCurrentCharacter().GetName(), 1);
            ubm.ChangeButtonText("Investigate " + cl.GetCurrentCharacter().GetName(), 2);
        }

    }
    public void DeselectElement()
    {
        if (!isHovering && hasSelectedElement)
        {
            UIButtonManager ubm = currentSelected.GetComponent<UIButtonManager>();
            ubm.DeactivateButtons();
            hasSelectedElement = false;
            HidePopup();
        }
    }
    public void InteractMenu()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            if (InputManager.Instance.AreDialogueControlsActive())
            {
                dialogueControlsActive = InputManager.Instance.AreDialogueControlsActive();
                InputManager.Instance.ActivateDialogueControls(false);
            }
        }
        else
        {
            if (dialogueControlsActive)
            {
                Debug.Log("Entered here");
                dialogueControlsActive = false;
                InputManager.Instance.ActivateDialogueControls(true);
            }
            pauseMenu.SetActive(false);
        }
    }
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
    public void ReturnToMainMenu()
    {
        Settings.ResetStatsGame();
        GameManager.Instance.MainMenuShip();
    }
}
