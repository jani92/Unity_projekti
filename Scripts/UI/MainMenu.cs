using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // New Gamella voisi olla joku lyhyt intro (ei tietenk‰‰n pakollista, kun aikaa on kaksi viikkoa j‰ljell‰). Silloin Level 1 painike voisi sen skipata.
    // Muuten niiss‰ voi k‰ytt‰‰ saman scenen numeroa.

    public void Play() {
        Debug.Log("Play-nappi toimii!");
    }

    public void NewGame() {
        SceneManager.LoadScene(1);
        Debug.Log("New Game -nappi toimii!");
    }

    public void Level1() {
        SceneManager.LoadScene(1);
        Debug.Log("Level 1 -nappi toimii!");
    }

    public void Level2() {
        SceneManager.LoadScene(2);
        Debug.Log("Level 2 -nappi toimii!");
    }

    public void Level3() {
        Debug.Log("Level 3 -nappi toimii!");
    }

    public void Options() {
        SceneManager.LoadScene(3);
        Debug.Log("Options-nappi toimii!");
    }

    public void Credits() {
        Debug.Log("Credits-nappi toimii!");

    }

    public void Exit() {
        Debug.Log("Exit-nappi toimii!");
        Application.Quit();
    }

}
