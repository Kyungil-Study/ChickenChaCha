using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SteppingTile : Tile
{
    public SteppingTile Next { get; set; }
    public SteppingTile Prev { get; set; }
    [Networked] public PlayerRef StandingPlayer { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        BoardManager.Instance.steppingTiles[Info.index] = this;
    }
}
