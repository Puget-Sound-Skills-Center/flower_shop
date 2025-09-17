using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneButton : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

