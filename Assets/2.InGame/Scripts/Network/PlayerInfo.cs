using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public struct PlayerInfo : INetworkStruct
{
    public PlayerRef player;
    public bool isActive;
    public int score;
    
    public PlayerInfo(PlayerRef player, bool isActive, int score)
    {
        this.player = player;
        this.isActive = isActive;
        this.score = score;
    }
    
}