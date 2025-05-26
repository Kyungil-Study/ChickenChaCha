using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text mPlayerNameText;
    [SerializeField] private TMP_Text mPlayerScoreText;
    
    public void UpdateUI(string playerName, int playerScore)
    {
        UpdateName(playerName);
        UpdateScore(playerScore);
    }

    public void UpdateName(string newName)
    {
        mPlayerNameText.text = newName;
    }
    
    public void UpdateScore(int newScore)
    {
        mPlayerScoreText.text = newScore.ToString();
    }
}
