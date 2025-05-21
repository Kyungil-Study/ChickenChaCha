using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Networked] public int Score { get; set; }
    private void Awake()
    {
        // 데이터 세이브 오브젝트에 데이터 가져오기
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void SetInstance()
    {
        Instance = this;
        Debug.Log(Instance.name);
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
    }
    
    public override void FixedUpdateNetwork()
    {
        
    }
}
