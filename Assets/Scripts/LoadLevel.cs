using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField]
    GameObject loadBar;
    [SerializeField]
    CanvasGroup loadScreen;

    // Start loading screen, activate loading screen
    public void LoadScene (int sceneIndex)
    {
        loadScreen.alpha = 1;
        loadScreen.blocksRaycasts = true;   // Block raycasts while loading
        StartCoroutine(LoadAsync(sceneIndex));
    }

    // Wait for scene to finish loading
    IEnumerator LoadAsync (int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            loadBar.transform.localScale = new Vector2(progress * 2f, loadBar.transform.localScale.y);
            yield return null;
        }
        
    }
}
