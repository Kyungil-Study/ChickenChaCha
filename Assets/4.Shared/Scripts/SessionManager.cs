using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public enum GameSessionState
{
    Ready,
    Login,
    Lobby,
    MatchMaking,
    Room,
    InGame,
    Result
}

public class GameClient
{
    private PlayerRef playerRef { get; set; }
}

public class SessionManager : MonoBehaviour , INetworkRunnerCallbacks
{
    public const int MAX_PLAYER_COUNT = 4;
    
    public class GameRoomInfo
    {
        public GameClient masterClient;
        public List<GameClient> questClients = new List<GameClient>();
    }
    
    private static SessionManager mInsatnce;
    public static SessionManager Instance
    {
        get
        {
            if (mInsatnce == null)
            {
                mInsatnce = FindObjectOfType<SessionManager>();
            }
            return mInsatnce;
        }
    }
    
    private NetworkRunner mNetworkRunner;
    private NetworkSceneManagerDefault mNetworkSceneManager;
    
    private GameSessionState mSessionState = GameSessionState.Ready;

    public class Callbacks
    {
        public Action OnEnteredLobby;
        public Action OnLeftLobby;
        
        public Action OnEnteredMatchMaking;
        public Action OnLeftMatchMaking;
        
        public Action OnEnteredRoom;
        public Action OnLeftRoom;
        
        public Action OnEnteredInGame;
        public Action OnLeftInGame;
        
        public Action OnEnteredResult;
        public Action OnLeftResult;
    }
    
    public Callbacks callbacks = new Callbacks();
    
    private void Awake()
    {
        AccountManagement.Instance.OnLogInEvent += OnLogIn;
    }
    
    private async Task InitRunnerAsync()
    {
        if (mNetworkRunner != null)
        {
            Destroy(mNetworkRunner);
        }

        if (mNetworkSceneManager != null)
        {
            Destroy(mNetworkSceneManager);
        }
        
        await Task.Delay(1000);
        
        mNetworkRunner = gameObject.AddComponent<NetworkRunner>();
        mNetworkSceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        mNetworkRunner.ProvideInput = true;
        
    }

    public async void EnterLobbyAsync()
    {
        if (mNetworkRunner == null)
        {
            await InitRunnerAsync();
        }
        
        mSessionState = GameSessionState.Lobby;
        callbacks.OnEnteredLobby?.Invoke();
    }

    public async void EnterMatchMakingAsync()
    {
        if (mSessionState == GameSessionState.MatchMaking)
        {
            return;
        }
        
        if (mNetworkRunner == null)
        {
            Debug.LogAssertion($"<color=red>[SessionManager] StartMatchMakingAsync ::: NetworkRunner가 초기화되지 않았습니다.</color>");
            return;
        }
        
        mSessionState = GameSessionState.MatchMaking;
        
        Debug.Log($"[SessionManager] StartMatchMaking ::: 세션 리스트 요청 중...");
        StartGameResult result = await mNetworkRunner.JoinSessionLobby(sessionLobby: SessionLobby.Shared);
        if (result.Ok)
        {
            Debug.LogError($"[SessionManager] StartMatchMakingAsync ::: 세션 리스트 요청 실패, result = {result}");
            return;
        }
        Debug.Log("[SessionManager] JoinSessionLobby Success");

        mSessionState = GameSessionState.Room;
        callbacks.OnEnteredRoom?.Invoke();
    }

    public async void LeaveMatchMakingAsync()
    {
        if (mNetworkRunner == null)
        {
            Debug.LogAssertion($"<color=red>[SessionManager] LeaveMatchMakingAsync ::: NetworkRunner가 초기화되지 않았습니다.</color>");
            return;
        }
        mSessionState = GameSessionState.Lobby;
        await mNetworkRunner.Shutdown();

        callbacks.OnLeftRoom?.Invoke();
    }

    private void OnLogIn(OnLogInEventArgs eventArgs)
    {
        // 중복요청 제외
        if (mSessionState == GameSessionState.Login)
        {
            return;
        }

        Debug.Log($"[SessionManager] OnSignIn ::: 유저 이름 = {eventArgs.UserID}");
            
        mSessionState = GameSessionState.Login;
        EnterLobbyAsync();
    }
    
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"[SessionManager] OnSessionListUpdated, sessionList.Count = {sessionList.Count}");
        foreach (SessionInfo session in sessionList)
        {
            Debug.Log($"Session Name = {session.Name}, Player Count = {session.PlayerCount}, Max Player Count = {session.MaxPlayers}");
        }
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player.PlayerId}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    #region NetworkRunnerCallbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

   

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    #endregion
    
}
