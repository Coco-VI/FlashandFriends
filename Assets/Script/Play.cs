using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public void LoadScene(string MainMenu)
    {
        SceneManager.LoadScene(MainMenu);
    }
}