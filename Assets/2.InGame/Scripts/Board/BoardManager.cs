using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BoardManager : DontDestroyOnNetwork<BoardManager>
{
    public GameObject steppingTilePrefab;
    public GameObject selectingTilePrefab;
    public Texture2D[] tileTextures;

    public SteppingTile[] steppingTiles = new SteppingTile[24];
    public SelectingTile[] selectingTiles = new SelectingTile[12];

    private readonly int[] imageKeys = new int[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

    public override void Spawned()
    {
        base.Spawned();
        StartCoroutine(LinkSteppingTiles());
    }

    private IEnumerator LinkSteppingTiles()
    {
        bool isAllReady = false;
        while (isAllReady == false)
        {
            yield return null;
            foreach (var tile in steppingTiles)
            {
                if (tile == null)
                {
                    break;
                }

                isAllReady = true;
            }
        }
        Debug.Log("All stepping tiles are ready. Linking...");

        int len = steppingTiles.Length;
        Debug.Log("Stepping tiles length: " + len);
        for (int i = 0; i < len; i++)
        {
            Debug.Log("Linking stepping tile " + i);
            int next = i == len - 1 ? 0 : i + 1;
            int prev = i == 0 ? len - 1 : i - 1;

            Debug.Log($"Next: {next} {steppingTiles[next] == null}");
            SteppingTile tile = steppingTiles[i];
            tile.Next = steppingTiles[next];
            tile.Prev = steppingTiles[prev];
        }

        Debug.Log("Linked all stepping tiles.");
    }

    public void InitBoard(NetworkPlayer[] players)
    {
        if (players.Length == 0)
        {
            players = FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.InstanceID);
        }
        SpawnSteppingTiles(transform.position + new Vector3(-6, 2.5f, -6));
        SpawnSelectingTiles(4, 3, transform.position, new Vector3(-3, 2.5f, -3), new Vector3(3, 2.5f, 3));
        InitPlayerPieces(players);
    }

    private void SpawnTile(GameObject prefab, Vector3 position, TileInfo info)
    {
        var netObj = Runner.Spawn(prefab, position, onBeforeSpawned: (runner, o) =>
        {
            Tile tile = o.GetComponent<Tile>();
            tile.Info = info;
        });
    }

    private void SpawnSteppingTiles(Vector3 zeroPosition)
    {
        int index = 0;

        int[] firstBag = RandomUtil.GetShuffled(imageKeys);
        int[] secondBag = RandomUtil.GetShuffled(imageKeys);
        var rndKey = firstBag.Concat(secondBag).ToArray();

        Vector3 position = zeroPosition;

        SpawnLine(Vector3.forward);
        SpawnLine(Vector3.right);
        SpawnLine(Vector3.back);
        SpawnLine(Vector3.left);

        void SpawnLine(Vector3 direction)
        {
            for (int i = 0; i < 6; i++)
            {
                int imageKey = rndKey[index];

                TileInfo info = new TileInfo(ETileType.Stepping, index, imageKey);
                SpawnTile(steppingTilePrefab, position, info);

                position += direction * 2;
                index++;
            }
        }
    }

    private void SpawnSelectingTiles(int columnCount, int rowCount, Vector3 offset, Vector3 start, Vector3 end)
    {
        Vector3 diff = end - start;
        Vector3 garo = Vector3.right * diff.x / (columnCount - 1);
        Vector3 sero = Vector3.forward * diff.z / (rowCount - 1);

        int[] rndKey = RandomUtil.GetShuffled(imageKeys);

        for (int x = 0; x < columnCount; x++)
        {
            for (int z = 0; z < rowCount; z++)
            {
                int index = z + x * rowCount;
                int imageKey = rndKey[index];

                TileInfo info = new TileInfo(ETileType.Selecting, index, imageKey);
                SpawnTile(selectingTilePrefab, offset + start + garo * x + sero * z, info);
            }
        }
    }

    private void InitPlayerPieces(NetworkPlayer[] players)
    {
        int playerCount = GameManager.Instance.playerCount;
        int div = steppingTiles.Length / playerCount;
        for (int i = 0; i < playerCount; i++)
        {
            SteppingTile tile = steppingTiles[i * div];
            NetworkPlayer player = players[i];

            if (player == null)
            {
                break;
            }

            player.RPC_TeleportTo(tile.transform.position);

            player.CurrentSteppingTile = tile;
            tile.StandingPlayer = player.Ref;
        }
    }

    public void SubscribeAllSelectingTiles(Action<Tile> callback)
    {
        foreach (var tile in selectingTiles)
        {
            tile.onClick += callback;
        }
    }
}