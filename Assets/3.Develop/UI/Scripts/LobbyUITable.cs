using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 로비 관련 UI를 싱글톤으로 찾아서 접근 가능한 룩업 테이블 클래스
public static class LobbyUITable
{
   // 각 친구 메뉴로 갈수 있는 버튼를 참조하고 있음
   public static UIGotoFriendMenu gotoFriendMenu => UIGotoFriendMenu.Instance;
   
   // 계정을 검색해서 친구를 찾고 추가 요청을 보낼 수 있는 메뉴
   public static UIFriendRequestMenu friendRequestMenu => UIFriendRequestMenu.Instance;
   
   // 요청 받은 친구요청을 확인 수락할 수 있는 메뉴
   public static UIFriendResponseMenu friendResponseMenu => UIFriendResponseMenu.Instance;
   
   // 친구 목록 표시 , 친구 목록에서 파티 초대할 수 있는 이벤트를 담고 있음
   public static UIFriendRequestMenu friendListMenu => UIFriendRequestMenu.Instance;
}
