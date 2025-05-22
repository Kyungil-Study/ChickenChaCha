using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    private string mBuildIndex = "";

    private void OnGUI()
    {
        // 중앙 위치 계산
        float width = 600;
        float height = 200;
        float x = (Screen.width - width) / 2;
        float y = (Screen.height - height) / 2;

        GUILayout.BeginArea(new Rect(x, y, width, height));
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Build Index:", GUILayout.Width(160));
                mBuildIndex = GUILayout.TextField(mBuildIndex, 50, GUILayout.Width(400));
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Load Scene", GUILayout.Width(560)))
            {
                if (int.TryParse(mBuildIndex, out int buildIndex))
                {
                    if (buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings)
                    {
                        SceneManager.LoadScene(buildIndex);
                    }
                    else
                    {
                        Debug.LogError("Invalid build index. Please enter a valid index.");
                    }
                }
                else
                {
                    Debug.LogError("Invalid input. Please enter a numeric build index.");
                }
            }
        }
        GUILayout.EndArea();
    }
}
