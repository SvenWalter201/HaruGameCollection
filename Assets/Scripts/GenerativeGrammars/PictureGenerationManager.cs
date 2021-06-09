using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using C:\Users\Ann - C\OneDrive\Desktop\UnityProject\HaruGameCollection\Assets\Scripts\LayeredDetailAssetGenerator.cs


public class PictureGenerationManager : Game
{
    [SerializeField] private Transform right;
    [SerializeField] private Transform left;
    [SerializeField] private Transform backRright;
    [SerializeField] private Transform backLeft;
    [SerializeField] private Transform background;
    [SerializeField] private Transform foreground;
    [SerializeField] private Transform down;
    [SerializeField] private Transform up;


    //component
    private MeshRenderer _renderHead;

    [SerializeField] private Texture2D[] eyes;
    [SerializeField] private Texture2D[] mouth;
    [SerializeField] private Texture2D[] hair;
    public GameObject charcter;
    public GameObject skyscraper;
    public GameObject bush;
    public GameObject tree;
    public GameObject house;
    public GameObject guitar;
    public GameObject car;
    public GameObject colourpalette;
    public GameObject Laterne;
    public GameObject paintbrush;
    public GameObject Vulcano;
    public GameObject pizza;
    public GameObject microphone;
    public GameObject constructionworker;
    public GameObject girl;
    public GameObject grandma;
    public GameObject princess;
    public GameObject teacher;
    public GameObject chefhat;



    private GenerativeGrammatiken grammars;
    const int MAX_SENTENCES = 5;
    public SentenceInformation[] sentences = new SentenceInformation[MAX_SENTENCES];
    public int count = 0;
    LayeredDetailAssetGenerator m;
    List<GameObject> presentGameObjects;
    GameObject current;
    List<Bounds> colliderBounds = new List<Bounds>();
    readonly CoroutineTimer timer = new CoroutineTimer();

    [Space]
    [Header("Timing")]

    [SerializeField]
    float drawingTime = 5;
    [SerializeField]
    float showingTime = 20;

    [Space]
    [Header("UIElements")]


    [SerializeField]
    GameObject Panel;
    [SerializeField]
    GameObject ProgressBar;

    [SerializeField]
    TextMeshProUGUI modeText;
    [SerializeField]
    TextMeshProUGUI remainingTimeText;

    [SerializeField]
    Image progressBar, progressBarMask;


    //[Header("Minimum of " + MAX_SENTENCES + " sentences" )]
    [SerializeField]
    TextMeshProUGUI sentence1;
    [SerializeField]
    TextMeshProUGUI sentence2;
    [SerializeField]
    TextMeshProUGUI sentence3;
    [SerializeField]
    TextMeshProUGUI sentence4;
    [SerializeField]
    TextMeshProUGUI sentence5;

    //public SentenceInformation[] Sentences { get => sentences; set => sentences = value; }

    void Start()
    {
        PlayGame();
    }
    protected override IEnumerator Init()
    {
        //initialize UI-components
        //SprogressBar.enabled = false;
        modeText.text = "Meisterwerke Level:Senioren";
        remainingTimeText.text = "";
        if (Panel != null)
        {
            Panel.SetActive(true);
        }
        if (ProgressBar != null)
        {
            ProgressBar.SetActive(true);
        }
        presentGameObjects = new List<GameObject>();
        grammars = GenerativeGrammatiken.Instance;
        yield break;
    }

    protected override IEnumerator Execute()
    {
        PaintPicture();
        sentence1.text = grammars.PrintSentence();
        progressBar.enabled = true;
        yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
        progressBar.enabled = false;
        //yield return timer.SimpleTimer(drawingTime);
        PaintPicture();
        sentence2.text = grammars.PrintSentence();
        progressBar.enabled = true;
        yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
        progressBar.enabled = false;
        //yield return timer.SimpleTimer(drawingTime);
        PaintPicture();
        sentence3.text = grammars.PrintSentence();
        progressBar.enabled = true;
        yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
        progressBar.enabled = false;
        //yield return timer.SimpleTimer(drawingTime);
        PaintPicture();
        sentence4.text = grammars.PrintSentence();
        progressBar.enabled = true;
        yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
        progressBar.enabled = false;
        //yield return timer.SimpleTimer(drawingTime);
        PaintPicture();
        sentence5.text = grammars.PrintSentence();
        progressBar.enabled = true;
        yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
        progressBar.enabled = false;
        //yield return timer.SimpleTimer(drawingTime);

        Panel.SetActive(false);
        yield return timer.SimpleTimer(showingTime);
        yield break;
    }




    public void PaintPicture()
    {
        SentenceInformation si = grammars.GenerateSentence();
        // group or single object spawning
        if (si.Singular == false)
        {
            int r = UnityEngine.Random.Range(2, 5);
            for (int i = 0; i < r; i++)
            {
                PlaceObjectAt(si);
                //Debug.Log(si.Singular + " mit " + i + " von " + r + "objekten");
            }
        }
        else
        {
            PlaceObjectAt(si);
        }

        ChangePersonTo(si);
        ChangeMoodTo(si);
        ChangeActionTo(si);
        //ChangeColorTo(si)
        //Change

    }

    private GameObject SpawnObject(string placableObject, Vector3 p, Quaternion q)
    {

        switch (placableObject)
        {
            case "Wolkenkratzer":
                {
                    return Instantiate(skyscraper, p, q);
                }
            case "Haus":
                {
                    return Instantiate(house, p, q);
                }
            case "Baum":
                {
                    return Instantiate(tree, p, q);
                }
            case "Busch":
                {
                    return Instantiate(bush, p, q);
                }
            case "Guitar":
                {
                    return Instantiate(guitar, p, q);
                }
            case "Microphone":
                {
                    return Instantiate(microphone, p, q);
                }
            case "Laterne":
                {
                    return Instantiate(Laterne, p, q);
                }
            case "teacher":
                {
                    return Instantiate(teacher, p, q);
                }
            case "girl":
                {
                    return Instantiate(girl, p, q);
                }
            case "chefhat":
                {
                    return Instantiate(chefhat, p, q);
                }
            case "constructionworker":
                {
                    return Instantiate(constructionworker, p, q);
                }
            case "princess":
                {
                    return Instantiate(princess, p, q);
                }
            case "grandma":
                {
                    return Instantiate(grandma, p, q);
                }
            default:
                return Instantiate(charcter, p, q);
        }

    }

    //retruns true if collides, retuns false if spot is free for other object
    public bool ValidateSpawn(Bounds b)
    {
        for (int j = 0; j < colliderBounds.Count; j++)
        {
            if (colliderBounds[j].Intersects(b))
            {
                return true;
            }
        }
        return false;
    }


    private void PlaceObjectAt(SentenceInformation si)
    {
        float radius = 3f;


        Vector3 position;

        switch (si.Position)
        {
            case "links":
            case "vorne links":
                {
                    position = left.position;
                    break;
                }
            case "rechts":
            case "vorne rechts":
                {
                    position = right.position;
                    break;
                }
            case "im Vordergrung":
                {
                    position = foreground.position;
                    break;
                }
            case "im Hintergrund":
                {
                    position = background.position;
                    break;
                }
            case "hinten links":
                {
                    position = backLeft.position;
                    break;
                }
            case "hinten rechts":
                {
                    position = backRright.position;
                    break;
                }
            case "unten":
                {
                    position = down.position;
                    //gameObject.transform.localScale = new Vector3(SCALE_OBJECT, SCALE_OBJECT, SCALE_OBJECT);
                    break;
                }
            default:
                {
                    position = foreground.position;
                    break;
                }

        }


        int max_iter = 100;
        int iter = 0;
        while (iter < max_iter) //while, colliding generate new position and check if its working
        {
            Vector2 pv = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 positionObject = new Vector3(pv.x, 0, pv.y) + position;

            GameObject gameObject = SpawnObject(si.Subject.name, positionObject, Quaternion.identity);
            current = gameObject;
            current.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            Collider c = current.GetComponent<Collider>();
            //Debug.Log(current.transform.position);
            //Debug.Log("Placing object!!!!");

            if (c != null) //check if object has collider
            {
                if (ValidateSpawn(c.bounds)) //check if object is intersecting with another object 
                {
                    Destroy(current);
                }
                else
                {
                    colliderBounds.Add(c.bounds);
                    radius = radius + 0.2f;
                    break;
                }

            }
            else
            {
                Debug.LogError("Object has no Collider");
                break;
            }
            iter++;
        }

        current.transform.localScale = new Vector3(1, 1, 1);

        presentGameObjects.Add(current);
    }

    private void ChangeActionTo(SentenceInformation si)
    {
        //_renderHead = current.GetComponent<MeshRenderer>();

        if (current.TryGetComponent<AssetHolder>(out AssetHolder ah) == true)
        {
            Vector3 pos = current.GetComponent<AssetHolder>().GetPosition((int)PositionAtCharakter.LEFTHAND);

            switch (si.Action.name)
            {
                case "musiziert":
                    SpawnObject("Guitar", pos, Quaternion.identity);
                    break;
                case "singt":
                    SpawnObject("Microphone", pos, Quaternion.identity);
                    break;
            }
        }

    }

    public void ChangePersonTo(SentenceInformation si)
    {
        if (current.TryGetComponent<AssetHolder>(out AssetHolder ahSS) == true)
        {
            Vector3 pos = current.GetComponent<AssetHolder>().GetPosition((int)PositionAtCharakter.HATPOS);

            switch (si.Subject.name)
            {
                case "eine Lehrerin":

                    SpawnObject("teacher", pos, Quaternion.identity);
                    break;
                case "ein Bäcker":
                case "eine Bäckerin":
                    SpawnObject("chefhat", pos, Quaternion.identity);
                    break;
                case "ein Mädchen":
                    SpawnObject("girl", pos, Quaternion.identity);
                    break;
                case "eine Großmutter":
                    SpawnObject("grandma", pos, Quaternion.identity);
                    break;
                case "eine Prinzessin":
                    SpawnObject("princess", pos, Quaternion.identity);
                    break;
                case "ein Bauarbeiter":
                    SpawnObject("constructionworker", pos, Quaternion.identity);
                    break;
            }
        }
    }
    public void ChangeMoodTo(SentenceInformation si)
    {

        GameObject gameObject = current;
        //Debug.Log("Game object " + current);
        _renderHead = gameObject.GetComponent<MeshRenderer>();



        //change the gender/ hairstyle
        if (si.Gender == 'm')
        {
            _renderHead.material.SetTexture("_DetailOneAlbedo", hair[0]);
        }
        else
        {
            //set female hairstyle
            _renderHead.material.SetTexture("_DetailOneAlbedo", hair[1]);
        }

        //generate mood
        _renderHead.material.SetFloat("Vector1_e51c87a81dbc4eb997d73131d765a0b9", 0);
        _renderHead.material.SetFloat("Vector1_e51c87a81dbc4eb997d73131d765a0b9", 0);
        switch (si.Mood.name)
        {


            case "erbost":
                {
                    //set eyes to ...
                    //set mouth to...
                    _renderHead.material.SetTexture("_DetailTwoAlbedo", eyes[1]);
                    _renderHead.material.SetTexture("_DetailThreeAlbedo", mouth[1]);
                    break;
                }
            case "aufgeregt":
                {
                    //set mouth to...
                    //set eyes to ...
                    _renderHead.material.SetTexture("_DetailTwoAlbedo", eyes[1]);
                    _renderHead.material.SetTexture("_DetailThreeAlbedo", mouth[0]);
                    break;
                }
            case "erschrocken":
                {
                    //set eyes to ...
                    //set mouth to...
                    _renderHead.material.SetTexture("_DetailTwoAlbedo", eyes[0]);
                    _renderHead.material.SetTexture("_DetailThreeAlbedo", mouth[1]);
                    break;
                }
            case "freundlich":
                {
                    //set eyes to ...
                    //set mouth to...
                    _renderHead.material.SetTexture("_DetailTwoAlbedo", eyes[0]);
                    _renderHead.material.SetTexture("_DetailThreeAlbedo", mouth[0]);
                    break;
                }
            case "unförmig":
                {
                    _renderHead.material.SetFloat("Vector1_e51c87a81dbc4eb997d73131d765a0b9", 1);
                    break;
                }
            case "bunt":
                {
                    //_renderHead.material.SetFloat("Vector1_e51c87a81dbc4eb997d73131d765a0b9", 1);
                    break;
                }
        }

    }

}
