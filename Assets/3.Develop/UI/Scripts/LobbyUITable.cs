using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUITable : UISingleton<LobbyUITable>
{
   // 각 친구 메뉴로 갈수 있는 버튼를 참조하고 있음
   public UIGotoFriendMenu gotoFriendMenu => UIGotoFriendMenu.Instance;
   
   // 계정을 검색해서 친구를 찾고 추가 요청을 보낼 수 있는 메뉴
   public UIFriendRequestMenu friendRequestMenu => UIFriendRequestMenu.Instance;
   
   // 요청 받은 친구요청을 확인 수락할 수 있는 메뉴
   public UIFriendResponseMenu friendResponseMenu => UIFriendResponseMenu.Instance;
   
   // 친구 목록 표시 , 친구 목록에서 파티 초대할 수 있는 이벤트를 담고 있음
   public UIFriendRequestMenu friendListMenu => UIFriendRequestMenu.Instance;
}
