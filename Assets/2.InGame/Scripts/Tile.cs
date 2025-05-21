using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Tile next;
    public Tile prev;
    public bool bIsOccupied;
    public PlayerRef player;
    public int imageKey;
    
    private Renderer mRenderer;

    private void Awake()
    {
        mRenderer = GetComponent<Renderer>();
    }

    public void SetImage(int key)
    {
        // TODO: get image by key and change renderer's base map
        imageKey = key;
    }
}
