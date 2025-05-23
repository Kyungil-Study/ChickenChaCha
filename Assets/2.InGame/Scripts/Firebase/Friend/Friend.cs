using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Friend : MonoBehaviour
{
    public FriendManager mFriendManager;
    [SerializeField] private TMP_InputField mInputFriendID;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendFriendRequest()
    {
        mFriendManager.SendFriendRequest(mInputFriendID.text, success =>
        {
            if (success) Debug.Log("요청 성공!");
            else Debug.Log("요청 실패!");
        });
    }
    
    public void AcceptFriendRequest()
    {
        mFriendManager.AcceptFriendRequest("상대 UID", success =>
        {
            if (success) Debug.Log("수락 완료!");
        });
    }

    public void RemoveFriend()
    {
        mFriendManager.RemoveFriend(mInputFriendID.text, success =>
        {
            if (success) Debug.Log("삭제 완료!");
        });
    }

    public void GetFriendList()
    {
        mFriendManager.GetFriendList(friendList =>
        {
            foreach (var uid in friendList)
            {
                Debug.Log("친구 UID: " + uid);
            }
        });
    }
}
