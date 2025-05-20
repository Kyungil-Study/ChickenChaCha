using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Test_GameManager : NetworkBehaviour
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_NotifyAction(TileActionType actionType, PlayerRef who)
    {
        Debug.Log($"[GameManager] {who}가 {actionType} 버튼을 눌렀습니다!");

        // 행동 판단 → 턴 진행 등 로직 작성 가능
    }
}