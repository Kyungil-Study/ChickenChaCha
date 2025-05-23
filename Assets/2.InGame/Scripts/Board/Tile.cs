using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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