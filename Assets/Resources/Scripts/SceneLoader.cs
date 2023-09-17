using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadMenu() {
        SceneManager.LoadScene("MenuScene");
    }
    
    public static void LoadGame() {
        SceneManager.LoadScene("GameScene");
    }
    
    public static void LoadDebug() {
        SceneManager.LoadScene("DebugScene");
    }
}
