using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> normalButtonList;
    [SerializeField]
    private List<GameObject> murderButtonList;
    [SerializeField]
    private ConcreteLocation associatedLocation;
    private void Start()
    {
        DeactivateButtons();
    }
    public void DeactivateButtons()
    {
        for (int i = 0; i < normalButtonList.Count; i++)
        {
            normalButtonList[i].SetActive(false);
        }
        for (int i = 0; i < murderButtonList.Count; i++)
        {
            if (murderButtonList[i].activeInHierarchy)
            {
                murderButtonList[i].SetActive(false);
            }
        }
    }
    public void ActivateNormalRoomButtons()
    {
        for (int i = 0; i < normalButtonList.Count; i++)
        {
            normalButtonList[i].SetActive(true);
        }
    }
    public void ActivateMurderRoomButtons()
    {
        for (int i = 0; i < murderButtonList.Count; i++)
        {
            murderButtonList[i].SetActive(true);
        }
    }
    public void ChangeButtonText(string message, int index)
    {
        TextMeshProUGUI text = normalButtonList[index].GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

    }
    public ConcreteLocation GetConcreteLocation()
    {
        return this.associatedLocation;
    }
}
