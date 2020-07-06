using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to the transition object so can use OnTransitionComplete as an animation event for when transition ends

public class LoadLevel : MonoBehaviour
{
    [SerializeField]
    GameObject loadBar;
    [SerializeField]
    CanvasGroup loadScreen;
    AsyncOperation op;

    [SerializeField]
    Animator anim;
    static LoadLevel loadInstance;
    int nextScene = -1;
    Scene scene;

    public static LoadLevel LoadInstance { get { return loadInstance; } }

    void Awake()
    {
        //Singleton
        if (loadInstance == null)
        {
            loadInstance = this;
        }
        else
        {
            Destroy(loadInstance.gameObject);
            loadInstance = this;
        }
    }

    // Start loading screen, activate loading screen
    public void LoadScene (int sceneIndex)
    {
        // Save the scene index to load and play scene transition animation
        nextScene = sceneIndex;
        anim.Play("FadeOutToLoading");
    }

    // Load alternate
    public void LoadAlternateShop()
    {
        nextScene = 12;
        anim.Play("FadeOutToLoading");
    }

    // Once scene transition completed, fade into the loading screen
    public void ShowLoading()
    {
        loadScreen.alpha = 1;
        loadScreen.blocksRaycasts = true;   // Block raycasts while loading
    }

    // Once transition to loading screen finished, start loading next level
    public void StartLoading()
    {
        StartCoroutine(LoadAsync(nextScene));
    }

    // Once transition to next scene is finished, load the finished/almost ready scene
    public void OnTransitionToScene()
    {
        op.allowSceneActivation = true;
    }

    // Wait for scene to finish loading
    IEnumerator LoadAsync (int sceneIndex)
    {
        op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;
        bool hasTransitioned = false;

        while (!op.isDone)
        {
            // Progress stops at .9 when scene activation is false, activate transition here and the transition will activate OnTransitionToScene() at end of anim
            if (op.progress == .9f)
            {
                if (!hasTransitioned)
                {
                    hasTransitioned = true;
                    anim.Play("FadeOutToScene");
                }

                yield return null;
            }

            // Load scene and update loading bar
            float progress = Mathf.Clamp01(op.progress / .9f);
            loadBar.transform.localScale = new Vector2(progress * 2f, loadBar.transform.localScale.y);
            yield return null;
        }

        
    }
}
