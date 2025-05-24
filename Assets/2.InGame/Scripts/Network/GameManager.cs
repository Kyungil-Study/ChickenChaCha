using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>, IToNetwork, IPlayerJoined
{
    public NetworkPlayer[] players = new NetworkPlayer[4];
    public int playerCount = 0; // 현재 플레이어 수

    [Networked] private int ActivePlayerIndex { get; set; }

    private NetworkPlayer ActivePlayer
    {
        get => players[ActivePlayerIndex];
        set => ActivePlayerIndex = value.PlayerIndex;
    }
    private List<NetworkPlayer> mTailPlayers; // 여기 있는 애들한테 꼬리 뺐으면 됌.

    #region GameManager
    private void Update()
    {
        if (Runner.IsSharedModeMasterClient && Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }
    public NetworkPlayer[] GetPlayersArray()
    {
        return players;
    }
    
    private void GameStart()
    {
        Debug.Log("게임 시작");
        BoardManager.Instance.InitBoard(players);
        players[0].RPC_ReceiveMovePermission(true);
        ActivePlayer = players[0];
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OpenTileResult(bool result)
    {
        if (result)
        {
            Debug.Log("정답입니다.");
            // 액티브 플레이어에게 정답 처리
        }
        else
        {
            Debug.Log("오답입니다.");
            // 액티브 플레이어에게 오답 처리
            // 다음 턴으로 넘기기
            Debug.Log($"index : {ActivePlayer.PlayerIndex} / count : {playerCount}");
            ActivePlayer.RPC_ReceiveMovePermission(false);
            ActivePlayer = players[(ActivePlayer.PlayerIndex + 1) % playerCount];
            ActivePlayer.RPC_ReceiveMovePermission(true);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_MoveTo(SteppingTile targetTile, SteppingTile currentSteppingTile)
    {
        // 현재 타일, 다음 타일, 플레이어
        targetTile.StandingPlayer = Runner.LocalPlayer;
        currentSteppingTile.StandingPlayer = PlayerRef.None;

    }
    
    public void PlayerJoined(PlayerRef player)
    {
        // UI에게 새로 접속한 플레이어 정보 전달
        RPC_PlayerJoined(player);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerJoined(PlayerRef player)
    {
        
    }
    

    #endregion
    
    #region RuleManager
    
    // 1. 획득한 꽁지로 승리 판별 => 액티브 플레이어에게 1번의 정보를 리턴 후 꽁지 정보를 받아와 4개일 시 승리 판정 아닐시 게임 진행
    // ps. 승리 판정이 나면 다른 플레이어들은 패배 판정
    public bool CheckTail(int tailCount) //이 함수가 true면 승리, false면 진행 혹은 패배
    {
        // 사전 작업: PassPlayer로 꽁지 개수 플레이어에게 전달해주기
        if (tailCount >= 4) // 플레이어 꽁지 개수 확인, 후에 == 4로 변경 예정
        {
            return true; // 꽁지가 4개면 true 리턴
        }
        return false; //꽁지가 4개 미만이면 false 리턴
    }
    
    // 2. 액티브 플레이어에게 선택 타일을 전달 받고 뒤집어 보여주며 성공 / 실패 판별 => 액티브 플레이어에게 전달해줘야 됨
    // ps. 이를 토대로 이동 여부도 판정
    public bool OpenTile(SteppingTile tile, SelectingTile selectTileInfo)
    {
        //SteppingTile tile = null; //게임매니저에게서 발판 정보 받아오기
        //Tile selectTileInfo = null; //플레이어에게 선택 타일 정보 받아오기
        tile = GetMatchTile(tile);
        if (tile.IsSamePicture(selectTileInfo))
        {
            RPC_OpenTileResult(true);
            return true;
        }
        RPC_OpenTileResult(false);
        return false;
    }
    
    
    // 3. 뭘 맞춰야 하는지 확인 하는 코드
    public SteppingTile GetMatchTile(SteppingTile tile)
    {
        mTailPlayers = new List<NetworkPlayer>();
        while (tile.StandingPlayer != PlayerRef.None)
        {
            var netObj = Runner.GetPlayerObject(tile.StandingPlayer);
            Debug.Log(tile.StandingPlayer);
            Debug.Log(netObj);
            var netPlayer = netObj.GetComponent<NetworkPlayer>();
            mTailPlayers.Add(netPlayer);
            tile = tile.Next; // 있으면 그 다음 발판 확인
        }
        
        return tile;
    }

    // 4.  뺏은 꼬리 개수만큼 액티브 플레이어에게 추가
    public void TakeTails(NetworkPlayer netPlayer)
    {
        int takeCount = 0;
        foreach (var netplayers in mTailPlayers)
        {
            takeCount += netplayers.tailCount; // 꼬리 개수 합산
            netplayers.tailCount = 0; // 꼬리 개수 초기화
        }
        netPlayer.tailCount += takeCount; // 액티브 플레이어에게 꼬리 개수 추가
    }
    #endregion


}
