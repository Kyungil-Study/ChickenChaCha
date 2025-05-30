using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mText;
    [SerializeField] private float mShowTime = 2f;
    [SerializeField] private float mOffset = 2.5f;
    
    private Transform mTarget;
    private Camera mMainCam;

    private void Start()
    {
        mMainCam = Camera.main;
    }
    
    public Vector3 GetTargetPosition()
    {
        return mTarget != null ? mTarget.position + Vector3.up * mOffset : Vector3.zero;
    }

    public void SetFollowTarget(Transform target)
    {
        mTarget = target;
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
        if (mTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(mTarget.position + Vector3.up * mOffset);
            transform.position = screenPos;
        }
        
        // UI가 항상 위를 보게 하려면 추가
        //if (mMainCam != null)
        //    transform.rotation = Quaternion.LookRotation(transform.position - mMainCam.transform.position);
    }
}
