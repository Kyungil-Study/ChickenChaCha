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

    public UnityEvent<bool> OnAcceptToggleChanged = new UnityEvent<bool>();

    public void Reset()
    {
        OnAcceptToggleChanged.RemoveAllListeners();
        mAcceptToggle.isOn = false;
    }

    public void SetEmailtext(string email)
    {
        mEmailText.text = email;
    }
    
    public void BindListner(UnityAction<bool> callback)
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
        OnAcceptToggleChanged?.Invoke(value);
    }

    
}
