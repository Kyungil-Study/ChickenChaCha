using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mText;
    [SerializeField] private float mShowTime = 2f;

    private Camera mMainCam;

    private void Awake()
    {
        mMainCam = Camera.main;
    }

    public void Show(Vector3 worldPos, string message)
    {
        mText.text = message;
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
        gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Hide), mShowTime);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // UI가 항상 위를 보게 하려면 추가
        if (mMainCam != null)
            transform.rotation = Quaternion.LookRotation(transform.position - mMainCam.transform.position);
    }
}
