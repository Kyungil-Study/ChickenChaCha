using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class BoardManager : NetworkBehaviour
{
    public Tile[] steppingTiles;
    public Tile[] selectingTiles;
    public GameObject playerPrefab;
    
    private readonly int[] imageKeys = new int[12] {0,1,2,3,4,5,6,7,8,9,10,11};

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitTiles();
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
            steppingTiles[i].SetImage(rndKey[i % keyLength]);
            steppingTiles[i].next = steppingTiles[i == steppingTiles.Length - 1 ? 0 : i + 1];
            steppingTiles[i].prev = steppingTiles[i == 0 ? steppingTiles.Length - 1 : i - 1];
        }
        
        // selectingTiles
        rndKey = RandomUtil.GetShuffled(imageKeys);
        for(int i = 0; i < selectingTiles.Length; i++)
        {
            selectingTiles[i].SetImage(rndKey[i]);
        }
    }
    
    public void InitPlayerPiece()
    {
        PlayerRef[] players = Runner.ActivePlayers.ToArray();
        int playerCount = players.Length;

        int div = steppingTiles.Length / playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            Tile tile = steppingTiles[i * div];
            PlayerRef player = players[i];
            Runner.Spawn(playerPrefab, tile.transform.position, Quaternion.identity, player);
            tile.player = player;
        }
    }

    
}
