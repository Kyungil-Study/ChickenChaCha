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
            string emailToAdd = mInputFriendID.text;
            mFriendManager.AddFriendByEmail(emailToAdd, success =>
            {
                Debug.Log(success ? "친구 추가 성공" : "친구 추가 실패");
            });
        }
    
        public void OnAcceptFriendRequest()
        {
            
        }

        public void OnRemoveFriend()
        {
            string emailToRemove = mInputFriendID.text;
            mFriendManager.RemoveFriendByEmail(emailToRemove, success =>
            {
                Debug.Log(success ? "친구 삭제 성공" : "친구 삭제 실패");
            });
        }

        public void OnGetFriendList()
        {
            mFriendManager.GetFriendList(friendEmails =>
            {
                foreach (var email in friendEmails)
                {
                    Debug.Log("친구 이메일: " + email);
                }
            });
        }
    }
}
