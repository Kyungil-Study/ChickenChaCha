using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UIPlayerScoreData
{
    public string playerName;
    public int playerScore;
}

public class ScoreBoardUI : MonoBehaviour
{
    [SerializeField] private PlayerScoreUI[] mPlayerScores;

    public void Update()
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
            return;
        
        //var playerInfos = gameManager.GetPlayersInfo();
        //UpdatePlayerScores(playerInfos);
    }

    public void UpdatePlayerScores(List<PlayerInfo> playerInfos)
    {
        if (mPlayerScores.Length < playerInfos.Count)
        {
            Debug.LogAssertion("PlayerScoreUI is not enough , scores count max is " + mPlayerScores.Length);
            return;
        }

        for (int i = 0; i < playerInfos.Count; i++)
        {
            var playerInfo = playerInfos[i];
            //mPlayerScores[i].UpdateUI(playerInfo.playerName, playerInfo.score);
        }
    }
    
}
