using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionFade : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup instructionUI;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartFadeOut(float fadeSeconds)
    {
        StartCoroutine(FadeOut(fadeSeconds));
    }
    public void StartFadeIn(float fadeSeconds)
    {
        StartCoroutine(FadeIn(fadeSeconds));
    }

    IEnumerator FadeIn(float seconds)
    {
        float currentTime = 0;
        while(currentTime < seconds)
        {
            instructionUI.alpha = currentTime / seconds;
            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }
        instructionUI.alpha = 1;
    }

    IEnumerator FadeOut(float seconds)
    {
        float currentTime = seconds;
        while (currentTime > 0)
        {
            instructionUI.alpha = currentTime / seconds;
            currentTime -= Time.unscaledDeltaTime;
            yield return null;
        }
        instructionUI.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
