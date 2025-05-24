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
    // 1. 플레이어 앞질렀는지 판별 함수 => 앞질렀는지 여부를 액티브 플레이어에게 전달해줘야됨
    // ps. 앞지른 플레이어가 꽁지를 가지고 있는지도 확인해야 함 => 앞지른 플레이어의 꽁지 정보 받아오기
    public int PassPlayer(SteppingTile tile) // 플레이어를 앞질렀는지 확인하고 앞질렀으면 가져가게 될 꽁지 개수 리턴;
    {
        int tail = 0;
        while (tile.Next.StandingPlayer != PlayerRef.None) // 다음 발판의 사람이 있는지 여부
        {
            tail++; /*임시로 지정, 나중에 꼬리 개수를 확인하는 변수로 변환 예정*/;
            //tile.StandingPlayer.PlayerId = 0; // **꽁지 개수 0으로 변경하는 코드 작성 필요**
            tile = tile.Next; // 있으면 그 다음 발판 확인
        }
        return tail;  // 얻게 될 꽁지 수 리턴
    }
    
    // 2. 획득한 꽁지로 승리 판별 => 액티브 플레이어에게 1번의 정보를 리턴 후 꽁지 정보를 받아와 4개일 시 승리 판정 아닐시 게임 진행
    // ps. 승리 판정이 나면 다른 플레이어들은 패배 판정
    public bool CheckTail(int tailCount) //이 함수가 true면 승리, false면 진행 혹은 패배
    {
        // 사전 작업: PassPlayer로 꽁지 개수 플레이어에게 전달해주기
        if (tailCount == 4) // 플레이어 꽁지 개수 확인, 후에 == 4로 변경 예정
        {
            return true; // 꽁지가 4개면 true 리턴
        }
        return false; //꽁지가 4개 미만이면 false 리턴
    }
    
    // 3. 액티브 플레이어에게 선택 타일을 전달 받고 뒤집어 보여주며 성공 / 실패 판별 => 액티브 플레이어에게 전달해줘야 됨
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

    // 4. 뭘 맞춰야 하는지 확인 하는 코드
    public SteppingTile GetMatchTile(SteppingTile tile)
    {
        while (tile.Next.StandingPlayer != PlayerRef.None) // 다음 발판의 사람이 있는지 여부
        {
            var netObj = Runner.GetPlayerObject(tile.Next.StandingPlayer);
            
            tile = tile.Next; // 있으면 그 다음 발판 확인
        }
        return tile;
    }
    #endregion
}
