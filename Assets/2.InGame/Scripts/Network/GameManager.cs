using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IToNetwork, IPlayerJoined
{
    
    [Networked] 
    private NetworkDictionary<NetworkId, INetworkStruct> mNetworkDictionary => default;
    
    
    public void SendSelectedTile(PlayerRef player, SelectingTile tile)
    {
        bool result = RuleManager.Instance.OpenTile(new SteppingTile(), tile);
        //SetPlayerState(player, result);
    }
    
    public void SendSelectedTile(SelectingTile tile)
    {
        throw new NotImplementedException();
    }

    public SelectingTile GetSelectedTile()
    {
        throw new NotImplementedException();
    }
    
     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
     public void RPC_AddDictionary(NetworkId networkId, INetworkStruct networkStruct)
     {
         mNetworkDictionary.Add(networkId, networkStruct);
         Debug.Log("Added to dictionary: " + networkId);
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
     

     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
     public void RPC_DebugList()
     {
         
     }
    
    public void PlayerJoined(PlayerRef player)
    {
        RPC_AddPlayer(player);
    }
}
