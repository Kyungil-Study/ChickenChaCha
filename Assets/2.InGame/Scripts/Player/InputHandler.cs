using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
public class InputHandler : NetworkBehaviour
{
    [SerializeField] private LayerMask clickLayer;
    
    [Networked] public bool bCanInput { get; set; } = false;

    // 타일 선택 콜백 처리 -> NetworkPlayer에게 보내기
    public Action<SelectingTile> OnTileSelected;
    
    private Camera camera;
    private bool bClicked;
    
    public override void Spawned()
    {
        camera = Camera.main;
        bClicked = false;
    }
    
    void Update()
    {
        if (bCanInput == false) return;   // 내 턴일때만 입력 가능하게 예외처리
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bClicked = true;
        }
    }
    public override void FixedUpdateNetwork()
    {
        SelectTileInfo();
    }
    
    public void SelectTileInfo()
    {
        if (bClicked)
        {
            bClicked = false; // 플래그 초기화

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickLayer))
            {
                var tile = hit.collider.gameObject.GetComponent<SelectingTile>();
                
                if (tile != null)
                {
                    // 콜백으로 처리 (직접 GameManager 호출 안 함)
                    OnTileSelected?.Invoke(tile);
                }
            }
            else
            {
                Debug.Log("[클릭 실패]");
            }
        }
    }
    
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 300, 250, 30), $"Clicked: {bClicked}");
    }
}