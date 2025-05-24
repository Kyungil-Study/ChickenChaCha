using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SteppingTile : Tile
{
    public SteppingTile Next { get; set; }
    public SteppingTile Prev { get; set; }
    public PlayerRef StandingPlayer { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        if (Runner.IsSharedModeMasterClient == false)
        {
            BoardManager.Instance.steppingTiles[Info.index] = this;
        }
    }
}
