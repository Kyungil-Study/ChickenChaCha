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

    public void UpdatePlayerScores(List<UIPlayerScoreData> playerScores)
    {
        if (mPlayerScores.Length < playerScores.Count)
        {
            Debug.LogAssertion("PlayerScoreUI is not enough , scores count max is " + mPlayerScores.Length);
            return;
        }

        for (int i = 0; i < playerScores.Count; i++)
        {
            mPlayerScores[i].UpdateUI(playerScores[i].playerName, playerScores[i].playerScore);
        }
    }
    
}
