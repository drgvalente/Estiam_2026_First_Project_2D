using UnityEngine;
using UnityEngine.SceneManagement;

/*
This script goes as Component to the Canvas
and the Buttons call his methods (functions)
thru the OnClick event (set on each button)
*/

public class MainMenuController : MonoBehaviour
{
    // EXACT NAME of the game scene
    [SerializeField] private string gameSceneName = "02_GamePlay";

    //==============================
    // Called by BtnPlay
    //==============================
    public void OnPlayClicked()
    {
        // SceneManager loads the scene by name
        // The scene MUST be added to the Scenes in the File → Build Profiles

        SceneManager.LoadScene(gameSceneName);
    }

    //==============================
    // Called by BtnOptions
    //==============================
    public void OnOptionsClicked()
    {
        // for now, jus write in Console
        // Lets implement that later
        Debug.Log("Options - Implement Later!!");
    }

    //==============================
    // Called by BtnQuit
    //==============================
    public void OnQuitClicked()
    {
        // This Don't work on editor
        // Will work fine on the Build (.exe final)
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
