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
    [SerializeField] private CurtainOpen curtain;


    //component
    public GameObject 
        charcter,
        skyscraper,
        bush,
        tree,
        house,
        guitar,
        car,
        colourpalette,
        Laterne,
        brush,
        Vulcano,
        pizza,
        microphone,
        constructionworker,
        girl,
        grandma,
        princess,
        teacher,
        chefhat;



    private GenerativeGrammatiken grammars;
    const int MAX_SENTENCES = 5;
    public SentenceInformation[] sentences = new SentenceInformation[MAX_SENTENCES];
    public int count = 0;
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

    TextMeshProUGUI[] sentenceArray = new TextMeshProUGUI[MAX_SENTENCES];

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
        sentenceArray[0] = sentence1;
        sentenceArray[1] = sentence2;
        sentenceArray[2] = sentence3;
        sentenceArray[3] = sentence4;
        sentenceArray[4] = sentence5;
        yield break;
    }

    protected override IEnumerator Execute()
    {

        for(int i = 0; i < MAX_SENTENCES; i++)
        {
            PaintPicture();
            sentenceArray[i].text = grammars.PrintSentence();
            progressBar.enabled = true;
            yield return timer.UITimer(drawingTime, progressBarMask, remainingTimeText);
            progressBar.enabled = false;
            //yield return timer.SimpleTimer(drawingTime);
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
        //ChangeColorTo(si);

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
        if(si.Subject.scalable == true)
        {
            float randomX;
            randomX = current.transform.localScale.x * UnityEngine.Random.Range(0.8f , 1.15f );
            current.transform.localScale = Vector3.one * randomX;

        }
        //else
            //current.transform.localScale = new Vector3(1, 1, 1);

        //roatating
        if (si.Subject.rotatable == true)
        {

            float randomY = current.transform.rotation.eulerAngles.y + UnityEngine.Random.Range(-50, 50);
            Debug.Log(current.transform.rotation.eulerAngles.y + " + " + UnityEngine.Random.Range(-60, 60));
            current.transform.rotation = Quaternion.Euler(0,randomY,0);
        }

        presentGameObjects.Add(current);
    }

    private void ChangeActionTo(SentenceInformation si)
    {
        //_renderHead = current.GetComponent<MeshRenderer>();

        if (current.TryGetComponent<AssetHolder>(out AssetHolder ah) == true)
        {
            Vector3 posLeftHand = current.GetComponent<AssetHolder>().GetPosition((int)PositionAtCharakter.LEFTHAND);
            Vector3 posRightHand = current.GetComponent<AssetHolder>().GetPosition((int)PositionAtCharakter.RIGHTHAND);

            switch (si.Action.name)
            {
                case "musiziert":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    SpawnObject("guitar", posLeftHand, Quaternion.identity);
                    break;
                case "singt":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    SpawnObject("microphone", posRightHand, Quaternion.identity);
                    break;
                case "isst":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    SpawnObject("pizza", posRightHand, Quaternion.identity);
                    break;
                case "malt":
                    current.GetComponent<CharacterController>().LoadAndPlay(si.Action.animation);
                    SpawnObject("paintbrush", posRightHand, Quaternion.identity);
                    SpawnObject("colourpalette", posLeftHand, Quaternion.identity);
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

            }
        }

    }

    public void ChangePersonTo(SentenceInformation si)
    {
        if (current.TryGetComponent<AssetHolder>(out AssetHolder ah) == true)
        {
            Vector3 pos = current.GetComponent<AssetHolder>().GetPosition((int)PositionAtCharakter.HATPOS);

            GameObject asset = SpawnObject(si.Subject.asset, pos, Quaternion.identity);
            asset.transform.parent = current.transform;
            Debug.Log("setting texture");
            current.GetComponent<CharacterController>().CreateMaterial(si.Subject.texture);
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
                Debug.Log("looking for textures");
                current.GetComponent<CharacterController>().CreateFaceMaterial(si.Mood.textures);
            }
            
        }
        
    }

}
