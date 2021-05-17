using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoroutineTimer
{
    /// <summary>
    /// Simple timer
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator SimpleTimer(float time)
    {
        float remainingTime = time;
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// A timer with UI components  (progressBar and countDown Text)
    /// </summary>
    /// <param name="time"></param>
    /// <param name="progressBar"></param>
    /// <param name="countDownText"></param>
    /// <returns></returns>
    public IEnumerator UITimer(float time, Image progressBarMask, TextMeshProUGUI countDownText)
    {
        float remainingTime = time;
        while (remainingTime > 0f)
        {
            progressBarMask.fillAmount = 1 - remainingTime / time;
            countDownText.text = Mathf.CeilToInt(remainingTime).ToString();


            remainingTime -= Time.deltaTime;
            yield return null;
        }
        countDownText.text = "";
        progressBarMask.fillAmount = 0f;
    }
}
