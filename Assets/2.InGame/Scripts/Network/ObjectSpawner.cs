using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    public GameObject playerPrefab;
    public NetworkPrefabRef gameManagerPrefab;
    
    private PlayerInfo mPlayerInfo;
    public FusionBootstrapDebugGUICustom gui;
    
    public void PlayerJoined(PlayerRef player)
    {
        mPlayerInfo = new PlayerInfo();
        GameManagerSpawn();
        PlayerSpawn(player);
    }

    private void GameManagerSpawn()
    {
        if (GameManager.Instance != null)
        {
            return;
        }

        if (Runner.IsSharedModeMasterClient)
        {
            Debug.Log("MasterClient에서 GameManager를 스폰합니다.");
            Runner.Spawn(gameManagerPrefab);
        }
    }

    private void PlayerSpawn(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            mPlayerInfo.player = player;
            mPlayerInfo.playerName = gui.nickName;
            mPlayerInfo.score = 1;
            mPlayerInfo.netObj = Runner.Spawn(playerPrefab);
            GameManager.Instance.PlayerJoinAddList(mPlayerInfo);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        // var players = Runner.ActivePlayers.OrderBy(player => player.AsIndex);
        // if (players.FirstOrDefault() == Runner.LocalPlayer)
        // {
        //     GameManagerChange();
        // }
    }
}