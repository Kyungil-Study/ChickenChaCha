using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{

    public GameObject playerPrefab;
    public NetworkPrefabRef gameManagerPrefab;
    private NetworkObject mGameManager;

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

    public void GameManagerChange()
    {
        GameManager.Instance.GetComponent<NetworkObject>().RequestStateAuthority();
    }

    private void PlayerSpawn(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(playerPrefab);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        var players = Runner.ActivePlayers.OrderBy(player => player.AsIndex);
        if (players.FirstOrDefault() == Runner.LocalPlayer)
        {
            GameManagerChange();
        }
    }
}