using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IToNetwork, IPlayerJoined
{
    
    [Networked] 
    private NetworkDictionary<int, Tile> mSteppingTileInfo => default;

    public void AddTile(int t, SteppingTile tile)
    {
        mSteppingTileInfo.Add(t, tile);
    }
    
    
    [Networked] 
    private NetworkDictionary<int, SelectingTile> mSelectingTileInfo => default;
    
    
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
    
    
    // 에러 코드 주석 처리
    // 에러코드 수정후
    // 아래 파일 호출부도 복원해주세요.
    // ScoreBoaurUI
    // PlayerController 
     [Networked] 
     private NetworkDictionary<PlayerRef, PlayerInfo> mPlayerInfo => default;
     
     
     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
     public void RPC_AddPlayer(PlayerRef player)
     {
         if (mPlayerInfo.Count >= 4)
         {
             Debug.Log("더 이상 추가 할 수 없습니다.");
             
             return;
         }

         PlayerInfo playerInfo = new PlayerInfo(player, false, 1, new NetworkId());
         Debug.Log($"{playerInfo.player} , {playerInfo.isActive}, {playerInfo.score}");
         mPlayerInfo.Add(player, playerInfo);
     }

     public PlayerInfo? GetPlayerInfoOrNull(PlayerRef player)
     {
         if(mPlayerInfo.TryGet(player, out var value))
         {
             return value;
         }
         return null;
     }

     public PlayerInfo? GetLocalPlayerOrNull()
     {
         if(mPlayerInfo.TryGet(Runner.LocalPlayer, out var value))
         {
             return value;
         }
         return null;
     }

     public List<PlayerRef> GetPlayersInfo()
     {
         List<PlayerRef> players = new List<PlayerRef>();
         foreach (var playerInfo in mPlayerInfo)
         {
             players.Add(playerInfo.Key);
         }
         return players;
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
