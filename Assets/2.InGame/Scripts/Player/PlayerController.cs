using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : NetworkBehaviour
{
    private Camera camera;
    [SerializeField] private LayerMask clickLayer;

    private int number = 0;

    public override void Spawned()
    {
        camera = Camera.main;
        // NetworkObject.AssignInputAuthority(PlayerRef);
        
        Debug.Log($"Object {Object.Id} (Player: {Runner.LocalPlayer}) Spawned. " +
                  $"Current InputAuthority: {Object.InputAuthority}, Current StateAuthority: {Object.StateAuthority}");
        
        if (Object.InputAuthority == PlayerRef.None)
        {
            Object.AssignInputAuthority(Runner.LocalPlayer);
        }

        Debug.Log($"Object {Object.Id} to Player {Runner.LocalPlayer}." +
                  $"InputAuthority: {Object.InputAuthority}, Current StateAuthority: {Object.StateAuthority}");
        
        // // --- 입력 권한 할당 시도 로직 ---
        // Debug.Log($"Object {Object.Id} (Player: {Runner.LocalPlayer}) Spawned. Current InputAuthority: {Object.InputAuthority}, Current StateAuthority: {Object.StateAuthority}");
        //
        // // 1. 현재 객체의 InputAuthority가 할당되지 않았는지 확인 (None)
        // if (Object.InputAuthority == PlayerRef.None)
        // {
        //     Debug.Log($"Object {Object.Id} has no InputAuthority. Checking if LocalPlayer ({Runner.LocalPlayer}) should claim it.");
        //
        //     // 2. 현재 클라이언트(Runner.LocalPlayer)가 이 객체의 상태 권한(StateAuthority)을 가지고 있는지 확인
        //     //    이것은 "이 객체는 내 것이다"라는 것을 판단하는 합리적인 근거가 될 수 있습니다.
        //     if (Object.StateAuthority == Runner.LocalPlayer)
        //     {
        //         Debug.Log($"LocalPlayer {Runner.LocalPlayer} has StateAuthority for Object {Object.Id}. Attempting to assign InputAuthority to self.");
        //
        //         // 3. 현재 클라이언트에게 입력 권한을 할당 시도
        //         Object.AssignInputAuthority(Runner.LocalPlayer);
        //
        //         if (success)
        //         {
        //             Debug.Log($"Successfully assigned InputAuthority for Object {Object.Id} to Player {Runner.LocalPlayer}. New InputAuthority: {Object.InputAuthority}");
        //         }
        //         else
        //         {
        //             // 이 경우는 드물지만, 동시에 다른 곳에서 권한 변경이 시도되거나 특정 조건으로 실패할 수 있습니다.
        //             Debug.LogWarning($"Failed to assign InputAuthority for Object {Object.Id} to Player {Runner.LocalPlayer}. Current InputAuthority remains: {Object.InputAuthority}");
        //         }
        //     }
        //     else
        //     {
        //         // 이 클라이언트는 이 객체의 상태 권한을 가지고 있지 않으므로, 입력 권한을 가져오려 시도하지 않는 것이 일반적입니다.
        //         // (다른 플레이어의 객체일 가능성이 높음)
        //         Debug.Log($"LocalPlayer {Runner.LocalPlayer} does not have StateAuthority for Object {Object.Id} (StateAuthority is {Object.StateAuthority}). Skipping InputAuthority assignment.");
        //     }
        // }
        // else
        // {
        //     // 이미 InputAuthority가 할당되어 있다면 추가 작업 불필요
        //     Debug.Log($"Object {Object.Id} already has InputAuthority: {Object.InputAuthority}. No action needed in Spawned for authority assignment.");
        // }
    }
    
    // void Update()
    // {
    //     if (Input.GetKey(KeyCode.Mouse0))
    //     {
    //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //         if (Physics.Raycast(ray, out RaycastHit hit, clickLayer))
    //         {
    //             Debug.Log(hit.collider.gameObject.name);
    //             number++;
    //         }
    //         else
    //         {
    //             Debug.Log("Raycast hit nothing");
    //         }
    //     }
    // }
    
    public override void FixedUpdateNetwork()
    {
        // BasicSpawner의 INetworkRunnerCallbacks를 구현한 OnInput 콜백에서 입력 수집을 관리한다면, 처리할 수 있는 코드
        // if (GetInput(out PlayerInputData inputData))
        // {
        //     if (inputData.bMouseClicked)
        //     {
        //         Debug.Log($"Player {Object.InputAuthority} (ID: {Object.Id}) clicked an object this tick!");
        //         // 여기에 클릭 시 수행할 로직을 추가합니다.
        //         // 예를 들어, inputData.ClickedObjectId를 사용해 특정 오브젝트와 상호작용
        //     }
        // }
        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit,Mathf.Infinity, clickLayer))
            {
                
                Debug.Log($"[{Runner.LocalPlayer}] 클릭 성공: {hit.collider.name}");
        
                // 예: 클릭한 오브젝트에 RPC 보내기
                var target = hit.collider.GetComponent<NetworkObject>();
                if (target != null)
                {
                    RPC_SelectTarget(target);
                }
                
                Debug.Log("입력 성공");
                number++;
            }
            else
            {
                Debug.Log("입력 실패");
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SelectTarget(NetworkObject target)
    {
        Debug.Log($"[서버] 타겟 선택됨: {target.name}");
        // 선택 처리 로직 예: 강조, 상태 저장 등
    }
    
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 200, 40), $"Last Clicked: {number.ToString()}");
    }
}
