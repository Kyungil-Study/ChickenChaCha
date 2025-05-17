using Fusion;
using UnityEngine;

public class ObjectSpawner : SimulationBehaviour, IPlayerJoined
{
    
    public GameObject playerPrefab;
    public NetworkPrefabRef gameManagerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        GameManagerSpawn();
        PlayerSpawn(player);
    }
    
    public void PlayerLeft(PlayerRef player)
    {
        // 플레이어가 떠났을 때 GameManager를 스폰
        GameManagerSpawn();
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
}