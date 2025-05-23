using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    [SerializeField] private UIAdapter mUIAdapter;
    // Start is called before the first frame update
    void Start()
    {
        mUIAdapter.SetTurnPlayerName("Player1");
        mUIAdapter.SetLocalPlayerName("Player2");
        
        List<UIPlayerScoreData> playerScores = new List<UIPlayerScoreData>();

        for (int i = 0; i < 4; i++)
        {
            playerScores.Add(new UIPlayerScoreData(){
                playerName = "Player" + (i + 1),
                playerScore = i * 10
            });
        }
        
        //mUIAdapter.UpdatePlayerScore(playerScores);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
