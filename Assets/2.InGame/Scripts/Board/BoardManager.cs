using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BoardManager : NetworkBehaviour
{
    public static BoardManager Instance;
    public GameObject steppingTilePrefab;
    public GameObject selectingTilePrefab;
    public Texture2D[] tileTextures;
    public SteppingTile[] steppingTiles = new SteppingTile[24];
    public SelectingTile[] selectingTiles = new SelectingTile[12];
    public GameObject playerPrefab;

    private readonly int[] imageKeys = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

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

    public void InitBoard()
    {
        SpawnSteppingTiles(transform.position + new Vector3(-6, 2.5f, -6));
        SpawnSelectingTiles(4,3,transform.position, new Vector3(-3, 2.5f, -3),new Vector3(3, 2.5f, 3));
        InitPlayerPieces();
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
                var netObj = Runner.Spawn(steppingTilePrefab, initPosition);
                steppingTiles[index] = netObj.GetComponent<SteppingTile>();
                int imageKey = rndKey[index];
                
                steppingTiles[index].SetImage(imageKey);
                TileInfo info = new TileInfo(ETileType.Stepping, index, imageKey);
                steppingTiles[index].Info = info;
                //GameManager.Instance.RPC_AddDictionary(netObj.Id,info);
                
                initPosition += direction * 2;
                index++;
            }

            return initPosition;
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
                
                var netObj = Runner.Spawn(selectingTilePrefab, offset + start + garo * x + sero * z);
                selectingTiles[index] = netObj.GetComponent<SelectingTile>();
                //GameManager.Instance.RPC_AddDictionary(netObj.Id, new TileInfo(ETileType.Selecting, index, imageKey));
                selectingTiles[index].SetImage(imageKey);
            }
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

            SpawnPlayer(player, tile.transform.position + transform.position);
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