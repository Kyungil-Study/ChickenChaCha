using Fusion;
using UnityEngine;

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
    [SerializeField] private new Renderer mRenderer;
    [Networked] public TileInfo Info { get; set; }

    public override void Spawned()
    {
        SetImage(Info.imageKey);
    }

    public void SetImage(int key)
    {
        mRenderer.material.mainTexture = BoardManager.Instance.tileTextures[key];
        Info = new TileInfo(Info.type, Info.index, key);
    }

    public bool IsSamePicture(Tile tile)
    {
        return Info.imageKey == tile.Info.imageKey;
    }
}