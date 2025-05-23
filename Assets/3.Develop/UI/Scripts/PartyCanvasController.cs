using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PartyCanvasController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button createButton;
    [Header("SubPanels")]
    [SerializeField] private GameObject createPanel;
    [Header("WaitImages")]
    [SerializeField] private GameObject[] waitImage;
    
    [Header("NotifyText")]
    [SerializeField] private TextMeshProUGUI notifyText;
    
    private void Start()
    {
        // 버튼 클릭 시 각 패널을 토글하는 리스너 등록
        createButton.onClick.AddListener(OnInviteClicked);
    }

    private void OnInviteClicked()
    {
        createPanel.SetActive(true);
    }

    private void OnSearchClicked()
    {
        createPanel.SetActive(false);
    }

    public void ExitSubPanel()
    {
        createPanel.SetActive(false);
    }

    public void OnWaitState()
    {
        // waitImage : 파티원에 따라 이미지 SetActive(true) 시킬 수 있도록
        // notifyText : 파티원 수에 따라 몇 명인지, 시간에 따라 ... 표시할 수 있도록
    }
}