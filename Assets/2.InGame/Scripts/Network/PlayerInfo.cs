using Fusion;
using UnityEngine;

public struct PlayerInfo : INetworkStruct
{
    public PlayerRef player;
    public bool isActive;
    public int score;
    public NetworkId steppingTile;
    
    public PlayerInfo(PlayerRef player, bool isActive, int score, NetworkId steppingTile)
    {
        this.player = player;
        this.isActive = isActive;
        this.score = score; 
        this.steppingTile = steppingTile;
    }
    
}