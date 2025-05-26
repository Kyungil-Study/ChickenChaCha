using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendListMenu : MonoBehaviour
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
    [SerializeField] private Button mInviteButton;
    [SerializeField] private Button mExitButton;
    
    List<UIFriendRequestItem> mRequestItemList = new List<UIFriendRequestItem>();
    HashSet<string> mCheckedItemList = new HashSet<string>();
    
    public event Action<List<string>> OnInviteButtonClicked;
    public event Action OnExitButtonClicked;
    
    private void Awake()
    {
        mInviteButton.onClick.AddListener(OnClickedInviteButton);
        mExitButton.onClick.AddListener(OnClickedExitButton);
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
        mCheckedItemList.Clear();
        
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
            mCheckedItemList.Add(email);
        }
        else
        {
            mCheckedItemList.Remove(email);
        }
    }

    void OnClickedInviteButton()
    {
        OnInviteButtonClicked?.Invoke(mCheckedItemList.ToList());
    }

    void OnClickedExitButton()
    {
        OnExitButtonClicked?.Invoke();
    }
}
