using UnityEngine;
using UnityEngine.SceneManagement;
// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public class SceneUI : MonoBehaviour
{
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 14;

        if (GUI.Button(new Rect(10, 10, 200, 45), "Mapa 1 - Prostopad³e", style))
            SceneManager.LoadScene("Scene1_Perpendicular");

        if (GUI.Button(new Rect(10, 65, 200, 45), "Mapa 2 - Równoleg³e", style))
            SceneManager.LoadScene("Scene2_Parallel");

        if (GUI.Button(new Rect(10, 120, 200, 45), "Mapa 3 - Dynamiczna", style))
            SceneManager.LoadScene("Scene3_Dynamic");

        // Wyœwietl aktualn¹ scenê
        GUI.Label(new Rect(10, 175, 300, 30),
            $"Aktualna scena: {SceneManager.GetActiveScene().name}");
    }
}