using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    private bool _isLoading;

    protected override void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading)
            return;

        _isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneBuildIndex)
    {
        if (_isLoading)
            return;

        _isLoading = true;
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDestroy();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isLoading = false;
    }
}
