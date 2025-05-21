using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>
{
    //public static GameManager Instance { get; private set; }
    
    [Networked] public int Score { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddScore()
    {
        Score++;
        Debug.Log(Score);
    }
    
    public void ColorChanged(MeshRenderer renderer, Color color)
    {
        renderer.material.color = color;
    }
    
    public override void FixedUpdateNetwork()
    {
        
    }
}
