using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageChanger : MonoBehaviour
{

    public Image image1;
    public Image image2;
    public Image image3;
    public Image image4;

    [SerializeField] private float glowTime = 4;
    [SerializeField] private float answerTime = 10;
    private void Start()
    {
        //Invoke("ChangeImage", 2);
        //StartCoroutine(Changing());    
    }

    IEnumerator Changing()
    {
        yield return new WaitForSeconds(glowTime);
        image1.sprite = Resources.Load<Sprite>("Blubs/Gl�hbirne") as Sprite;
        yield return new WaitForSeconds(glowTime);
        image1.sprite = Resources.Load<Sprite>("Blubs/Gl�hbrine_oh") as Sprite;
        image2.sprite = Resources.Load<Sprite>("Blubs/Gl�hbirne_whiteai") as Sprite;
        yield return new WaitForSeconds(glowTime);
        image2.sprite = Resources.Load<Sprite>("Blubs/Gl�hbrine_white_oh") as Sprite;
        image3.sprite = Resources.Load<Sprite>("Blubs/Gl�hbirne_red") as Sprite;
        yield return new WaitForSeconds(glowTime);
        image3.sprite = Resources.Load<Sprite>("Blubs/Gl�hbrine_red_oh") as Sprite;
        image4.sprite = Resources.Load<Sprite>("Blubs/Gl�hbirne_blueai") as Sprite;
        yield return new WaitForSeconds(glowTime);
        image4.sprite = Resources.Load<Sprite>("Blubs/Gl�hbrine_blue_oh") as Sprite;

        yield return new WaitForSeconds(answerTime);


    }
}
