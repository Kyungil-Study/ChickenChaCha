using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UIAdapter : MonoBehaviour, IToUI
{
    private static UIAdapter instance;
    
    [SerializeField] private PlayerUI mPlayerUI;
    [SerializeField] private ScoreBoardUI mScoreBoardUI;
    
    public static UIAdapter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIAdapter>();

                if (instance == null)
                {
                    Debug.LogError("UIAdapter couldn't be found, You should add it to the scene.");
                }
            }
            return instance;
        }
    }

    public void SetLocalPlayerName(string playerName)
    {
        mPlayerUI.SetLocalPlayerName(playerName);
    }

    public void UpdatePlayerScore(List<UIPlayerScoreData> playerScores)
    {
        mScoreBoardUI.UpdatePlayerScores(playerScores);
    }
}
