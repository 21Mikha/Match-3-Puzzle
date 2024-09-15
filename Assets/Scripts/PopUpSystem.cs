using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PopUpSystem : MonoBehaviour
{
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI finishText;
    public Button retryButton;
    public float displayTime = 3f;
    public LevelManager levelManager;
    public Transition transition;

    private Animator winTextAnimator;
    private Animator loadingTextAnimator;
    private Animator loseTextAnimator;
    private Animator finishTextAnimator;

    private void Start()
    {
        
        winTextAnimator = winText.GetComponent<Animator>();
        loadingTextAnimator = loadingText.GetComponent<Animator>();
        loseTextAnimator = loseText.GetComponent<Animator>();
        finishTextAnimator = finishText.GetComponent<Animator>();

        //Initially hide all elements
        winText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        finishText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
    }


    // Winning Flow
    public void ShowWinningPopUp()
    {
        winText.text = "Congratulations!";
        loadingText.text = "Loading next level...";
        ResetAlpha(ref winText);
        ResetAlpha(ref loadingText);
        StartCoroutine(WinningFlow());
    }
    private IEnumerator WinningFlow()
    {
        // First, play transition in animation
        yield return StartCoroutine(transition.PlayTransitionIn());

        // Show win elements and play animations
        winText.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);

        winTextAnimator.SetTrigger("PlayAnimation");

        // Wait for display time
        yield return new WaitForSeconds(displayTime);

        // Fade out win elements by triggering the FadeOut animation
        winTextAnimator.SetTrigger("FadeOut");
        loadingTextAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(0.8f);  // Wait for fade-out animations to complete
        winText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(false);


            // Load next level before transition out
            levelManager.LoadLevelByIndex(++levelManager.currentLevelIndex);
            levelManager.HandleAfterTransition();

            // Play transition out animation after fading out elements
            yield return StartCoroutine(transition.PlayTransitionOut());


    }

    // Losing Flow
    public void ShowLosingPopUp()
    {
        loseText.text = "Time's Up! Level Lost";
        StartCoroutine(LosingFlow());
    }

    private IEnumerator LosingFlow()
    {
        // First, play transition in animation
        yield return StartCoroutine(transition.PlayTransitionIn());

        // Show lose elements and play animations
        loseText.gameObject.SetActive(true);
        loseTextAnimator.SetTrigger("PlayAnimation");
        retryButton.gameObject.SetActive(true);
        retryButton.onClick.AddListener(OnRetryButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        // Fade out lose elements
        loseTextAnimator.SetTrigger("FadeOut");
        retryButton.gameObject.SetActive(false);

        // Then play transition out and reload the level
        StartCoroutine(LosingTransitionOut());
    }

    private IEnumerator LosingTransitionOut()
    {
        loseText.gameObject.SetActive(false);
        // Load current level before transition out
        levelManager.LoadLevelByIndex(levelManager.currentLevelIndex);
        levelManager.HandleAfterTransition();
        yield return StartCoroutine(transition.PlayTransitionOut());
        loseText.gameObject.SetActive(false);
    }

    // Finish Game Flow (Thanking message)
    public void ShowFinishGamePopUp()
    {
        finishText.text = "Thanks for playing this Demo !";
        StartCoroutine(FinishGameFlow());
    }

    private IEnumerator FinishGameFlow()
    {
        // Play transition in animation
        yield return StartCoroutine(transition.PlayTransitionIn());

        // Show finish elements and play animation
        finishText.gameObject.SetActive(true);

    }

    void ResetAlpha(ref TextMeshProUGUI text)
    {
        Color color = text.color;  //Get the current color
        color.a = 1f;      //Set alpha to 1 (fully opaque)
        text.color = color;
    }
}
