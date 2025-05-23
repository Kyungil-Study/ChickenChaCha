using System.Collections;
using System.Collections.Generic;
using Fusion;
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
        Debug.Log("[상태] Active 진입");
        player.inputHandler.bCanInput = true;
    }

    public void ExitState(NetworkPlayer player)
    {
        player.inputHandler.bCanInput = false;
    }

    public void Update(NetworkPlayer player)
    {
        // 정답 판별 후 상태 전환 요청 가능
    }
}


public class WaitingState : IPlayerState
{
    public void EnterState(NetworkPlayer player)
    {
        Debug.Log("[상태] Waiting 진입");
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
    public SelectingTile selectedTile;
    public InputHandler inputHandler;
    
    [SerializeField] private IPlayerState currentState;
    private int tailCount;

    public override void Spawned()
    {
        inputHandler = GetComponent<InputHandler>();
        SetState(new WaitingState()); // 초기 상태는 대기로
    }

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

    // 외부 매니저 클래스에서 상태 변경 가능하도록하는 메서드
    public void ReceiveMovePermission(bool allowed)
    {
        if (allowed)
            SetState(new ActiveState());
        else
            SetState(new WaitingState());
    }

    public int CheckTailInfo() => tailCount;
    public SelectingTile GetSelectedTile() => selectedTile;
    public void SetSelectedTile(SelectingTile tile) => selectedTile = tile;
    public void SelectTileInfo() { }
}
