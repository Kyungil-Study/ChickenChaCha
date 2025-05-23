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

        if (Input.GetKey(KeyCode.Alpha1))   // 단축키로 선택한 타일 확인하기
        {
            Debug.Log(selectedTile);
        }
        
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E");
            NetworkObject netObj = GetComponent<NetworkObject>();
            NetworkId id = netObj.Id;
            
            
            GameManager.Instance.AddDictionary<PlayerInfo> (
                id, new PlayerInfo(Runner.LocalPlayer, false, 1, id)
            );
            Debug.Log("Added to dictionary: " + id);
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
        GUI.Label(new Rect(20, 300, 250, 30), $"Click Handled: {bClicked}");
    }
}