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
    [Networked]
    public int imageKey { get; set; }
    [SerializeField] private Renderer renderer;

    public void SetImage(int key, Texture2D img)
    {
        renderer.material.mainTexture = img;
        imageKey = key;
    }

    public bool IsSamePicture(Tile tile)
    {
        return imageKey == tile.imageKey;
    }
}