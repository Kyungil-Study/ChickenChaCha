using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IPlayerJoined, IPlayerLeft, IToNetwork
{
    private List<PlayerInfo> mPlayers;
    public class PlayerInfo
    {
        public PlayerRef player;
        public string playerName;
        public int score;
    }
    
    public override void Spawned()
    {
        mPlayers = new List<PlayerInfo>();
    }

    public void PlayerJoined(PlayerRef player)
    {
        mPlayers.Add(new PlayerInfo
        {
            player = player,
            score = 1
        });
    }

    public void PlayerLeft(PlayerRef player)
    {
        int removedPlayersScore = mPlayers.Find(p => p.player == player).score;
        mPlayers.Remove(mPlayers.Find(p => p.player == player));
    }

    public List<PlayerInfo> GetPlayersInfo()
    {
        return mPlayers;
    }
    
    // [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    // public void RPC_AddScore()
    // {
    //     
    // }
    // public void ColorChanged(MeshRenderer renderer, Color color)
    // {
    //     renderer.material.color = color;
    // }
}
