using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public float scaleValue = 1.25f;
    public float duration = 1f;
    public float rotation = 90;

    private Vector3 mOriginScale;
    private bool mBIsActive;
    
    private void Awake()
    {
        mOriginScale = transform.localScale;
        mBIsActive = true;
        StartCoroutine(co_ScaleUpAndDown());
    }

    private void OnEnable()
    {
        mBIsActive = true;
        StartCoroutine(co_ScaleUpAndDown());
    }
    private void OnDisable()
    {
        mBIsActive = false;
        StopCoroutine(co_ScaleUpAndDown());
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * rotation);
    }

    private IEnumerator co_ScaleUpAndDown()
    {
        while (mBIsActive)
        {
            // 크기 키우기
            yield return StartCoroutine(ScaleTo(mOriginScale * scaleValue));
            // 크기 복원
            yield return StartCoroutine(ScaleTo(mOriginScale));
        }
    }
    
    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
