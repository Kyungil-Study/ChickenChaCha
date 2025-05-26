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

public class FusionPartyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("파티 UI")]
    [SerializeField] private TMP_InputField inputRoomName;
    [SerializeField] private TMP_InputField inputInviteFriend;
    [SerializeField] private TMP_InputField inputAcceptRoomName;
    [SerializeField] private Button buttonCreateParty;
    [SerializeField] private Button buttonInviteFriend;
    [SerializeField] private Button buttonShowInvites;
    [SerializeField] private Button buttonAcceptInvite;
    [SerializeField] private Button buttonRejectInvite;

    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private NetworkRunner runner;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        buttonCreateParty.onClick.AddListener(OnCreatePartyClicked);
        buttonInviteFriend.onClick.AddListener(OnInviteFriendClicked);
        buttonShowInvites.onClick.AddListener(ShowPartyInvitations);
        buttonAcceptInvite.onClick.AddListener(OnAcceptInvite);
        buttonRejectInvite.onClick.AddListener(OnRejectInvite);
    }

    private async void OnCreatePartyClicked()
    {
        string roomName = inputRoomName.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "FusionRoom_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            Scene = null,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnInviteFriendClicked()
    {
        string friendEmail = inputInviteFriend.text;
        if (!string.IsNullOrEmpty(friendEmail))
        {
            InviteFriendToParty(friendEmail);
        }
    }

    private async void InviteFriendToParty(string friendEmail)
    {
        try
        {
            string myUid = auth.CurrentUser?.UserId;
            string myEmail = auth.CurrentUser?.Email;
            string roomName = runner.SessionInfo.Name;

            if (myUid == null || string.IsNullOrEmpty(roomName)) return;

            string friendUid = await FindUidByEmail(friendEmail);
            if (friendUid == null) return;

            var invitationRef = db.Collection("users").Document(friendUid)
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
            string myUid = auth.CurrentUser?.UserId;
            if (myUid == null) return;

            var snapshot = await db.Collection("users").Document(myUid)
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
            string roomName = inputAcceptRoomName.text;
            string myUid = auth.CurrentUser?.UserId;

            if (string.IsNullOrEmpty(roomName) || myUid == null) return;

            var invitationRef = db.Collection("users").Document(myUid)
                .Collection("invitations").Document(roomName);

            await invitationRef.DeleteAsync();

            await runner.StartGame(new StartGameArgs
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
            string roomName = inputAcceptRoomName.text;
            string myUid = auth.CurrentUser?.UserId;

            if (string.IsNullOrEmpty(roomName) || myUid == null) return;

            var invitationRef = db.Collection("users").Document(myUid)
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
            var snapshot = await db.Collection("users")
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
