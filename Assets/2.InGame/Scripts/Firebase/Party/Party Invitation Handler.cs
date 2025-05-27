// PartyInvitationHandler.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Fusion;
using UnityEngine;

public class PartyInvitationHandler : MonoBehaviour
{
    public class InvitationData
    {
        public string RoomName;
        public string From;
    }
    
    private FirebaseAuth mAuth;
    private FirebaseFirestore mDB;
    //private NetworkRunner mRunner;
    private string mRoomName;
    
    private static PartyInvitationHandler mInstance;
    
    public static PartyInvitationHandler Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<PartyInvitationHandler>();
            }
            return mInstance;
        }
        
    }
    
    private List<InvitationData> mRequestInvitationList = new List<InvitationData>();
    public List<InvitationData> RequestInvitationList => mRequestInvitationList;

    private void Awake()
    {
        mAuth = UserManager.Instance.Auth;
        mDB = UserManager.Instance.DB;
        //mRunner = SessionManager.Instance.NetworkRunner;
    }
    

    public async void ShowInvitations(Action OnCompleteTask)
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            if (myUid == null) return;

            var snapshot = await mDB.Collection("users").Document(myUid)
                .Collection("invitations")
                .GetSnapshotAsync();

            mRequestInvitationList.Clear();
            foreach (var doc in snapshot.Documents)
            {
                string roomName = doc.GetValue<string>("roomName");
                string from = doc.GetValue<string>("from");
                Debug.Log($"🎉 {from} 님이 '{roomName}' 파티에 초대했습니다.");

                var invitationData = new InvitationData();
                invitationData.From = from;
                invitationData.RoomName = doc.GetValue<string>("roomName");
                mRequestInvitationList.Add(invitationData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("초대 목록 불러오기 실패: " + e.Message);
        }
        
        OnCompleteTask?.Invoke();
    }

    public async void AcceptInvite(OnInviteEventArgs args, Action OnComplete)
    {
        try
        {
            Debug.Log($"Accept Invitation {args.InviteRoomName}");
            
            string myUid = mAuth.CurrentUser?.UserId;
            string roomName = args.InviteRoomName;
            if (string.IsNullOrEmpty(roomName) || myUid == null) return;
            
            // 내가 참가할 방을 선택했어
            
            // 방이 어딘지를 조회해야돼

            
            var invitationRef = mDB.Collection("users").Document(myUid)
                .Collection("invitations").Document(roomName);

            await invitationRef.DeleteAsync();
            
            SceneRef sceneRef = SceneRef.FromIndex(SessionManager.IN_GAME_SCENE_INDEX);
            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(sceneRef);
            
            await SessionManager.Instance.NetworkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomName,
                PlayerCount = 1,
                SceneManager = SessionManager.Instance.NetworkSceneManager,
                Scene = sceneInfo
            });

            Debug.Log("✅ 초대 수락 후 입장 시도: " + roomName);
        }
        catch (Exception e)
        {
            Debug.LogError("초대 수락 중 오류: " + e.Message);
        }
        
        OnComplete?.Invoke();
    }

    public async void RejectInvite(OnInviteEventArgs args, Action OnComplete)
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            string roomName = args.InviteRoomName;
            if (string.IsNullOrEmpty(roomName) || myUid == null) return;

            var invitationRef = mDB.Collection("users").Document(myUid)
                .Collection("invitations").Document(roomName);

            await invitationRef.DeleteAsync();
            Debug.Log("❌ 초대 거절 처리 완료: " + roomName);
        }
        catch (Exception e)
        {
            Debug.LogError("초대 거절 중 오류: " + e.Message);
        }
        
        OnComplete?.Invoke();
    }
}
