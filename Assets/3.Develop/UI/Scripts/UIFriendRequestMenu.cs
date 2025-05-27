using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendRequestMenu : UISingleton<UIFriendRequestMenu>
{
    
    [Header("Search")]
    [SerializeField] private TMP_InputField mSearchInputField;
    
    [SerializeField] private Button mRequestButton;
    [SerializeField] private TMP_Text mRequestLogText;

    public event Action<string> OnRequestClicked;

    private void Awake()
    {
        OnRequestClicked += UserManager.Instance.OnSendFriendRequest;
    }

    private void OnEnable()
    {
        UpdateRequestLog("");
    }
    
    void Start()
    {
        mRequestButton.onClick.AddListener(OnClickedRequestButton);
    }

    public void OnClickedRequestButton()
    {
        OnRequestClicked?.Invoke(mSearchInputField.text);
    }

    
    public void UpdateRequestLog(string log)
    {
        mRequestLogText.text = log;
    }
    
}
