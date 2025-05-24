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
    public Button SearchButton => mSearchButton;
    [SerializeField] private TMP_Text mSearchLogText;
    public TMP_Text SearchLogText => mSearchLogText;
    
    [SerializeField] private Button mRequestButton;
    public Button RequestButton => mRequestButton;
    [SerializeField] private TMP_Text mRequestLogText;
    public TMP_Text RequestLogText => mRequestLogText;

    public event Action<string> OnSearchButtonClicked;

    private void OnEnable()
    {
        UpdateSearchLog("");
        UpdateRequestLog("");
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
    
    // Start is called before the first frame update
    void Start()
    {
        mSearchButton.onClick.AddListener(OnClickedSearchButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
