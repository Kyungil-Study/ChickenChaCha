using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    [Networked] public int Score { get; set; }
    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddScore()
    {
        Score++;
        Debug.Log(Score);
    }
    
    public void ColorChanged(MeshRenderer renderer, Color color)
    {
        renderer.material.color = color;
        Debug.Log("Who am I? " + color);
    }
    
    public override void FixedUpdateNetwork()
    {
        
    }
}
