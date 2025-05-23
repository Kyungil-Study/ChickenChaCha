using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>
{

    [Networked] 
    [UnitySerializeField]
    private NetworkDictionary<NetworkId, int> mNetworkDictionary => default;
    //
    // public void SendSelectedTile(SelectingTile tile)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public SelectingTile GetSelectedTile()
    // {
    //     throw new NotImplementedException();
    // }
    //
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddDictionary(NetworkId networkId, int networkStruct)
    {
        mNetworkDictionary.Add(networkId, networkStruct);
        Debug.Log("Added to dictionary: " + networkId);
    }
    //  
    public void SetPlayerState(PlayerRef Player, bool isActive)
    {
        if(Runner.TryGetPlayerObject(Player, out NetworkObject netObj))
        {
            netObj.GetComponent<NetworkPlayer>().ReceiveMovePermission(isActive);
        }
        // mPlayerInfo.Set(nextPlayer,
        //     new PlayerInfo(mPlayerInfo[nextPlayer].player, isActive, mPlayerInfo[nextPlayer].score));
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

    // public T? GetNetWorkStrut <T>(NetworkId id) where T : INetworkStruct
    // {
    //     if (mNetworkDictionary.TryGet(id, out INetworkStruct networkStruct))
    //     {
    //         return networkStruct as T;
    //     }
    //     return null;
    // }
}
