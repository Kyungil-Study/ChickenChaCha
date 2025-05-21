public interface IToMap
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

    public void NextTlieInfo(); //리턴값 타일
    // 앞에 있는 타일의 정보를 확인할 수 있는 메소드가 필요합니다.
    
    public bool OnPlayerInfo();
    // 타일 위 플레이어 여부를 확인할 수 있는 메소드가 필요합니다.

    public bool TileSeparate();
    // 같은 타일인지 확인하는 메소드가 필요합니다.
    
    #endregion
}
