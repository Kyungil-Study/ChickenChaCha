using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = System.Object;


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
    public const int LOBBY_SCENE_INDEX = 1;
    public const int IN_GAME_SCENE_INDEX = 2;
    
    public class GameRoomInfo
    {
        public string roomName;
        public List<GameClient> players = new List<GameClient>();
        public int userCount => players.Count; // 마스터 클라이언트 포함
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
    
    private GameRoomInfo mRoomInfo = new GameRoomInfo();
    public GameRoomInfo RoomInfo => mRoomInfo;
    
    private NetworkRunner mNetworkRunner;
    private NetworkSceneManagerDefault mNetworkSceneManager;
    
    private GameSessionState mSessionState = GameSessionState.Ready;

    public class Callbacks
    {
        public Action OnLoginSuccess;
        
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
        UserManager.Instance.OnLogInEvent += OnLogIn;
        callbacks.OnLoginSuccess += LoadLobbyScene;
    }
    
    private void LoadLobbyScene()
    {
        SceneManager.LoadScene(LOBBY_SCENE_INDEX, LoadSceneMode.Single);
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
        if (result.Ok == false)
        {
            Debug.LogError($"[SessionManager] StartMatchMakingAsync ::: 세션 리스트 요청 실패, result = {result}");
            return;
        }
        Debug.Log("[SessionManager] JoinSessionLobby Success");
        
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
        LoadLobbyScene();
        EnterLobbyAsync();
    }
    
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"[SessionManager] OnSessionListUpdated, sessionList.Count = {sessionList.Count}");
        SessionInfo joinAble = sessionList.FirstOrDefault(sessionInfo =>
        {
            return sessionInfo.PlayerCount < sessionInfo.MaxPlayers &&
                   sessionInfo.IsOpen && sessionInfo.IsVisible;
        });

        if (joinAble == null)
        {
            CreateRoom("room_" + Guid.NewGuid());
        }
        else
        {
            JoinRoom(joinAble.Name);
        }
    }

    private async void CreateRoom(string roomName)
    {
        Debug.Log($"[SessionManager] CreateRoom : {roomName}");

        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName, // room_{guid} 가 roomName
            PlayerCount = MAX_PLAYER_COUNT,
            SceneManager = mNetworkSceneManager,
        };
        
        await mNetworkRunner.StartGame(args);
        mRoomInfo.players.Clear();
        mRoomInfo.roomName = roomName;
        
        mSessionState = GameSessionState.Room;
        
        callbacks.OnEnteredRoom?.Invoke();
    }

    private async void JoinRoom(string roomName)
    {
        Debug.Log($"[SessionManager] JoinRoom : {roomName}");
        
        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName, // room_{guid} 가 roomName
            SceneManager = mNetworkSceneManager,
        };
        
        await mNetworkRunner.StartGame(args);
        mRoomInfo.players.Clear();
        mRoomInfo.roomName = roomName;
        mSessionState = GameSessionState.Room;

        callbacks.OnEnteredRoom?.Invoke();
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Joined: {player.PlayerId}");
        bool canStart = mNetworkRunner.IsSharedModeMasterClient;
        canStart &= runner.ActivePlayers.Count() == MAX_PLAYER_COUNT;
        if (canStart)
        {
            Debug.Log($"[SessionManager] OnPlayerJoined : {player.PlayerId} ::: 플레이어 수가 {MAX_PLAYER_COUNT}명에 도달했습니다. 게임 시작을 요청합니다.");
            StartInGame();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
       
    }

    private void StartInGame()
    {
        Debug.Log($"[SessionManager] StartInGame : Master에서 호출됨");
        mNetworkRunner.LoadScene(SceneRef.FromIndex(IN_GAME_SCENE_INDEX));
    }
    
    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        mSessionState = GameSessionState.InGame;
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

    

    #endregion
    
}
