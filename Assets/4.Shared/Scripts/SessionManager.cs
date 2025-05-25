using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    Lobby,
    Match,
    InGame
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
    
    private GameSessionState mGameState = GameSessionState.Ready;
    public GameSessionState GameState
    {
        get { return mGameState; }
        set { mGameState = value; }
    }

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
        if (mNetworkRunner != null)
        {
            Destroy(mNetworkRunner);
        }
        if(mNetworkSceneManager != null)
        {
            Destroy(mNetworkSceneManager);
        }
        
        await Task.Delay(1000);
        
        if (mNetworkRunner == null)
        {
            await InitRunnerAsync();
        }
        
        Debug.Log($"[SessionManager] StartMatchMaking ::: 세션 리스트 요청 중...");
        StartGameResult result = await mNetworkRunner.JoinSessionLobby(sessionLobby: SessionLobby.Shared);
        Debug.Log($"[SessionManager] JoinSessionLobby Result = {result.ToString()}");

        mGameState = GameSessionState.Lobby;
    }

    private void OnLogIn(OnLogInEventArgs eventArgs)
    {
        Debug.Log($"[SessionManager] OnSignIn ::: 유저 이름 = {eventArgs.UserID}");
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
