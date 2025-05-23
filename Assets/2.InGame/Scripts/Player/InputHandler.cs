using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
public class InputHandler : NetworkBehaviour
{
    [SerializeField] private LayerMask clickLayer;
    
    public SelectingTile selectedTile;
    public bool bCanInput = false;
    
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
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            bClicked = true;
        }

        if (Input.GetKey(KeyCode.Alpha1))   // 단축키로 선택한 타일 확인하기
        {
            Debug.Log(selectedTile);
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
                Debug.Log($"[클릭한 타일] {hit.collider.name}");
                
                if (tile != null)
                {
                    selectedTile = tile;
                    GameManager.Instance.SendSelectedTile(selectedTile);
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
        GUI.Label(new Rect(20, 300, 250, 30), $"Click Handled: {bClicked}");
    }
}