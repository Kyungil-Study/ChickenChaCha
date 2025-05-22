using Fusion;
using UnityEngine;

public interface IToPlayer
{
    /*
     * A, B, C, D 팀원들의 요청 사항 입니다.
     * 알파벳 부분을 본인의 담당 UI, Network, Map, Player + _이름 으로 설정해주세요
     * 예시) FromUI_홍길동, FromPlayer_홍길동
     */
    
    #region From 프로젝트 매니저_박승식
    // 매니저

    #endregion
    
    #region From 기술 리드_김우태
    // 기술

    #endregion
    
    #region FromA
    // 팀원 A

    #endregion
    
    #region FromB
    // 팀원 B

    #endregion
    
    #region FromC
    // 팀원 C

    #endregion
    
    #region FromRule_ 문현승

    public int CheckTailInfo();
    // 플레이어가 현재 가지고 있는 꽁지 개수를 리턴하는 메소드가 필요합니다.

    public void SelectTileInfo(GameObject tile); // 리턴값 타일
    // 플레이어가 현재 선택한 타일을 확인할 수 있는 메소드가 필요합니다.
    
    #endregion
}
