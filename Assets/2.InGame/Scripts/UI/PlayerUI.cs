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
    private static PlayerUI instance;
    public static PlayerUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerUI>();

                if (instance == null)
                {
                    Debug.LogError("PlayerUI couldn't be found, You should add it to the scene.");
                }
            }
            return instance;
        }
    }
    
    
    
    // 로컬 플레이어 이름 저장
    [SerializeField] private TMP_Text mPlayerNameText;
    
    public void SetLocalPlayerName(string playerName)
    {
        mPlayerNameText.text = playerName;
    }
}
