using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header ("Main Menu")]
    public GameObject mainMenuContainer;

    [Header ("Loading Screen")]
    public GameObject loadingScreenContainer;
    public Image loadingBarFill;
    public float loadingSpeed;

    private void Start()
    {
        mainMenuContainer.SetActive(true);
        loadingScreenContainer.SetActive(false);
    }

    public void Play()
    {
        mainMenuContainer.SetActive(false);
        loadingScreenContainer.SetActive(true);
        loadingBarFill.fillAmount = 0;

        StartCoroutine(LoadSceneAsync(1));
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float loadingProgressValie = Mathf.Clamp01(operation.progress / loadingSpeed);
            loadingBarFill.fillAmount = loadingProgressValie;
            yield return null;
        }
    }
}
