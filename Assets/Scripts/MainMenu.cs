using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void HandleChangeDifficulty(int val)
    {
        GameSettings.difficulty = val + 1;
    }

    public void HandlePlayButton()
    {
        SceneManager.LoadScene(1);
    }
}
