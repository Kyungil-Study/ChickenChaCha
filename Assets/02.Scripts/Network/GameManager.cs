using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour, GameData, IPlayerLeft
{
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        // 데이터 세이브 오브젝트에 데이터 가져오기
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void InstanceNull(bool isMaster)
    {
        if (isMaster)
        {
            Instance = null;
        }
    }
    
    [Networked, OnChangedRender(nameof(SaveData))]
    public int Score { get; set; }
    
    public void SaveData()
    {
        RPC_SaveData();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SaveData()
    {
        // 데이터 세이브 오브젝트에 데이터 저장
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

    public void PlayerLeft(PlayerRef player)
    {
        
        Debug.Log("게임매니저 삭제됨");
        
    }
}
