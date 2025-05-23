using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UIAdapter : MonoBehaviour, IToUI
{
    private static UIAdapter mInstance;
    public static UIAdapter Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<UIAdapter>();

                if (mInstance == null)
                {
                    Debug.LogError("UIAdapter couldn't be found, You should add it to the scene.");
                }
            }
            return mInstance;
        }
    }
    
    [SerializeField] private PlayerUI mPlayerUI;
    [SerializeField] private ScoreBoardUI mScoreBoardUI;
    [SerializeField] private TurnUI mTurnUI;

    public void SetLocalPlayerName(string playerName)
    {
        mPlayerUI.SetLocalPlayerName(playerName);
    }

    public void SetTurnPlayerName(string playerName)
    {
        mTurnUI.SetTurnPlayerName(playerName);
    }

    /*public void UpdatePlayerScore(List<UIPlayerScoreData> playerScores)
    {
        mScoreBoardUI.UpdatePlayerScores(playerScores);
    }*/
}
