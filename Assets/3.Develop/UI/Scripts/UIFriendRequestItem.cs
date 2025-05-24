using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFriendRequestItem : MonoBehaviour
{
    [SerializeField] private Toggle mAcceptToggle;
    [SerializeField] private Text mEmailText;

    public UnityEvent<bool, string> OnAcceptToggleChanged = new UnityEvent<bool, string>();

    public void Reset()
    {
        OnAcceptToggleChanged.RemoveAllListeners();
        mAcceptToggle.isOn = false;
    }

    public void SetEmailtext(string email)
    {
        mEmailText.text = email;
    }
    
    public void BindListner(UnityAction<bool,string> callback)
    {
        OnAcceptToggleChanged.RemoveAllListeners();
        mAcceptToggle.isOn = false;
        OnAcceptToggleChanged.AddListener(callback);
    }
    
    private void Awake()
    {
        mAcceptToggle.onValueChanged.AddListener(OnAcceptToggleValueChanged);
    }

    private void OnAcceptToggleValueChanged(bool value)
    {
        OnAcceptToggleChanged?.Invoke(value, mEmailText.text);
    }

    
}
