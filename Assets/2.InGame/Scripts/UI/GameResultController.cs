using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class GameResultController : UISingleton<GameResultController>
{
    [Header("UI Refs")]
    [SerializeField] private GameObject mInGamePanel;
    [SerializeField] private GameObject mScorePanel;
    [SerializeField] private TMP_Text mScoreText;
    
    public void OnEndedGame(PlayerRef winner)
    {
        Debug.Log("OnEndedGame");
        mInGamePanel.SetActive(false);
        gameObject.SetActive(true);
        
    }
    
}
