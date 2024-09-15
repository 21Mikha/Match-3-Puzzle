using TMPro;
using UnityEngine;

public class MatchFeedback : MonoBehaviour
{
    public TextMeshPro feedbackText;
    public GameObject[] starBurstAnimations;
    public Animator feedbackAnimator;
    public Animator[] starBurstAnimator;
    private string[] cheeringPhrases =
    {
        "Wow!",
        "Great!",
        "Nice!",
        "Cool!",
        "Awesome!",
        "Bravo!",
        "Super!",
        "Yay!",
        "Well done!",
        "Perfect!",
        "Sweet!",
        "Amazing!"
    };
    //important to track the active coroutine
    private Coroutine currentFeedbackRoutine;  

    private void Awake()
    {
        feedbackText.gameObject.SetActive(false);
        foreach (var animation in starBurstAnimations)
        {
            animation.SetActive(false);
        }
    }

    public string GetRandomCheeringPhrase()
    {
        int randomIndex = Random.Range(0, cheeringPhrases.Length);
        return cheeringPhrases[randomIndex];
    }

    public void PlayFeedback(Slot slot)
    {
        //Stop the previous coroutine if it's still running
        if (currentFeedbackRoutine != null)
        {
            StopCoroutine(currentFeedbackRoutine);
        }

        //Set the text to a random phrase
        feedbackText.text = GetRandomCheeringPhrase();

        feedbackText.gameObject.SetActive(true);

        //Set the position of the feedback to match the slot's middle point
        transform.position = slot.middlePoint.position;

        //Trigger the animations
        feedbackAnimator.SetTrigger("PlayTextFeedback");
        foreach (var animation in starBurstAnimations)
        {
            animation.SetActive(true);
        }
        foreach (var animator in starBurstAnimator)
        {
            animator.SetTrigger("PlayStarBurst");
        }

        //Start the coroutine again
        currentFeedbackRoutine = StartCoroutine(DeactivateAfterAnimation());
    }

    //Coroutine to deactivate the text after the animation is finished
    private System.Collections.IEnumerator DeactivateAfterAnimation()
    {
        //Wait for the length of the animation
        yield return new WaitForSeconds(1f);

        //Deactivate
        feedbackText.gameObject.SetActive(false);
        foreach (var animation in starBurstAnimations)
        {
            animation.SetActive(false);
        }

        //Reset the coroutine reference
        currentFeedbackRoutine = null;
    }
}
