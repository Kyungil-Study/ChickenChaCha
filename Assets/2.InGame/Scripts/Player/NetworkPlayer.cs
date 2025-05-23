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
        Debug.Log("[상태] : Active 진입");
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
        player.inputHandler.bCanInput = false;
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

public class NetworkPlayer : NetworkBehaviour, IToPlayer
{
    public InputHandler inputHandler;
    public IPlayerState currentState;
    
    public GameObject tailModel;// 꽁지 모델 관리 오브젝트
    [Networked, OnChangedRender(nameof(OnChangedTailCount))] public int networkedTailCount { get; set; }

    void OnChangedTailCount()
    {
        
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Runner.SetPlayerObject(Runner.LocalPlayer, Object); // 게임 매니저가 이 오브젝트(Player)를 찾을 수 있도록하는 코드
        }
        inputHandler = GetComponent<InputHandler>();
        
        // 타일 선택 동작 위임 : 이벤트
        inputHandler.OnTileSelected = HandleTileSelected;
        SetState(new WaitingState()); // 초기 상태는 대기로
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
            //GameManager.Instance.SendSelectedTile(tile);  // 인터페이스로 연결되어 있을 것
        }
    }
    
    // 외부 매니저 클래스에서 상태 변경 가능하도록하는 메서드
    public void MovePlayer(Vector3 position)
    {
        transform.position = position;
    }

    public void ReceiveMovePermission(bool allowed)
    {
        if (allowed)
            SetState(new ActiveState());
        else
            SetState(new WaitingState());
    }
    
}
