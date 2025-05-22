using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IPlayerLeft, IToNetwork
{
    private List<PlayerInfo> mPlayers;
    
    public override void Spawned()
    {
        mPlayers = new List<PlayerInfo>();
    }

    public void ActivePlayer(PlayerRef player)
    {
        
    }

    public void PlayerJoinAddList(PlayerInfo playerInfo)
    {
        mPlayers.Add(playerInfo);
    }
    
    public void AblePlayerInputAuthority(PlayerRef player)
    {
        foreach (var playerInfo in mPlayers)
        {
            if (playerInfo.player == player)
            {
                playerInfo.netObj.AssignInputAuthority(player);
            }
        }
    }
    
    public void RemovePlayerInputAuthority(PlayerRef player)
    {
        foreach (var playerInfo in mPlayers)
        {
            if (playerInfo.player == player)
            {
                playerInfo.netObj.RemoveInputAuthority();
            }
        }
    }
    
    public PlayerInfo GetPlayer(PlayerRef player)
    {
        foreach (var playerInfo in mPlayers)
        {
            if (playerInfo.player == player)
            {
                return playerInfo;
            }
        }

        return null;
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
