using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultController : UISingleton<GameResultController>
{
    [Header("UI Refs")]
    [SerializeField] private GameObject mInGamePanel; // In-game panel to hide when the game ends
    [SerializeField] private TMP_Text mScoreText;
    [SerializeField] private Button mExitButton;

    private void Awake()
    {
        mExitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
        SessionManager.Instance.LeaveRoom();
    }
    
    public void UpdateResult(bool isWinner)
    {
        if (isWinner)
        {
            mScoreText.text = $"<color=blue>승리!</color>";
        }
        else
        {
            mScoreText.text = $"<color=red>패배!</color>";
        }
    }
    
    public void OnEndedGame(bool isWinner)
    {
        Debug.Log("OnEndedGame");
        mInGamePanel.SetActive(false);
        gameObject.SetActive(true);
        UpdateResult(isWinner);
        
    }
    
}
