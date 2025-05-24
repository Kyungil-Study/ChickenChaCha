using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class GameManager : DontDestroyOnNetwork<GameManager>
{
    public NetworkPlayer[] players;
    
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
        players[0].ReceiveMovePermission(true);
    }

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
        if (tile.Next.IsSamePicture(selectTileInfo))
        {
            return true;
        }
        return false;
    }
    
    private List<NetworkPlayer> tailPlayers; // 여기 있는 애들한테 꼬리 뺐으면 됌.
    // 3. 뭘 맞춰야 하는지 확인 하는 코드
    public SteppingTile GetMatchTile(SteppingTile tile)
    {
        while (tile.Next.StandingPlayer != PlayerRef.None) // 다음 발판의 사람이 있는지 여부
        {
            tailPlayers = new List<NetworkPlayer>();
            var netObj = Runner.GetPlayerObject(tile.Next.StandingPlayer);
            var netPlayer = netObj.GetComponent<NetworkPlayer>();
            if (netPlayer.tailCount != 0)
            {
                tailPlayers.Add(netPlayer);
            }
            
            tile = tile.Next; // 있으면 그 다음 발판 확인
        }
        return tile;
    }

    // 4.  뺏은 꼬리 개수만큼 액티브 플레이어에게 추가
    public void TakeTails(NetworkPlayer netPlayer)
    {
        netPlayer.tailCount += tailPlayers.Count;
        foreach (var netplayers in tailPlayers)
        {
            netplayers.tailCount--;
        }
    }
    #endregion
}
