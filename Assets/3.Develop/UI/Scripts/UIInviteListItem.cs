using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInviteListItem : MonoBehaviour
{
    [SerializeField] private Toggle mAcceptToggle;
    public Toggle AcceptToggle => mAcceptToggle;
    [SerializeField] private Text mEmailText;
    public string Email => mEmailText.text;
    [SerializeField] private Text mRoomNameText;
    public string RoomName => mRoomNameText.text;

    public UnityEvent<UIInviteListItem> OnAcceptToggleChanged = new UnityEvent<UIInviteListItem>();

    public void Reset()
    {
        OnAcceptToggleChanged.RemoveAllListeners();
        mAcceptToggle.isOn = false;
    }

    public void SetEmailtext(string email)
    {
        mEmailText.text = email;
    }
    
    public void SetRoomNameText(string roomName)
    {
        mRoomNameText.text = roomName;
    }
    
    public void BindListner(UnityAction<UIInviteListItem> callback)
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
        OnAcceptToggleChanged?.Invoke(this);
    }

    
}
