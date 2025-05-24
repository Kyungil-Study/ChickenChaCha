using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendResponseMenu : UISingleton<UIFriendResponseMenu>
{
    [Header("Request User Item Prefab")]
    [SerializeField] private UIFriendRequestItem mRequestItemPrefab;
    
    [Space(10)]
    [Header("Request Menu UI Action Refs")]
    [SerializeField] private Button mAcceptButton;
    public Button AcceptButton => mAcceptButton;
    
    [SerializeField] private Button mRejectButton;
    
    List<UIFriendRequestItem> mRequestItemList = new List<UIFriendRequestItem>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
