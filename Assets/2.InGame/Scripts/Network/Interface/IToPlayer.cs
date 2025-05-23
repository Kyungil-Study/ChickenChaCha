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

    public void ReceiveMovePermission(bool allowed); // 말 이동 가능한지 여부

    public void SetState(IPlayerState newState);    // 향후 확장을 고려해서 플레이어 상태 변경 가능하도록 작성한 메서드
    
    // 

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

    // 게임 매니저에 있는 데이터 가져가기로

    #endregion
}
