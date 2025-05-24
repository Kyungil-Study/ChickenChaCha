using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendRequestMenu : UISingleton<UIFriendRequestMenu>
{
    
    [SerializeField] private TMP_InputField mSearchInputField;
    
    [SerializeField] private Button mSearchButton;
    public Button SearchButton => mSearchButton;
    [SerializeField] private TMP_Text mSearchLogText;
    public TMP_Text SearchLogText => mSearchLogText;
    
    [SerializeField] private Button mRequestButton;
    public Button RequestButton => mRequestButton;
    [SerializeField] private TMP_Text mRequestLogText;
    public TMP_Text RequestLogText => mRequestLogText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
