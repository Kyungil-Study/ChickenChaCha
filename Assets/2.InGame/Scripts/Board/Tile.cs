using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum ETileType
{
    Stepping,
    Selecting
}

public struct TileInfo : INetworkStruct
{
    public ETileType type;
    public int index;
    public int imageKey;

    public TileInfo(ETileType type, int index, int imageKey)
    {
        this.type = type;
        this.index = index;
        this.imageKey = imageKey;
    }
}

public abstract class Tile : NetworkBehaviour
{
    private int ImageKey { get; set; }
    [SerializeField] private Renderer renderer;
    [Networked] public TileInfo Info { get; set; }

    public override void Spawned()
    {
        SetImage(Info.imageKey);
    }

    public void SetImage(int key)
    {
        renderer.material.mainTexture = BoardManager.Instance.tileTextures[key];
        ImageKey = key;
    }

    public bool IsSamePicture(Tile tile)
    {
        return ImageKey == tile.ImageKey;
    }
}