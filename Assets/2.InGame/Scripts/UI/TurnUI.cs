using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    // 로컬 플레이어 이름 저장
    [SerializeField] private TMP_Text mPlayerNameText;
    
    public void SetTurnPlayerName(string playerName)
    {
        mPlayerNameText.text = playerName;
    }
}
