using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RuleManager : DontDestroyOnNetwork<RuleManager>, IToRule
{
    private Tile mNextTile;
    /*
        할일 목록
        - 1. 플레이어를 앞질렀는지 판별( + 대기 플레이어의 꽁지 여부 확인)
        - 2. 획득한 꽁지로 승리 판별
        - 3. 액티브 플레이어에게 선택 타일을 전달 받고 뒤집어 보여주며 성공 / 실패 판별
    */
    
    /*
    public IToMap NextTlieInfo()
    // A : SteppingTile의 .Next 프로퍼티를 사용하세요.
    
    public PlayerRef OnPlayerInfo()
    // A : SteppingTile의 .StandingPlayer 프로퍼티를 사용하세요.

    public bool EqualTile(IToPlayer tile)
    // A : Tile의 .IsSamePicture(Tile tile) 메소드를 사용하세요.
    */
    
    // 1. 플레이어 앞질렀는지 판별 함수 => 앞질렀는지 여부를 액티브 플레이어에게 전달해줘야됨
    // ps. 앞지른 플레이어가 꽁지를 가지고 있는지도 확인해야 함 => 앞지른 플레이어의 꽁지 정보 받아오기
    public int PassPlayer() // 플레이어를 앞질렀는지 확인하고 앞질렀으면 가져가게 될 꽁지 개수 리턴;
    {
        int tail = 0;
        SteppingTile tile = null; // 다음 타일 정보
        while (true)
        {
            if (tile.Next.StandingPlayer != null) // 다음 발판의 사람이 있는지 여부
            {
                tail += tile.StandingPlayer.PlayerId /*임시로 지정, 나중에 꼬리 개수를 확인하는 변수로 변환 예정*/;
                //tile.StandingPlayer.PlayerId = 0; // **꽁지 개수 0으로 변경하는 코드 작성 필요**
                tile = tile.Next; // 있으면 그 다음 발판 확인
            }
            else // 위 과정 반복 후 플레이어가 없으면 그 발판 정보 가져오기
            { 
                mNextTile = tile.Next;
                break;
            }
        }
        
        return tail;  // 얻게 될 꽁지 수 리턴
    }
    
    // 2. 획득한 꽁지로 승리 판별 => 액티브 플레이어에게 1번의 정보를 리턴 후 꽁지 정보를 받아와 4개일 시 승리 판정 아닐시 게임 진행
    // ps. 승리 판정이 나면 다른 플레이어들은 패배 판정
    public bool CheckTail() //이 함수가 true면 승리, false면 진행 혹은 패배
    {
        IToPlayer CheckTailInfo = null;
        // 사전 작업: PassPlayer로 꽁지 개수 플레이어에게 전달해주기
        if (CheckTailInfo != null) // 플레이어 꽁지 개수 확인, 후에 == 4로 변경 예정
        {
            return true; // 꽁지가 4개면 true 리턴
        }
        return false; //꽁지가 4개 미만이면 false 리턴
    }
    
    // 3. 액티브 플레이어에게 선택 타일을 전달 받고 뒤집어 보여주며 성공 / 실패 판별 => 액티브 플레이어에게 전달해줘야 됨
    // ps. 이를 토대로 이동 여부도 판정
    public bool OpenTile()
    {
        SteppingTile tile = null; //게임매니저에게서 발판 정보 받아오기
        Tile SelectTileInfo = null; //플레이어에게 선택 타일 정보 받아오기
        if (tile.Next.IsSamePicture(SelectTileInfo))
        {
            return true;
        }
        return false;
    }
}
