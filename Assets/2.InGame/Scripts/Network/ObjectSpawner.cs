using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    public GameObject playerPrefab;
    public NetworkPrefabRef gameManagerPrefab;
    
    public void PlayerJoined(PlayerRef player)
    {
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
            var playerObj = Runner.Spawn(playerPrefab);
            playerObj.GetComponent<NetworkPlayer>().playerIndex = player.AsIndex - 1;
            Runner.SetPlayerObject(player, playerObj);
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