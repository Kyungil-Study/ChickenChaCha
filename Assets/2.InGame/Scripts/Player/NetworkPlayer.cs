using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어 데이터, 애니메이션 등 처리하기

public interface IPlayerState
{
    void EnterState(NetworkPlayer player);
    void ExitState(NetworkPlayer player);
    void Update(NetworkPlayer player); // 상태별 로직 처리
}

public class ActiveState : IPlayerState
{
    public void EnterState(NetworkPlayer player)
    {
        Debug.Log($"[{player.Index}] : Active 진입");
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
        Debug.Log($"[{player.Index}] : Waiting 진입");
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

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft, IAfterSpawned
{
    [SerializeField] private Renderer mRenderer;
    public NetworkTransform networkTransform;
    public PlayerScoreUI scoreUI;
    public InputHandler inputHandler;
    public GameObject[] tailModels;
    public IPlayerState currentState;
    public bool bHasLeft = false;
    
    [Networked] public PlayerRef Ref { get; set; }
    [Networked] public int Index { get; set; }

    [Networked]
    [OnChangedRender(nameof(OnChangedTailCount))]
    public int TailCount { get; set; } // 꼬리 개수, OnChangedRender로 변경 감지

    public string Name => $"Player {Ref.PlayerId.ToString()}";

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

    private void OnChangedTailCount()
    {
        for (int i = 0; i < TailCount; i++)
        {
            tailModels[i].SetActive(true);
        }

        for (int i = TailCount; i < tailModels.Length; i++)
        {
            tailModels[i].SetActive(false);
        }

        if (GameManager.Instance.CheckTail(TailCount))
        {
            Debug.Log("Winning!");
            RPC_Result(Ref);
        }
        else
        {
            Debug.Log("Continue playing...");
        }

        scoreUI.UpdateScore(TailCount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Result(PlayerRef player)
    {
        GameResultController.Instance.OnEndedGame(player);
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
        if (HasStateAuthority)
        {
            UIAdapter.Instance.SetLocalPlayerName($"{Ref.PlayerId}");
        }

        UIAdapter.Instance.RegisterPlayer(this);
        tailModels[0].SetActive(true);
        IEnumerator RegisterPlayer()
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.players[Index] = this;
            GameManager.Instance.playerCount++;
        }
    }

    public void AfterSpawned()
    {
        mRenderer.material = BoardManager.Instance.chickMats[Index];
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

    private bool mbIsWaitingTile = false;
    // 타일 선택 처리 (상태가 Active일 때만 처리)
    private void HandleTileSelected(SelectingTile tile)
    {
        if (currentState is ActiveState && mbIsWaitingTile == false)
        {
            RPC_StartWait(tile);
            var currentTile = GameManager.Instance.GetMatchTile(CurrentSteppingTile);
            var isSuccess = GameManager.Instance.OpenTile(currentTile, tile);
            if (isSuccess)
            {
                MoveTo(currentTile);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_StartWait(SelectingTile tile)
    {
        StartCoroutine(WaitTileAnimationCoroutine(tile));
    }

    private IEnumerator WaitTileAnimationCoroutine(SelectingTile tile)
    {
        NetworkPlayer local = Runner.GetPlayerObject(Runner.LocalPlayer).GetComponent<NetworkPlayer>();
        local.mbIsWaitingTile = true;

        tile.ShowFace();
        AnimatorStateInfo state = tile.anim.GetCurrentAnimatorStateInfo(0);
        while ((state.shortNameHash == SelectingTile.HIDE_FACE && state.normalizedTime > 0.95f) == false)
        {
            state = tile.anim.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        local.mbIsWaitingTile = false;
        yield break;
    }

    public void MoveTo(SteppingTile targetTile)
    {
        transform.position = targetTile.transform.position;
        GameManager.Instance.RPC_MoveTo(targetTile, CurrentSteppingTile, Runner.LocalPlayer);
        CurrentSteppingTile = targetTile;
        LookAtNextTile();
    }

    private void LookAtNextTile()
    {
        transform.forward = CurrentSteppingTile.Next.transform.position - CurrentSteppingTile.transform.position;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_LookAtNextTile()
    {
        LookAtNextTile();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ReceiveMovePermission(bool allowed)
    {
        if (allowed)
        {
            SetState(new ActiveState());
        }
        else
        {
            SetState(new WaitingState());
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TeleportTo(Vector3 position)
    {
        networkTransform.Teleport(position);
    }

    public void PlayerLeft(PlayerRef player)
    {
        var leftPlayer = Runner.GetPlayerObject(player).GetComponent<NetworkPlayer>();
        leftPlayer.bHasLeft = true;
        leftPlayer.mRenderer.material.shader = BoardManager.Instance.soulShader;
    }
}