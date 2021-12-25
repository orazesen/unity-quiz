using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class responsible from confirmations
public class ConfirmationController : MonoBehaviour
{
    // Unity Objects
    public GameObject confirmationPanel;
    public Text text;
    public Button yesBtn;
    public Button noBtn;
    public Button okBtn;
    public AudioSource btnClick;

    // Opens alert panel with single button
    public void ShowSingleButton(string _message)
    {
        confirmationPanel.SetActive(true);
        text.text = _message;

        yesBtn.gameObject.SetActive(false);

        noBtn.gameObject.SetActive(false);

        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(() => confirmationPanel.SetActive(false));
        okBtn.onClick.AddListener(btnClick.Play);

        okBtn.GetComponentInChildren<Text>().text = "Ok";
        okBtn.gameObject.SetActive(true);
    }

    // Opens two button confirmation
    public void ShowMessage(string _message, Action _yesBtn, string yesString, Action _noBtn, string noString)
    {
        confirmationPanel.SetActive(true);
        text.text = _message;

        yesBtn.gameObject.SetActive(true);

        noBtn.gameObject.SetActive(true);

        yesBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.AddListener(_yesBtn.Invoke);
        yesBtn.onClick.AddListener(() => confirmationPanel.SetActive(false));
        yesBtn.onClick.AddListener(btnClick.Play);

        noBtn.onClick.RemoveAllListeners();
        if (_noBtn != null)
        {
            noBtn.onClick.AddListener(_noBtn.Invoke);
        }        
        noBtn.onClick.AddListener(() => confirmationPanel.SetActive(false));

        yesBtn.GetComponentInChildren<Text>().text = yesString;
        noBtn.GetComponentInChildren<Text>().text = noString;
        noBtn.onClick.AddListener(btnClick.Play);
        okBtn.gameObject.SetActive(false);
    }
}
