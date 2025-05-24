using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

// 플레이어 데이터, 애니메이션 등 처리하기

public interface IPlayerState
{
    void EnterState(NetworkPlayer player);
    void ExitState(NetworkPlayer player);
    void Update(NetworkPlayer player);  // 상태별 로직 처리
}

public class ActiveState : IPlayerState
{
    public void EnterState(NetworkPlayer player)
    {
        Debug.Log($"[{player.PlayerIndex}] : Active 진입");
        player.inputHandler.bCanInput = true;
    }

    public void ExitState(NetworkPlayer player)
    {
        player.inputHandler.bCanInput = false;
    }

    public void Update(NetworkPlayer player)
    {
        // 정답 판별 후 상태 전환 요청하기
    }
}

public class WaitingState : IPlayerState
{
    public void EnterState(NetworkPlayer player)
    {
        Debug.Log("[상태] : Waiting 진입");
        if (player.inputHandler != null)
        {
            player.inputHandler.bCanInput = false;
        }
    }

    public void ExitState(NetworkPlayer player)
    {
        // Active로 바꿔야되나? 흠
    }

    public void Update(NetworkPlayer player)
    {
        // 대기 상태에서 특별한 동작 없을 수 있음
    }
}

public class NetworkPlayer : NetworkBehaviour//, IToPlayer
{
    [Networked] public PlayerRef Ref { get; set; }
    [Networked] public int PlayerIndex { get; set; }
    public NetworkTransform networkTransform;
    public int tailCount;
    public GameObject tailModel;// 꽁지 모델 관리 오브젝트
    [Networked, OnChangedRender(nameof(OnChangedTailCount))] public int NetworkedTailCount { get; set; }
    
    public InputHandler inputHandler;
    public IPlayerState currentState;

    [Networked] private int CurrentSteppingTileIndex { get; set; }
    public SteppingTile CurrentSteppingTile
    {
        get => BoardManager.Instance.steppingTiles[CurrentSteppingTileIndex];
        set => RPC_SetTileIndex(value.Info.index);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetTileIndex(int index)
    {
        CurrentSteppingTileIndex = index;
    }

    void OnChangedTailCount()
    {
        
    }

    private void Update()
    {
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(GameManager.Instance);
            GameManager.Instance.players[PlayerIndex] = this;
            GameManager.Instance.playerCount++;
            Debug.Log(GameManager.Instance.playerCount);
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        inputHandler = GetComponent<InputHandler>();
        networkTransform = GetComponent<NetworkTransform>();
        
        // 타일 선택 동작 위임 : 이벤트
        inputHandler.OnTileSelected = HandleTileSelected;
        SetState(new WaitingState()); // 초기 상태는 대기로

        StartCoroutine(RegisterPlayer());
    }
    private IEnumerator RegisterPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log($"playerIndex: {PlayerIndex}, playerRef: {Ref}");
        GameManager.Instance.players[PlayerIndex] = this;
        GameManager.Instance.playerCount++;
        Debug.Log($"playerCount : {GameManager.Instance.playerCount}");
    }
    
    // 상태 확장을 고려해서 플레이어 상태 변경
    public void SetState(IPlayerState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public override void FixedUpdateNetwork()
    {
        currentState?.Update(this);
    }

    // 타일 선택 처리 (상태가 Active일 때만 처리)
    private void HandleTileSelected(SelectingTile tile)
    {
        if (currentState is ActiveState)
        {
            var targetTile = GameManager.Instance.GetMatchTile(CurrentSteppingTile);
            bool isSuccess = GameManager.Instance.OpenTile(CurrentSteppingTile, tile);
            if (isSuccess)
            {
                MoveTo(targetTile);
            }
        }
    }
    
    // 외부 매니저 클래스에서 상태 변경 가능하도록하는 메서드
    public void MoveTo(SteppingTile targetTile)
    {
        transform.position = targetTile.transform.position;
        GameManager.Instance.RPC_MoveTo(targetTile, CurrentSteppingTile);
        CurrentSteppingTile = targetTile;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ReceiveMovePermission(bool allowed)
    {
        if (allowed)
            SetState(new ActiveState());
        else
            SetState(new WaitingState());
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TeleportTo(Vector3 position)
    {
        networkTransform.Teleport(position);
    }
    
    
}
