using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
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

        //var playerRefs= gameManager.GetPlayersInfo();
        //var playerRefs = gameManager.GetPlayersInfo();
        //UpdatePlayerScores(playerInfos);
    }

    public void UpdatePlayerScores(List<PlayerRef> playerRefs)
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
            return;
        
        if (mPlayerScores.Length < playerRefs.Count)
        {
            Debug.LogAssertion("PlayerScoreUI is not enough , scores count max is " + mPlayerScores.Length);
            return;
        }

        // for (int i = 0; i < playerRefs.Count; i++)
        // {
        //     PlayerInfo? infoOrNull = gameManager.GetPlayerInfoOrNull(playerRefs[i]);
        //     if (infoOrNull.HasValue)
        //     {
        //         var playerInfo = infoOrNull.Value;
        //         mPlayerScores[i].UpdateUI(playerInfo.player.ToString(), playerInfo.score);
        //     }
        // }
    }
    
}
