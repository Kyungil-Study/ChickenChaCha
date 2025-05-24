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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnSteppingTiles(transform.position + new Vector3(-6, 2.5f, -6));
            SpawnSelectingTiles(4,3,transform.position, new Vector3(-3, 2.5f, -3),new Vector3(3, 2.5f, 3));
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

            int imgKey = rndKey[i % keyLength];
            steppingTiles[i].SetImage(imgKey, tileTextures[imgKey]);
            steppingTiles[i].Next = steppingTiles[i == steppingTiles.Length - 1 ? 0 : i + 1];
            steppingTiles[i].Prev = steppingTiles[i == 0 ? steppingTiles.Length - 1 : i - 1];
        }

        // selectingTiles
        rndKey = RandomUtil.GetShuffled(imageKeys);
        for (int i = 0; i < selectingTiles.Length; i++)
        {
            int imgKey = rndKey[i % keyLength];
            selectingTiles[i].SetImage(imgKey, tileTextures[imgKey]);
        }
    }

    public void SpawnSteppingTiles(Vector3 zeroPosition)
    {
        int index = 0;
        Vector3 firstEnd = SpawnLine(zeroPosition, Vector3.forward);
        Vector3 secondEnd = SpawnLine(firstEnd, Vector3.right);
        Vector3 thirdEnd = SpawnLine(secondEnd, Vector3.back);
        SpawnLine(thirdEnd, Vector3.left);

        Vector3 SpawnLine(Vector3 initPosition, Vector3 direction)
        {
            for (int i = 0; i < 6; i++)
            {
                var tile = Runner.Spawn(steppingTilePrefab, initPosition);
                steppingTiles[index++] = tile.GetComponent<SteppingTile>();
                
                initPosition += direction * 2;
            }

            return initPosition;
        }
    }

    public void SpawnSelectingTiles(int columnCount, int rowCount, Vector3 offset,Vector3 start, Vector3 end)
    {
        Vector3 diff = end - start;
        Vector3 garo = Vector3.right * diff.x / (columnCount - 1);
        Vector3 sero = Vector3.forward * diff.z / (rowCount - 1);
        for (int x = 0; x < columnCount; x++)
        {
            for (int z = 0; z < rowCount; z++)
            {
                var tile = Runner.Spawn(selectingTilePrefab, offset + start + garo * x + sero * z);
                selectingTiles[z + x * rowCount] = tile.GetComponent<SelectingTile>();
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