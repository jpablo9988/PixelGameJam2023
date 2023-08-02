using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Journal : MonoBehaviour
{
    public List<JournalEntry> Journals { get; private set; }
    public JournalEntry SelectedJournal { get; private set; }

    [SerializeField]
    private GameObject scrollView;
    [SerializeField]
    private GameObject journalPanel;
    private bool dialogueControlsActive = false;
    private List<Button> buttons;
    void Awake()
    {
        Journals = new();
        journalPanel.SetActive(false);
    }
    private void Start()
    {
        buttons = new(scrollView.GetComponentsInChildren<Button>());
        DeactivateAllButtons(false);
    }
    public void AddJournalEntry (JournalEntry data)
    {
        Journals.Add(data);
    }
    int count = 0;
    private void RefreshJournal()
    {
        journalPanel.SetActive(true);
        // .. Deactivate relevant inputs : World UI & Dialogue, if it was active... //
        if (InputManager.Instance.AreDialogueControlsActive())
        {
            InputManager.Instance.ActivateDialogueControls(false);
            dialogueControlsActive = InputManager.Instance.AreDialogueControlsActive();
        }
        UIManager.Instance.ActivateWorldCanvas(false);
        
        for( int i = 0; i < Journals.Count; i++)
        {
            if (Journals[i].Obtained)
            {
                Debug.Log(count);
                if (count < buttons.Count)
                {
                    buttons[count].gameObject.SetActive(true);
                    TextMeshProUGUI tmPro = buttons[count].GetComponentInChildren<TextMeshProUGUI>();
                    tmPro.text = Journals[i].GetWrittenJournal();
                    count++;
                }
            }
        }
    }
    public void InteractJournal()
    {
        if (!journalPanel.activeInHierarchy)
        {
            RefreshJournal();
        }
        else
        {
            if (dialogueControlsActive)
            {
                dialogueControlsActive = false;
                InputManager.Instance.ActivateDialogueControls(true);
            }
            UIManager.Instance.ActivateWorldCanvas(true);
            for (int i = 0; i < count; i++)
            {
                buttons[count].gameObject.SetActive(false);
            }
            count = 0;
            journalPanel.SetActive(false);
        }
    }
    private void DeactivateAllButtons(bool data)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(data);
        }
    }
}
