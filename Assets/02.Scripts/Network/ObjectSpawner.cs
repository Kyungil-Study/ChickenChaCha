using Fusion;
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
            Runner.Spawn(playerPrefab);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(Runner.IsSharedModeMasterClient)
        {
            Debug.Log($"{player}, {Runner.LocalPlayer}");
            GameManager.Instance.InstanceNull(true);
            GameManagerSpawn();
        }
    }
}