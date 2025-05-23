using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>
{

    [Networked] 
    [UnitySerializeField]
    private NetworkDictionary<NetworkId, PlayerInfo> mNetworkPlayerDictionary => default;
    
    [Networked] 
    [UnitySerializeField]
    private NetworkDictionary<NetworkId, TileInfo> mNetworkTileDictionary => default;
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddDictionary<T>(NetworkId id, INetworkStruct networkStruct) where T : INetworkStruct
    {
        
        if (networkStruct is PlayerInfo playerInfo)
        { 
            mNetworkPlayerDictionary.Add(id, playerInfo);
        }

        if (networkStruct is TileInfo tileInfo)
        {
            mNetworkTileDictionary.Add(id, tileInfo);
        }
        Debug.Log("Added to dictionary: " + id);
    }
     
    public void SetPlayerState(PlayerRef Player, bool isActive)
    {
        if(Runner.TryGetPlayerObject(Player, out NetworkObject netObj))
        {
            netObj.GetComponent<NetworkPlayer>().ReceiveMovePermission(isActive);
        }
        // mPlayerInfo.Set(nextPlayer,
        //     new PlayerInfo(mPlayerInfo[nextPlayer].player, isActive, mPlayerInfo[nextPlayer].score));
    }
    
    public INetworkStruct GetNetWorkStrut <T>(NetworkId id)
    {
        if (typeof(T) == typeof(PlayerInfo))
        {
            if (mNetworkPlayerDictionary.TryGet(id, out PlayerInfo networkStruct))
            {
                return networkStruct;
            }
        }

        if (typeof(T) == typeof(TileInfo))
        {
            if (mNetworkTileDictionary.TryGet(id, out TileInfo networkStruct))
            {
                return networkStruct;
            }
        }
        return null;
    }
    
    
    
    // public PlayerInfo? GetPlayerInfoOrNull(PlayerRef player)
    // {
    //     foreach (var data in mNetworkDictionary)
    //     {
    //         if (data.Value is PlayerInfo playerInfo)
    //         {
    //             if (playerInfo.player == player)
    //             {
    //                 return playerInfo;
    //             }
    //         }
    //     }
    //        return null;
    // }
    //  
    //  public INetworkStruct GetNetWorkStrut (NetworkId id)
    //  {
    //      if (mNetworkDictionary.TryGet(id, out INetworkStruct networkStruct))
    //      {
    //          return networkStruct;
    //      }
    //      return null;
    //  }
}
