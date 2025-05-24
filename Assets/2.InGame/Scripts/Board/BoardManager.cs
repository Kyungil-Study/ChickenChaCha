using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Fusion;
using UnityEngine;

public class BoardManager : DontDestroyOnNetwork<BoardManager>
{
    public GameObject steppingTilePrefab;
    public GameObject selectingTilePrefab;
    public GameObject playerPrefab;
    public Texture2D[] tileTextures;
    
    public SteppingTile[] steppingTiles = new SteppingTile[24];
    public SelectingTile[] selectingTiles = new SelectingTile[12];

    private readonly int[] imageKeys = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

    public void InitBoard(NetworkPlayer[] players)
    {
        SpawnSteppingTiles(transform.position + new Vector3(-6, 2.5f, -6));
        SpawnSelectingTiles(4,3,transform.position, new Vector3(-3, 2.5f, -3),new Vector3(3, 2.5f, 3));
        InitPlayerPieces(players);
    }

    public void SpawnSteppingTiles(Vector3 zeroPosition)
    {
        int index = 0;
        
        int[] firstBag = RandomUtil.GetShuffled(imageKeys);
        int[] secondBag = RandomUtil.GetShuffled(imageKeys);
        var rndKey= firstBag.Concat(secondBag).ToArray();
        
        Vector3 firstEnd = SpawnLine(zeroPosition, Vector3.forward);
        Vector3 secondEnd = SpawnLine(firstEnd, Vector3.right);
        Vector3 thirdEnd = SpawnLine(secondEnd, Vector3.back);
        SpawnLine(thirdEnd, Vector3.left);

        Vector3 SpawnLine(Vector3 initPosition, Vector3 direction)
        {
            for (int i = 0; i < 6; i++)
            {
                int imageKey = rndKey[index];
                
                TileInfo info = new TileInfo(ETileType.Stepping, index, imageKey);
                var netObj = Runner.Spawn(steppingTilePrefab, initPosition, onBeforeSpawned: (runner, o) =>
                {
                    Tile tile = o.GetComponent<Tile>();
                    tile.Info = info;
                });
                
                initPosition += direction * 2;
                index++;
            }

            return initPosition;
        }
        for (int i = 0; i < steppingTiles.Length; i++)
        {
            steppingTiles[i].Next = steppingTiles[(i + 1) % steppingTiles.Length];
            steppingTiles[i].Prev = steppingTiles[(i - 1 + steppingTiles.Length) % steppingTiles.Length];
        }
    }

    public void SpawnSelectingTiles(int columnCount, int rowCount, Vector3 offset,Vector3 start, Vector3 end)
    {
        Vector3 diff = end - start;
        Vector3 garo = Vector3.right * diff.x / (columnCount - 1);
        Vector3 sero = Vector3.forward * diff.z / (rowCount - 1);
        
        int[] rndKey = RandomUtil.GetShuffled(imageKeys);
        
        for (int x = 0; x < columnCount; x++)
        {
            for (int z = 0; z < rowCount; z++)
            {
                int index = z + x * rowCount;
                int imageKey = rndKey[index];
                
                TileInfo info = new TileInfo(ETileType.Selecting, index, imageKey);
                var netObj = Runner.Spawn(selectingTilePrefab, offset + start + garo * x + sero * z, onBeforeSpawned: (runner, o) =>
                {
                    Tile tile = o.GetComponent<Tile>();
                    tile.Info = info;
                });
            }
        }
    }
    
    public void InitPlayerPieces(NetworkPlayer[] players)
    {
        int div = steppingTiles.Length / players.Length;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].transform.position = steppingTiles[i * div].transform.position;
        }
    }

    public void SubscribeAllSelectingTiles(Action<Tile> callback)
    {
        foreach (var tile in selectingTiles)
        {
            tile.onClick += callback;
        }
    }
}