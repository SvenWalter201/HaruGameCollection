﻿using System;
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
    [SerializeField] private CurtainOpen curtain;
    [SerializeField] private GameObject gras;


    //component
    public GameObject
        character;



    private GenerativeGrammatiken grammars;
    const int MAX_SENTENCES = 5;
    public SentenceInformation[] sentences = new SentenceInformation[MAX_SENTENCES];
    public int count = 0;
    List<GameObject> presentGameObjects;
    GameObject current;
    List<Bounds> colliderBounds = new List<Bounds>();
    readonly CoroutineTimer timer = new CoroutineTimer();
    [SerializeField] float radius = 1.5f;

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

    [SerializeField] 
    PositionController positionController;


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

    TextMeshProUGUI[] sentenceArray = new TextMeshProUGUI[MAX_SENTENCES];

    //public SentenceInformation[] Sentences { get => sentences; set => sentences = value; }

    void Start()
    {
        
        PlayGame();
    }
    protected override IEnumerator Init()
    {
        //get the required resources from the virtual world
        if (!AppManager.useVirtualWorld)
        {
            gras.SetActive(false);
            positionController = VirtualWorldController.Instance.positionController;
        }

        modeText.text = StringRes.Get("_DuplikLevelSeniors");
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
        sentenceArray[0] = sentence1;
        sentenceArray[1] = sentence2;
        sentenceArray[2] = sentence3;
        sentenceArray[3] = sentence4;
        sentenceArray[4] = sentence5;
        yield break;
    }

    protected override IEnumerator Execute()
    {

        for (int i = 0; i < MAX_SENTENCES; i++)
        {
            PaintPicture();
            sentenceArray[i].text = grammars.PrintSentence();
            progressBar.enabled = true;
            yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
            progressBar.enabled = false;
        }

        Panel.SetActive(false);
        curtain.MoveCurtain();
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
            }
        }
        else
        {
            PlaceObjectAt(si);
        }

        ChangeActionTo(si);
        ChangeMoodTo(si);
        ChangePersonTo(si);
        ChangeColorTo(si);

    }

    private GameObject SpawnObject(string placableObject, Vector3 p, Quaternion q)
    {
        if(JsonFileManager.LoadObject("Prefabs",placableObject,out UnityEngine.Object o))
        {
            return Instantiate((GameObject)o, p, q);
        }
        else
        {
            Debug.LogWarning("No Object found");
            return null;
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


    //seting location, scale and rotaion
    private void PlaceObjectAt(SentenceInformation si)
    {


        Vector3 position;

        switch (si.Position)
        {
            case "links":
            case "vorne links":
                {
                    position = positionController.GetLeft.position;
                    break;
                }
            case "rechts":
            case "vorne rechts":
                {
                    position = positionController.GetRight.position;
                    break;
                }
            case "im Vordergrung":
                {
                    position = positionController.GetForeground.position;
                    break;
                }
            case "im Hintergrund":
                {
                    position = positionController.GetBackground.position;
                    break;
                }
            case "hinten links":
                {
                    position = positionController.GetBackLeft.position;
                    break;
                }
            case "hinten rechts":
                {
                    position = positionController.GetBackRight.position;
                    break;
                }
            case "unten":
                {
                    position = positionController.GetDown.position;
                    break;
                }
            default:
                {
                    position = positionController.GetForeground.position;
                    break;
                }

        }


        int max_iter = 200;
        int iter = 0;
        while (iter < max_iter) //while, colliding generate new position and check if its working
        {
            Vector2 pv = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 positionObject = new Vector3(pv.x, 0, pv.y) + position;

            GameObject gameObject = SpawnObject(si.Subject.model, positionObject, Quaternion.identity);
            current = gameObject;
            current.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            Collider c = current.GetComponent<Collider>();

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

        //scaling
        if(si.Subject.scalable)
        {
            float randomX;
            randomX = current.transform.localScale.x * UnityEngine.Random.Range(0.8f , 1.15f );
            current.transform.localScale = Vector3.one * randomX;

        }

        //roatating
        if (si.Subject.rotatable)
        {

            float randomY = current.transform.rotation.eulerAngles.y + UnityEngine.Random.Range(-50, 50);
            current.transform.rotation = Quaternion.Euler(current.transform.rotation.x,randomY, current.transform.rotation.z);
        }

        presentGameObjects.Add(current);
    }

    private void ChangeActionTo(SentenceInformation si)
    {
        //_renderHead = current.GetComponent<MeshRenderer>();

        if (current.TryGetComponent(out AssetHolder ah))
        {
            switch (si.Action.name)
            {
                case "musiziert":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    StartCoroutine(SpawnAtEndOfFrame("guitar", current, PositionAtCharacter.LEFTHAND));
                    break;
                case "singt":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    StartCoroutine(SpawnAtEndOfFrame("microphone", current, PositionAtCharacter.RIGHTHAND));
                    break;
                case "isst":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    StartCoroutine(SpawnAtEndOfFrame("pizza", current, PositionAtCharacter.RIGHTHAND));
                    break;
                case "malt":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    StartCoroutine(SpawnAtEndOfFrame("paintbrush", current, PositionAtCharacter.RIGHTHAND));
                    StartCoroutine(SpawnAtEndOfFrame("colourpalette", current, PositionAtCharacter.LEFTHAND));
                    break;
                case "läuft":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    break;
                case "wartet":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    break;
                case "überlegt":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    break;
                case "ist":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    break;

            }
        }

    }

    public void ChangePersonTo(SentenceInformation si)
    {
        if (current.TryGetComponent<AssetHolder>(out AssetHolder ah))
        {
            StartCoroutine(SpawnAtEndOfFrame(si.Subject.asset, current, PositionAtCharacter.HATPOS));
            //Debug.Log("setting texture");
            current.GetComponent<CharacterController>().CreateMaterial(si.Subject.texture);
        }
    }

    IEnumerator SpawnAtEndOfFrame(string asset, GameObject current, PositionAtCharacter p)
    {
        yield return new WaitForEndOfFrame();
        Transform t = current.GetComponent<AssetHolder>().GetTransform(p);
        Debug.Log(t.rotation.ToString());
        GameObject ins = SpawnObject(asset, t.position, t.rotation);
        ins.transform.parent = t;


    }

    public void ChangeColorTo(SentenceInformation si)
    {
        if (current.TryGetComponent<AnimalController>(out AnimalController ac))
        {
            Debug.Log("setting the colour");
            current.GetComponent<AnimalController>().CreateColor(si.Colour.name);
        }
    }


    public void ChangeMoodTo(SentenceInformation si)
    {
        if(si.Subject.type == "person")
        {
            GameObject gameObject = current;
            //_renderHead = gameObject.GetComponentInChildren<Renderer>();

            //generate mood
            if(si.Mood.textures != null)
            {
                //Debug.Log("looking for textures");
                current.GetComponent<CharacterController>().CreateFaceMaterial(si.Mood.textures);
            }
            
        }
        
    }

}
