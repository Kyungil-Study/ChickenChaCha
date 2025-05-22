using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct PlayerInputData : INetworkInput
{
    public bool bPressedSpace;
    public NetworkBool bMouseClicked; // 마우스 클릭 여부
    // 필요하다면 클릭된 오브젝트의 NetworkId 등을 추가할 수 있습니다.
    // public NetworkId ClickedObjectId;
}