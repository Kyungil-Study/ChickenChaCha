using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIInviteListMenu : UISingleton<UIFriendResponseMenu>
{
    public class ViewItemData
    {
        public string userEmail;
        public string roomName;
    }
    
    [SerializeField] private GameObject mListView;
    [Header("Request User Item Prefab")]
    [SerializeField] private UIInviteListItem mRequestItemPrefab;
    
    [Space(10)]
    [Header("Request Menu UI Action Refs")]
    [SerializeField] private Button mAcceptButton;
    [SerializeField] private Button mRejectButton;
    [SerializeField] private Button mExittButton;
    
    List<UIInviteListItem> mRequestItemList = new List<UIInviteListItem>();
    string mAcceptedInviationRoomName = "";
    
    public event Action<OnInviteEventArgs, Action> OnAcceptButtonClicked;
    public event Action<OnInviteEventArgs, Action> OnRejectButtonClicked;
    public event Action OnExitButtonClicked;
    

    private void Start()
    {
        mAcceptButton.onClick.AddListener(OnClickedAcceptButton);
        mRejectButton.onClick.AddListener(OnClickedRejectButton);
        mExittButton.onClick.AddListener(OnClickedExitButton);
        OnAcceptButtonClicked += PartyInvitationHandler.Instance.AcceptInvite;
        OnRejectButtonClicked += PartyInvitationHandler.Instance.RejectInvite;
    }

    private void OnEnable()
    {
        PartyInvitationHandler.Instance.ShowInvitations(() =>
        {
            UpdateView();
        });
    }

    public void UpdateView()
    {
        Debug.Log($"Response Menu UpdateView called. Request count: {PartyInvitationHandler.Instance.RequestInvitationList.Count}");
        List<ViewItemData> viewItemList = new List<ViewItemData>();
        foreach (var request in PartyInvitationHandler.Instance.RequestInvitationList)
        {
            ViewItemData itemData = new ViewItemData();
            itemData.userEmail = request.From;
            itemData.roomName = request.RoomName;
            viewItemList.Add(itemData);
        }
        UpdateListView(viewItemList);
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
        
        
        // create new items
        foreach (var item in requestItemList)
        {
            UIInviteListItem newItem = Instantiate(mRequestItemPrefab, mListView.transform);
            newItem.Reset();    
            newItem.SetEmailtext(item.userEmail);
            newItem.SetRoomNameText(item.roomName);
            newItem.BindListner(OnAcceptToggleChanged);
            
            // add listener to accept button
            mRequestItemList.Add(newItem);
        }
    }

    void OnAcceptToggleChanged(UIInviteListItem toggleItem)
    {
        Debug.Log($"OnAcceptToggleChanged :::: Toggle  Invitation {toggleItem.AcceptToggle.isOn} {toggleItem.RoomName}");
        foreach (var requestItem in mRequestItemList)
        {
            if (toggleItem != requestItem)
            {
                requestItem.AcceptToggle.isOn = false;
            }
        }

        if (toggleItem.AcceptToggle.isOn == true)
        {
            mAcceptedInviationRoomName = toggleItem.RoomName;
        }
        else
        {
            mAcceptedInviationRoomName = "";
        }
    }

    void OnClickedAcceptButton()
    {
        Debug.Log("Accept button clicked.");
        OnInviteEventArgs args = new OnInviteEventArgs();
        args.InviteRoomName = mAcceptedInviationRoomName;
        OnAcceptButtonClicked?.Invoke(args,
            () =>
            {
                PartyInvitationHandler.Instance.ShowInvitations(() =>
                {
                    UpdateView();
                });
            });
    }

    void OnClickedRejectButton()
    {
        OnInviteEventArgs args = new OnInviteEventArgs();
        args.InviteRoomName = mAcceptedInviationRoomName;
        OnRejectButtonClicked?.Invoke(args,
            () =>
            {
                PartyInvitationHandler.Instance.ShowInvitations(() =>
                {
                    UpdateView();
                });
            });
    }

    void OnClickedExitButton()
    {
        OnExitButtonClicked?.Invoke();
    }
}
