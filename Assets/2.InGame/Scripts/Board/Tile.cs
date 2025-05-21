using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Tile : NetworkBehaviour
{
    public Tile next;
    public Tile prev;
    public int imageKey { get; set; } // TODO: Networked
    public PlayerRef player;
    public Renderer renderer;

    public Texture2D img;
    
    public void SetImage(int key)
    {
        renderer.material.mainTexture = LoadImage(imageKey);
        imageKey = key;
    }
    
    private Texture2D LoadImage(int key)
    {
        return Resources.Load<Texture2D>($"Tile/{key + 1:00}"); // TODO: change
    }
}