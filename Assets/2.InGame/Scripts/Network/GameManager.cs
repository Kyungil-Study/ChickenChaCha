using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IToNetwork, IPlayerJoined
{
    [Networked] 
    [UnitySerializeField]
    private NetworkDictionary<PlayerRef, PlayerInfo> mPlayerInfo => default;
    
    
    private int mPlayerCount;
    protected override void Awake()
    {
        base.Awake();
        mPlayerCount = 0;
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddPlayer(PlayerRef player)
    {
        if (mPlayerCount >= 4)
        {
            Debug.Log("Max player count reached.");
            return;
        }
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.player = player;
        playerInfo.isActive = false;
        playerInfo.score = 1;
        mPlayerInfo.Add(player, playerInfo);
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
    
}
