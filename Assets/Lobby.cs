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
        float width = Screen.width / 2;
        float height = Screen.height / 2;
        float x = (Screen.width - width) / 2;
        float y = (Screen.height - height) / 2;
        
        // 글씨 크기 스타일 정의
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 50 };
        GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField) { fontSize = 50 };
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 50 };

        GUILayout.BeginArea(new Rect(x, y, width, height));
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Build Index:", labelStyle, GUILayout.Width(width * 0.3f));
                mBuildIndex = GUILayout.TextField(mBuildIndex, 50, textFieldStyle, GUILayout.Width(width * 0.6f));
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Load Scene", buttonStyle, GUILayout.Width(width * 0.9f), GUILayout.Height(height * 0.2f)))
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
