using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SteppingTile : Tile, IAfterSpawned
{
    public SteppingTile Next ;
    public SteppingTile Prev ;
    public PlayerRef StandingPlayer { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        BoardManager.Instance.steppingTiles[Info.index] = this;
    }

    public void AfterSpawned()
    {
        var tiles = BoardManager.Instance.steppingTiles;
        int index = Info.index;
        int next = index == tiles.Length - 1 ? 0 : index + 1;
        int prev = index == 0 ? tiles.Length - 1 : index - 1;

        Debug.Log(tiles[next]);
        Next = tiles[next];
        // Prev = tiles[prev];
    }
}
