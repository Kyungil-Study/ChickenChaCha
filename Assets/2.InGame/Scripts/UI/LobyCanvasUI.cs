using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LobyCanvasUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button createButton;
    [SerializeField] private Button exitButton;
    [Header("WaitImages")]
    [SerializeField] private GameObject[] waitImage;
    [Header("PartyMember")]
    [SerializeField] private TextMeshProUGUI[] partyMemberText;
    
    
    [Header("게임 찾기")]
    public UnityEvent OnMatchClicked;
    [Header("게임 떠가니")]
    public UnityEvent OnLeaveClicked;
    
    [Header("Testing")]
    [SerializeField] private TMP_InputField roomNameText;
    [SerializeField] private TMP_InputField playerCountText;
    
    [SerializeField] GameObject mGotoFriendPanel;
    
    private void Start()
    {
        // 버튼 클릭 시 각 패널을 토글하는 리스너 등록
        createButton.onClick.AddListener(OnInviteClicked);
        exitButton.onClick.AddListener(Application.Quit);
        OnMatchClicked.AddListener(EnterMatchMaking);

        if (UserManager.Instance.User.IsAnonymous)
        {
            mGotoFriendPanel.SetActive(false);
        }
    }

    private void Update()
    {
    }

    private void EnterMatchMaking()
    {
        SessionManager.Instance.RoomNameForTesting = roomNameText.text;
        SessionManager.Instance.RoomMapPlayerCount = int.Parse(playerCountText.text);
        
        SessionManager.Instance.EnterMatchMakingAsync();
    }

    private void OnInviteClicked()
    {
        OnMatchClicked?.Invoke();
    }

    private void OnSearchClicked()
    {
    }

    public void ExitSubPanel()
    {
        OnLeaveClicked?.Invoke();
    }

    public void OnWaitState()
    {
        // waitImage : 파티원에 따라 이미지 SetActive(true) 시킬 수 있도록
        // notifyText : 파티원 수에 따라 몇 명인지, 시간에 따라 ... 표시할 수 있도록
    }

    public void SetPartyMemberText(string partyMemberArgs)
    {
        // 파티 멤버 바뀌면 멤버 패널 바뀔 수 있도록
    }
}