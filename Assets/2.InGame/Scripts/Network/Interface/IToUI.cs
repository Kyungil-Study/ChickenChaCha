using System.Collections.Generic;

public interface IToUI
{
    /*
     * A, B, C, D 팀원들의 요청 사항 입니다.
     * 알파벳 부분을 본인의 담당 UI, Network, Map, Player + _이름 으로 설정해주세요
     * 예시) FromUI_홍길동, FromPlayer_홍길동
     */
    #region From 프로젝트 매니저_박승식
    // 매니저
    void SetLocalPlayerName(string playerName);
    void SetTurnPlayerName(string playerName);
    void UpdatePlayerScore(List<UIPlayerScoreData> playerScores);

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

    #region FromD

    // 팀원 D

    #endregion
}
