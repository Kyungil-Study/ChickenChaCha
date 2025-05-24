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
    Start,
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
                DontDestroyOnLoad(mInsatnce.gameObject);
            }
            return mInsatnce;
        }
    }
    
    private NetworkRunner mNetworkRunner;
    private NetworkSceneManagerDefault mNetworkSceneManager;
    
    private GameSessionState mGameState = GameSessionState.Start;
    public GameSessionState GameState
    {
        get { return mGameState; }
        set { mGameState = value; }
    }
    
    private void Awake()
    {
        if(mInsatnce == null)
        {
            mInsatnce = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(mInsatnce != this) // 내가 초기화된 싱글톤이 아니라면 삭제
        {
            Destroy(gameObject);
        }
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

    public async void StartMatchMaking()
    {
        Debug.Log($"[SessionManager] StartMatchMaking ::: 세션 리스트 요청 중...");
        StartGameResult result = await mNetworkRunner.JoinSessionLobby(SessionLobby.ClientServer);
        Debug.Log($"[SessionManager] JoinSessionLobby Result = {result.ToString()}");
    }
    
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"[SessionManager] OnSessionListUpdated, sessionList.Count = {sessionList.Count}");

    }

    #region NetworkRunnerCallbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    #endregion
    
}
