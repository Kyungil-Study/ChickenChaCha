using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIFriendResponseMenu : UISingleton<UIFriendResponseMenu>
{
    public class ViewItemData
    {
        public string userEmail;
    }
    
    [SerializeField] private GameObject mListView;
    [Header("Request User Item Prefab")]
    [SerializeField] private UIFriendRequestItem mRequestItemPrefab;
    
    [Space(10)]
    [Header("Request Menu UI Action Refs")]
    [SerializeField] private Button mAcceptButton;
    [SerializeField] private Button mRejectButton;
    
    List<UIFriendRequestItem> mRequestItemList = new List<UIFriendRequestItem>();
    HashSet<string> mAcceptedItemList = new HashSet<string>();
    
    public event Action<List<string>> OnAcceptButtonClicked;
    public event Action OnExitButtonClicked;

    private void Awake()
    {
        mAcceptButton.onClick.AddListener(OnClickedAcceptButton);
        mRejectButton.onClick.AddListener(OnClickedExitButton);
    }

    public void UpdateListView(List<ViewItemData> requestItemList)
    {
        // destroy all items
        foreach (var item in mRequestItemList)
        {
            Destroy(item.gameObject);
        }
        // clear cached items
        mRequestItemList.Clear();
        mAcceptedItemList.Clear();
        
        // create new items
        foreach (var item in requestItemList)
        {
            UIFriendRequestItem newItem = Instantiate(mRequestItemPrefab, mListView.transform);
            newItem.Reset();    
            newItem.SetEmailtext(item.userEmail);
            newItem.BindListner(OnAcceptToggleChanged);
            
            // add listener to accept button
            mRequestItemList.Add(newItem);
        }
    }

    void OnAcceptToggleChanged(bool isOn, string email)
    {
        if (isOn)
        {
            mAcceptedItemList.Add(email);
        }
        else
        {
            mAcceptedItemList.Remove(email);
        }
    }

    void OnClickedAcceptButton()
    {
        OnAcceptButtonClicked?.Invoke(mAcceptedItemList.ToList());
    }

    void OnClickedExitButton()
    {
        OnExitButtonClicked?.Invoke();
    }
}
