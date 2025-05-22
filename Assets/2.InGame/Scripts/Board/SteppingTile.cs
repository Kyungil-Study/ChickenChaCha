using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SteppingTile : Tile
{
    public SteppingTile Next { get; set; }
    public Tile Prev { get; set; }
    public PlayerRef StandingPlayer { get; set; }
}
