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
    [SerializeField] private Renderer mRenderer;
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
    
    [Networked] private bool MbInitialized { get; set; } = false;
    public override void FixedUpdateNetwork()
    {
        if (MbInitialized == false)
        {
            transform.position +=  (2 * Vector3.down) * Runner.DeltaTime * 4;
            if (transform.position.y <= 2.5f)
            {
                MbInitialized = true;
            }
        }
    }
}