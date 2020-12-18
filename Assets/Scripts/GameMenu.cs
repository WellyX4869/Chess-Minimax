using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void HandleBackToMainMenuButton()
    {
        Debug.Log("ANJAYY");
        SceneManager.LoadScene(0);
    }

    public void HandleRestart()
    {
        Debug.Log("ANJAYY");
        SceneManager.LoadScene(1);
    }
}
