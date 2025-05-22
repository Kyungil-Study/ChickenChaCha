using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class BoardManager : NetworkBehaviour
{
    public static BoardManager Instance;
    public SteppingTile[] steppingTiles;
    public SelectingTile[] selectingTiles;
    public GameObject playerPrefab;
    
    private readonly int[] imageKeys = new int[12] {0,1,2,3,4,5,6,7,8,9,10,11};
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Runner.Despawn(Object);
        }
    }

    public void InitTiles()
    {
        int[] rndKey = RandomUtil.GetShuffled(imageKeys);
        int keyLength = imageKeys.Length;
        
        // steppingTiles
        for (int i = 0; i < steppingTiles.Length; i++)
        {
            if (i == keyLength)
            {
                rndKey = RandomUtil.GetShuffled(imageKeys);
            }
            steppingTiles[i].imageKey = (rndKey[i % keyLength]);
            steppingTiles[i].Next = steppingTiles[i == steppingTiles.Length - 1 ? 0 : i + 1];
            steppingTiles[i].Prev = steppingTiles[i == 0 ? steppingTiles.Length - 1 : i - 1];
        }
        
        // selectingTiles
        rndKey = RandomUtil.GetShuffled(imageKeys);
        for(int i = 0; i < selectingTiles.Length; i++)
        {
            selectingTiles[i].imageKey = (rndKey[i]);
        }
    }
    
    public void InitPlayerPieces()
    {
        PlayerRef[] players = Runner.ActivePlayers.ToArray();
        int playerCount = players.Length;

        int div = steppingTiles.Length / playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            SteppingTile tile = steppingTiles[i * div];
            PlayerRef player = players[i];
            
            SpawnPlayer(player, tile.transform.position);
            tile.StandingPlayer = player;
        }
    }

    private void SpawnPlayer(PlayerRef player, Vector3 position)
    {
        Runner.Spawn(playerPrefab, position, Quaternion.identity, player);
    }
    
    public void SubscribeAllSelectingTiles(Action<Tile> callback)
    {
        foreach (var tile in selectingTiles)
        {
            tile.onClick += callback;
        }
    }
}
