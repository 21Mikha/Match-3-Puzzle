using UnityEngine;
using System;
using System.Collections;

public class Transition : MonoBehaviour
{
    public Material material;

    //Animation curves for both circles
    public AnimationCurve circle1Curve; 
    public AnimationCurve circle2Curve;

    public event Action OnScreenCovered;

    private bool hasTriggeredThresholdEvent = false;

    public void Awake()
    {
        hasTriggeredThresholdEvent = false; // Reset the trigger flag
        material.SetFloat("_Circle1Radius", 0);
        material.SetFloat("_Circle2Radius", 0);
    }
    public void PlayFullTransitionAnimation()
    {
        hasTriggeredThresholdEvent = false; 
        material.SetFloat("_Circle1Radius", 0);
        material.SetFloat("_Circle2Radius", 0);


        StartCoroutine(PlayTransitionIn(() =>
        {
            // When PlayTransitionIn completes -> trigger PlayTransitionOut
            StartCoroutine(PlayTransitionOut());
        }));
    }


    public IEnumerator PlayTransitionIn(Action onTransitionInComplete = null)
    {
        // Animate Circle1 radius from 0 to 1 over 2 seconds (using AnimationCurve)
        float circle1Radius = 0f;

        material.SetFloat("_Circle1Radius", 0);
        material.SetFloat("_Circle2Radius", 0);

        float duration1 = 2f;
        float elapsedTime = 0f;


        while (elapsedTime < duration1)
        {
            //non-linear interpolation
            float t = elapsedTime / duration1;
            circle1Radius = Mathf.Lerp(0f, 1f, circle1Curve.Evaluate(t)); 
            material.SetFloat("_Circle1Radius", circle1Radius); 

            // Check if Circle1 radius has gone above (0.99) and raise event
            if (!hasTriggeredThresholdEvent && circle1Radius >= 0.99f)
            {
                hasTriggeredThresholdEvent = true;
                OnScreenCovered?.Invoke(); 
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is exactly 1.2f (depending on screen size) --on mobile screen size (1) works fine so far...
        material.SetFloat("_Circle1Radius", 1.2f);

        onTransitionInComplete?.Invoke();
    }

    public IEnumerator PlayTransitionOut(Action onTransitionOutComplete = null)
    {
        material.SetFloat("_Circle1Radius", 1.2f);
        material.SetFloat("_Circle2Radius", 0);
        float circle2Radius = 0f;
        float duration2 = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration2)
        {
            float t = elapsedTime / duration2;
            circle2Radius = Mathf.Lerp(0f, 1f, circle2Curve.Evaluate(t)); 
            material.SetFloat("_Circle2Radius", circle2Radius);  
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        material.SetFloat("_Circle2Radius", 1.2f);

        onTransitionOutComplete?.Invoke();  
    }


}
