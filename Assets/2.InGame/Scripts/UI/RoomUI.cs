using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RoomUI : UISingleton<RoomUI>
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button InviteButton;
    [SerializeField] Button ExitButton;
    [SerializeField] TMP_Text roomMessageText;
    
    private void Awake()
    {
        startGameButton.interactable = false;
        SessionManager.Instance.callbacks.OnFulledRoom += () =>
        {
            startGameButton.interactable = true;
        };
        if (SessionManager.Instance.IsMasterClient == false)
        {
            startGameButton.gameObject.SetActive(false);
        }
        
        startGameButton.onClick.AddListener(OnStartGame);
        ExitButton.onClick.AddListener(SessionManager.Instance.LeaveRoom);
    }
    
    public void RegistPlayer(NetworkPlayer player)
    {
        // 플레이어가 등록되면 게임 시작 가능 여부를 확인
        if (SessionManager.Instance.CanStartGame)
        {
            startGameButton.interactable = true;
        }
        SessionManager.GameRoomInfo roomInfo = SessionManager.Instance.RoomInfo;
        
        roomMessageText.text = $"Room:{roomInfo.roomName} # {roomInfo.roomPlayerCount} / {roomInfo.roomMaxPlayerCount}";
    }
    
    public void OnStartedGame()
    {
        // 게임이 시작되면 UI를 비활성화
        gameObject.SetActive(false);
    }

    private void OnStartGame()
    {
        if (SessionManager.Instance.CanStartGame == true )
        {
            GameManager.Instance.GameStart();
        }
    }
    
}
