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
    
    [SerializeField] private Button mSearchButton;
    [SerializeField] private TMP_Text mSearchLogText;
    
    [SerializeField] private Button mRequestButton;
    [SerializeField] private TMP_Text mRequestLogText;

    public event Action<string> OnSearchButtonClicked;
    public event Action<string> OnRequestClicked;

    private void Awake()
    {
        OnRequestClicked += UserManager.Instance.OnSendFriendRequest;
    }

    private void OnEnable()
    {
        UpdateSearchLog("");
        UpdateRequestLog("");
    }
    
    void Start()
    {
        mSearchButton.onClick.AddListener(OnClickedSearchButton);
        mRequestButton.onClick.AddListener(OnClickedRequestButton);
    }

    public void OnClickedRequestButton()
    {
        OnRequestClicked?.Invoke(mSearchInputField.text);
    }

    void OnClickedSearchButton()
    {
        string searchText = mSearchInputField.text;
        OnSearchButtonClicked?.Invoke(searchText);
    }
    
    public void UpdateSearchLog(string log)
    {
        mSearchLogText.text = log;
    }

    public void UpdateRequestLog(string log)
    {
        mRequestLogText.text = log;
    }
    
}
