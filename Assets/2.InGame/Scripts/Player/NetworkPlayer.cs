using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// 플레이어 데이터, 애니메이션 등 처리하기

public class NetworkPlayer : NetworkBehaviour
{
    private int tailCount;
    public int CheckTailInfo()
    {
        // 나중에 꼬리 개수 리턴
        return tailCount;
    }
}