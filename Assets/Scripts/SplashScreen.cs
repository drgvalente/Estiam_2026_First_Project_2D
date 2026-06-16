using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // necessary to load screens
using UnityEngine.UI; // necessary to manipulate UI (Slider)
using TMPro;

public class SplashScreen : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Time in seconds the SplashScreen will be visible")]
    public float splashDuration = 3f;

    [Header("Loading UI References")]
    public GameObject loadingPanel;   // The Panel that contains the bar and text
    public Slider loadProgressBar;    // The progress bar
    public TMP_Text loadProgressText; // Text to show "0%", "50%", etc.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Garantees that the load panel starts invisible
        loadingPanel.SetActive(false);

        // Starts the Coroutine that waits and changes scene
        StartCoroutine(StartGameFlow());
    }

    private IEnumerator StartGameFlow()
    {
        // 1. Wait for the splashDuration time
        yield return new WaitForSeconds(splashDuration);

        // 2. Activates the loading UI
        loadingPanel.SetActive(true);

        // 3. Starts the asinc loading of the scene "01_MainMenu"
        AsyncOperation asyncload = SceneManager.LoadSceneAsync("01_MainMenu");

        // Do not allow the scene changes automatically when finishes loading
        // That allow us ensure the bar reaches 100% before the change
        asyncload.allowSceneActivation = false;

        // 4. While the scene isn't finish loaded
        while (asyncload.progress < 0.9f)
        {
            // the progress goes from 0.0 to 0.9
            // we multiply for 100 to turn in percentage
            float progressValue = Mathf.Clamp01(asyncload.progress / 0.9f) * 100;

            // Update the progress bar and text
            loadProgressBar.value = progressValue / 100f;
            loadProgressText.text = Mathf.RoundToInt(progressValue).ToString() + "%";

            yield return null; // wait for 1 frame and repeat the loop
        }

        // 5. Loop is over, the load reached 90% (to Unity is 100% of the file)
        loadProgressBar.value = 1f;
        loadProgressText.text = "100%";

        // Wait for 1 more second seeing the 100% before change (optional)
        yield return new WaitForSeconds(1f);

        // 6. Authorizes the Scene change
        asyncload.allowSceneActivation = true;
    }

    private IEnumerator LoadMainMenuAfterDelay()
    {
        // wait for the time we defined
        yield return new WaitForSeconds(splashDuration);

        // load the scene index 1 (01_MainMenu in Build Profiles)
        SceneManager.LoadScene(1);
    }

}
