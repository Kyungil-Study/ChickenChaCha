// FusionPartyManager.cs - Fusion 기반 파티 초대 시스템 + Firebase 연동

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using Fusion;
using Fusion.Sockets;
using UnityEngine.Serialization;

public class FusionPartyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [FormerlySerializedAs("inputRoomName")]
    [Header("파티 UI")]
    [SerializeField] private TMP_InputField mInputRoomName;
    [FormerlySerializedAs("inputInviteFriend")] [SerializeField] private TMP_InputField mInputInviteFriend;
    [FormerlySerializedAs("inputAcceptRoomName")] [SerializeField] private TMP_InputField mInputAcceptRoomName;
    [FormerlySerializedAs("buttonCreateParty")] [SerializeField] private Button mButtonCreateParty;
    [FormerlySerializedAs("buttonInviteFriend")] [SerializeField] private Button mButtonInviteFriend;
    [FormerlySerializedAs("buttonShowInvites")] [SerializeField] private Button mButtonShowInvites;
    [FormerlySerializedAs("buttonAcceptInvite")] [SerializeField] private Button mButtonAcceptInvite;
    [FormerlySerializedAs("buttonRejectInvite")] [SerializeField] private Button mButtonRejectInvite;

    private FirebaseAuth mAuth;
    private FirebaseFirestore mDB;
    private NetworkRunner mRunner;

    private void Start()
    {
        mAuth = FirebaseAuth.DefaultInstance;
        mDB = FirebaseFirestore.DefaultInstance;

        mRunner = gameObject.AddComponent<NetworkRunner>();
        mRunner.ProvideInput = true;

        mButtonCreateParty.onClick.AddListener(OnCreatePartyClicked);
        mButtonInviteFriend.onClick.AddListener(OnInviteFriendClicked);
        mButtonShowInvites.onClick.AddListener(ShowPartyInvitations);
        mButtonAcceptInvite.onClick.AddListener(OnAcceptInvite);
        mButtonRejectInvite.onClick.AddListener(OnRejectInvite);
    }

    private async void OnCreatePartyClicked()
    {
        string roomName = mInputRoomName.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "FusionRoom_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        await mRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            Scene = null,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnInviteFriendClicked()
    {
        string friendEmail = mInputInviteFriend.text;
        if (!string.IsNullOrEmpty(friendEmail))
        {
            InviteFriendToParty(friendEmail);
        }
    }

    private async void InviteFriendToParty(string friendEmail)
    {
        try
        {
            string myUid = mAuth.CurrentUser?.UserId;
            string myEmail = mAuth.CurrentUser?.Email;
            string roomName = mRunner.SessionInfo.Name;

            if (myUid == null || string.IsNullOrEmpty(roomName)) return;

            string friendUid = await FindUidByEmail(friendEmail);
            if (friendUid == null) return;

            var invitationRef = mDB.Collection("users").Document(friendUid)
                .Collection("invitations").Document(roomName);

            var data = new Dictionary<string, object>
            {
                { "roomName", roomName },
                { "from", myEmail },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            await invitationRef.SetAsync(data);
            Debug.Log($"✅ {friendEmail}에게 파티 초대 전송 완료");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ 초대 전송 실패: " + ex.Message);
        }
    }

    private async void ShowPartyInvitations()
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
                string from = doc.GetValue<string>("from");
                Debug.Log($"🎉 {from} 님이 '{roomName}' 파티에 초대했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("초대 목록 불러오기 실패: " + e.Message);
        }
    }

    private async void OnAcceptInvite()
    {
        try
        {
            string roomName = mInputAcceptRoomName.text;
            string myUid = mAuth.CurrentUser?.UserId;

            if (string.IsNullOrEmpty(roomName) || myUid == null) return;

            var invitationRef = mDB.Collection("users").Document(myUid)
                .Collection("invitations").Document(roomName);

            await invitationRef.DeleteAsync();

            await mRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = roomName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            Debug.Log("✅ 초대 수락 후 입장 시도: " + roomName);
        }
        catch (Exception e)
        {
            Debug.LogError("초대 수락 중 오류: " + e.Message);
        }
    }

    private async void OnRejectInvite()
    {
        try
        {
            string roomName = mInputAcceptRoomName.text;
            string myUid = mAuth.CurrentUser?.UserId;

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

    private async System.Threading.Tasks.Task<string> FindUidByEmail(string email)
    {
        try
        {
            var snapshot = await mDB.Collection("users")
                .WhereEqualTo("email", email).Limit(1)
                .GetSnapshotAsync();

            return snapshot.Count > 0 ? snapshot.Documents.First().Id : null;
        }
        catch (Exception e)
        {
            Debug.LogError("UID 조회 실패: " + e.Message);
            return null;
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
}
