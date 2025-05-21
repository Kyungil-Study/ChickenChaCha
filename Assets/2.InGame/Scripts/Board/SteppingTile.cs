using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SteppingTile : Tile
{
    public Tile next;
    public Tile prev;
    public PlayerRef standingPlayer;
}
