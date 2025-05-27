// PartyInvitationHandler.cs
using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Fusion;
using UnityEngine;

public class PartyInvitationHandler : MonoBehaviour
{
    private FirebaseAuth mAuth;
    private FirebaseFirestore mDB;
    //private NetworkRunner mRunner;
    private string mRoomName;

    private void Awake()
    {
        mAuth = UserManager.Instance.Auth;
        mDB = UserManager.Instance.DB;
        //mRunner = SessionManager.Instance.NetworkRunner;
    }

    public async void ShowInvitations()
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            if (myUid == null) return;

            var snapshot = await mDB.Collection("users").Document(myUid)
                .Collection("invitations")
                .GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                string roomName = doc.GetValue<string>("roomName");
                mRoomName = roomName;
                string from = doc.GetValue<string>("from");
                Debug.Log($"🎉 {from} 님이 '{roomName}' 파티에 초대했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("초대 목록 불러오기 실패: " + e.Message);
        }
    }

    public async void AcceptInvite()
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            string roomName = mRoomName;
            if (string.IsNullOrEmpty(roomName) || myUid == null) return;
            
            var invitationRef = mDB.Collection("users").Document(myUid)
                .Collection("invitations").Document(roomName);

            await invitationRef.DeleteAsync();
            
            SceneRef sceneRef = SceneRef.FromIndex(2);
            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(sceneRef);
            
            await SessionManager.Instance.NetworkRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomName,
                PlayerCount = 4,
                SceneManager = SessionManager.Instance.NetworkSceneManager,
                Scene = sceneInfo
            });

            Debug.Log("✅ 초대 수락 후 입장 시도: " + roomName);
        }
        catch (Exception e)
        {
            Debug.LogError("초대 수락 중 오류: " + e.Message);
        }
    }

    public async void RejectInvite()
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            string roomName = mRoomName;
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
    }
}
