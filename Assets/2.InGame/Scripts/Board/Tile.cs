using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tile : NetworkBehaviour
{
    [Networked,OnChangedRender(nameof(SetImage))]
    public int imageKey { get; set; }
    [SerializeField] private Renderer renderer;

    public void SetImage()
    {
        renderer.material.mainTexture = LoadImage(imageKey);
    }
    
    private Texture2D LoadImage(int key)
    {
        return Resources.Load<Texture2D>($"Tile/{key + 1:00}"); // TODO: change
    }
}