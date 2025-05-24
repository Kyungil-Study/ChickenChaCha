using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>
{

    [Networked] 
    private NetworkDictionary<NetworkId, PlayerInfo> mNetworkPlayerDictionary => default;
    
    [Networked, Capacity(36)] 
    private NetworkDictionary<NetworkId, TileInfo> mNetworkTileDictionary => default;
    
    public void AddDictionary<T>(NetworkId id, INetworkStruct networkStruct) where T : INetworkStruct
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
        Debug.Log(Player);
        if (Player != Runner.LocalPlayer)
        {
            return;
        }
        
        if(Runner.TryGetPlayerObject(Player, out NetworkObject netObj))
        {
            netObj.GetComponent<NetworkPlayer>().ReceiveMovePermission(isActive);
            Debug.Log("Set Player State");
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
    
    //==========================이 앞, 추가됨=======================================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }

    public PlayerRef[] ActivePlayers;
    public int CurPlayerIndex { get; set; }
    
    public void GameStart()
    {
        ActivePlayers = Runner.ActivePlayers.OrderBy(player => player.AsIndex).ToArray();
        BoardManager.Instance.InitBoard(ActivePlayers);
        SetPlayerState(ActivePlayers[0],true);
    }

    private SteppingTile FindTargetTile(PlayerRef player)
    {
        var t = Runner.GetPlayerObject(player).GetComponent<NetworkPlayer>();
        SteppingTile standing = t.currentTile;
        
        while (standing.StandingPlayer != PlayerRef.None)
        {
            standing = standing.Next;
        }

        return standing;
    }

    public void Showdown(PlayerRef player, Tile select)
    {
        SteppingTile target = FindTargetTile(player);
        if (target.IsSamePicture(select))
        {
            Success();
        }
        else
        {
            Fail();
        }
        void Success()
        {
            Runner.GetPlayerObject(player).GetComponent<NetworkPlayer>().MovePlayer(target);
            
            Debug.Log("Suck");
        }

        void Fail()
        {
            SetPlayerState(player, false);
            MoveIndex();
            
            PlayerRef nextPlayer = ActivePlayers[CurPlayerIndex];
            SetPlayerState(nextPlayer, true);
            
            Debug.Log("Fuck");
        }
    }
    
    private void MoveIndex()
    {
        CurPlayerIndex++;
        if (CurPlayerIndex >= ActivePlayers.Length)
        {
            CurPlayerIndex = 0;
        }
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
