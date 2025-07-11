using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class MainMenu : MonoBehaviour
{

    public TMP_Text whiteButtonText;  // Assign in the Inspector
    public TMP_Text blackButtonText;
    public void PlayAsWhite()
    {
        //PlayerPrefs.SetInt("PlayerColor", 1); // White = 1\
        PlayerPrefs.SetInt("PlayerColor", 1); // 1 for White
        PlayerPrefs.SetInt("IsWhitePlayer", 1); // true (1)
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");  // Load chess scene
    }

    public void PlayAsBlack()
    {
        //PlayerPrefs.SetInt("PlayerColor", -1); // Black = -1
        PlayerPrefs.SetInt("IsWhitePlayer", 0); // false (0)
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");  // Load chess scene
    }
    void Start()
    {
        whiteButtonText.text = "Play as White";  // Update the button text
        blackButtonText.text = "Play as Black";
    }
}




