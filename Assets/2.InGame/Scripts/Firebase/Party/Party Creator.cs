// PartyCreator.cs
using System;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public class PartyCreator : MonoBehaviour
{
    public NetworkRunner Runner { get; private set; }

    private void Awake()
    {
        Runner = GameObject.Find("SessionManager").GetComponent<NetworkRunner>();
        Runner.ProvideInput = true;
    }

    public async Task CreateParty(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "FusionRoom_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        await Runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log("✅ 파티 생성 완료: " + roomName);
    }
}