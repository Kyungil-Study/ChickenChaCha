using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IToNetwork, IPlayerJoined
{
    [Networked] 
    [UnitySerializeField]
    private NetworkDictionary<PlayerRef, PlayerInfo> mPlayerInfo => default;
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddPlayer(PlayerRef player)
    {
        if (mPlayerInfo.Count >= 4)
        {
            Debug.Log("Max player count reached.");
            return;
        }

        PlayerInfo playerInfo = new PlayerInfo(player, false, 1);
        Debug.Log($"{playerInfo.player} , {playerInfo.isActive}, {playerInfo.score}");
        mPlayerInfo.Add(player, playerInfo);
    }

    public PlayerInfo? GetPlayerInfoOrNull(PlayerRef player)
    {
        foreach (KeyValuePair<PlayerRef, PlayerInfo> playerInfo in mPlayerInfo)
        {
            if (playerInfo.Key == player)
            {
                return playerInfo.Value;
            }
        }
        return null;
    }

    public PlayerRef GetLocalPlayer()
    {
        return Runner.LocalPlayer;
    }

    public List<PlayerRef> GetPlayersInfo()
    {
        List<PlayerRef> players = new List<PlayerRef>();
        foreach (KeyValuePair<PlayerRef, PlayerInfo> playerInfo in mPlayerInfo)
        {
            players.Add(playerInfo.Key);
        }
        return players;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DebugList()
    {
        
    }
    
    public void PlayerJoined(PlayerRef player)
    {
        RPC_AddPlayer(player);
    }
    
    // public void ColorChanged(MeshRenderer renderer, Color color)
    // {
    //     renderer.material.color = color;
    // }
    
    // public void AblePlayerInputAuthority(PlayerRef player)
    // {
    //     foreach (var playerInfo in mPlayers)
    //     {
    //         if (playerInfo.player == player)
    //         {
    //             playerInfo.netObj.AssignInputAuthority(player);
    //         }
    //     }
    // }
    //
    // public void RemovePlayerInputAuthority(PlayerRef player)
    // {
    //     foreach (var playerInfo in mPlayers)
    //     {
    //         if (playerInfo.player == player)
    //         {
    //             playerInfo.netObj.RemoveInputAuthority();
    //         }
    //     }
    // }

    public void SendSelectedTile(SelectingTile tile)
    {
        
    }

    public SelectingTile GetSelectedTile()
    {
        throw new NotImplementedException();
    }
    
    public int CheckTailInfo(IToPlayer player)
    {
        throw new NotImplementedException();
    }
}
