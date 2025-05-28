using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    Dictionary<NetworkPlayer, PlayerScoreUI> mPlayerScoreUIMap = new Dictionary<NetworkPlayer, PlayerScoreUI>();
    
    public void Update()
    {
        foreach ( KeyValuePair<NetworkPlayer,PlayerScoreUI> pair in mPlayerScoreUIMap)
        {
            pair.Value.UpdateUI(pair.Key.Name.ToString(), pair.Key.ScoreCount);
        }
    }

    public void BindPlayer(NetworkPlayer player)
    {
        int index = player.Index;
        mPlayerScoreUIMap.Add(player, mPlayerScores[index]);
        mPlayerScoreUIMap[player].UpdateUI(player.Name.ToString(), player.ScoreCount);
    }
}
