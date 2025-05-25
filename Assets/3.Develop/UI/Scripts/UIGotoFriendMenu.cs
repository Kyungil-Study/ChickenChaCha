using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIGotoFriendMenu : UISingleton<UIGotoFriendMenu>
{
    
    [SerializeField] private Button mFriendRequestMenuButton;
    [SerializeField] private Button mFriendResponseMenuButton;
    [SerializeField] private Button mFriendListMenuButton;
    
    [SerializeField] public UnityEvent onClickedGotoFriendRequestMenu = new UnityEvent();
    [SerializeField] public UnityEvent onClickedGotoFriendResponseMenu = new UnityEvent();
    [SerializeField] public UnityEvent onClickedGotoFriendListMenu = new UnityEvent();

    private void Awake()
    {
        mFriendRequestMenuButton.onClick.AddListener(OnClickedRequestButton);
        mFriendResponseMenuButton.onClick.AddListener(OnClickedResponseButton);
        mFriendListMenuButton.onClick.AddListener(OnClickedListButton);
    }

    void OnClickedRequestButton()
    {
        onClickedGotoFriendRequestMenu?.Invoke();
    }

    void OnClickedResponseButton()
    {
        onClickedGotoFriendResponseMenu?.Invoke();
    }
    
    void OnClickedListButton()
    {
        onClickedGotoFriendListMenu?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
