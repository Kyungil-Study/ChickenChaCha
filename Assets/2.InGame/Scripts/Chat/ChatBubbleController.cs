using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ChatBubbleController : MonoBehaviour
{
    [SerializeField] private List<ChatBubble> mBubbleList = new(); // 하이어라키에 미리 연결
    
    public static ChatBubbleController Instance { get; private set; }
    private Dictionary<string, ChatBubble> mBubbleDict = new(); // 닉네임 → ChatBubble

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(NetworkPlayer player)
    {
        RegisterPlayer(player.Name.ToString(), player.Ref.PlayerId, player.transform);
    }

    public void RegisterPlayer(string nickname, int index, Transform target)
    {
        if (index < 0 || index >= mBubbleList.Count)
        {
            return;
        }

        var bubble = mBubbleList[index];
        bubble.SetFollowTarget(target);
        bubble.gameObject.SetActive(false);
    
        Debug.Log($"ChatBubbleController RegisterPlayer {nickname} : {bubble}");
        mBubbleDict[nickname] = bubble;
    }
    
    public void ShowBubble(string nickname, string message)
    {
        Debug.Log($"ChatBubbleController ShowBubble {nickname} : {message}");
        if (mBubbleDict.TryGetValue(nickname, out var bubble))
        {
            Vector3 worldPos = bubble.GetTargetPosition();
            bubble.Show(worldPos, message);
        }
    }
}
