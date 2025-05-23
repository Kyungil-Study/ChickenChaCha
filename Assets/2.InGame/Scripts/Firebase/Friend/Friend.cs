using TMPro;
using UnityEngine;

namespace _2.InGame.Scripts.Firebase.Friend
{
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

        public void OnSendFriendRequest()
        {
            mFriendManager.SendFriendRequestByEmail(mInputFriendID.text, success =>
            {
                if (success) Debug.Log("요청 성공!");
                else Debug.Log("요청 실패!");
            });
        }
    
        public void OnAcceptFriendRequest()
        {
            mFriendManager.AcceptFriendRequestByEmail("상대 UID", success =>
            {
                if (success) Debug.Log("수락 완료!");
            });
        }

        public void OnRemoveFriend()
        {
            mFriendManager.RemoveFriendByEmail(mInputFriendID.text, success =>
            {
                if (success) Debug.Log("삭제 완료!");
            });
        }

        public void OnGetFriendList()
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
}
