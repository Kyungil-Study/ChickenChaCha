using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileActionType
{
    Move,
    Stop
}

public class Test_TileAction : MonoBehaviour
{
    public TileActionType actionType = TileActionType.Stop;
}