using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerUI : MonoBehaviour
{
    // 로컬 플레이어 이름 저장
    [SerializeField] private TMP_Text mPlayerNameText;
    
    public void SetLocalPlayerName(string playerName)
    {
        mPlayerNameText.text = playerName;
    }
}
