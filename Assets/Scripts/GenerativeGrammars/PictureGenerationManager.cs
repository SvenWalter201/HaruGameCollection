using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//using C:\Users\Ann - C\OneDrive\Desktop\UnityProject\HaruGameCollection\Assets\Scripts\LayeredDetailAssetGenerator.cs


public class PictureGenerationManager : Singleton<PictureGenerationManager>
{
    [SerializeField] private Texture2D[] eyes;
    [SerializeField] private Texture2D[] mouth;
    [SerializeField] private Texture2D[] hair;
    [SerializeField] private Transform right;
    [SerializeField] private Transform left;
    [SerializeField] private Transform backRright;
    [SerializeField] private Transform backLeft;
    [SerializeField] private Transform background;
    [SerializeField] private Transform foreground;
    [SerializeField] private Transform down;
    [SerializeField] private Transform up;


    //private float SCALE_OBJECT = 0.2f ;

    //[SerializeField] private Material baseMaterial;

    //component
    private MeshRenderer _renderHead;

    public GameObject charcter;
    public GameObject skyscraper;
    public GameObject bush;
    public GameObject tree;
    public GameObject house;
    public GameObject guitar;
    public GameObject car;
    public GameObject chefhat;
    public GameObject colourpalette;
    public GameObject Laterne;
    public GameObject paintbrush;
    public GameObject Vulcano;
    public GameObject pizza;
    public GameObject microphone;

     

    private GenerativeGrammatiken grammars;
    public SentenceInformation[] sentences = new SentenceInformation[5];
    public int count = 0;
    const int MAX_SENTENCES = 5;
    LayeredDetailAssetGenerator m;
    List<GameObject> presentGameObjects ;
    GameObject current;
    List<Bounds> colliderBounds = new List<Bounds>();


    public SentenceInformation[] Sentences { get => sentences; set => sentences = value; }

    private void Start()
    {
        presentGameObjects = new List<GameObject>();
        grammars = GenerativeGrammatiken.Instance;
    }

    //setting the informatioin sent by the grammar generator
    public void SendToPGMAnager(SentenceInformation si)

    {
        GetComponent<LayeredDetailAssetGenerator>();

        if(count < MAX_SENTENCES)
        {
            
            //Debug.Log(count);
            sentences[count] = si;
            //generate the new texture here
            PaintPicture(si);
           
            count++;
        }
        else
        {
            Debug.Log("Picture is full, dont generate more sentences");
            //clearPicture
            for (int i = 0; i < presentGameObjects.Count; i++)
            {
                Destroy(presentGameObjects[i]);
            }
            colliderBounds.Clear();
            presentGameObjects.Clear();
            grammars.ClearRecentlyUsed();
            for(int i= 0; i < MAX_SENTENCES; i ++)
            {
                Debug.Log(this.sentences[i].PrintToString());
            }
            count = 0;
        }
        
    }

    private void PaintPicture(SentenceInformation si)
    {
        // group or single object spawning
        if(si.Singular == false)
        {
            int r = UnityEngine.Random.Range(2, 5);
            for (int i = 0; i < r;i++)
            {
                PlaceObjectAt(si) ;
                Debug.Log(si.Singular + " mit " + i + " von " + r + "objekten");
            }
        }
        else
        {
            PlaceObjectAt(si);
        }

        ChangeMoodTo(si);
        SetActionTo(si);
        
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
            default:
                return Instantiate(bush, p, q);
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
            case "links": case "vorne links":
                {
                    position = left.position;
                    break;
                }
            case "rechts": case "vorne rechts":
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
        while (true && iter < max_iter) //while, colliding generate new position and check if its working
        {
            Vector2 pv = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 positionObject = new Vector3(pv.x, 0, pv.y) + position;
            GameObject gameObject = SpawnObject(si.Person, positionObject, Quaternion.identity);
            current = gameObject;
            current.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
            Collider c = current.GetComponent<Collider>();
            //Debug.Log(current.transform.position);
            //Debug.Log("Placing object!!!!");

            if (c != null) //check if object has collider
            {
                if (ValidateSpawn(c.bounds)) //check if object is intersecting with another object 
                {
                    Destroy(current);
                    iter++;
                }
                else
                {
                    colliderBounds.Add(c.bounds);
                    radius = radius + 0.2f;
                    break;
                }
                
            }
        }

        current.transform.localScale = new Vector3(1, 1, 1);

        presentGameObjects.Add(current);
    }
    
    private void SetActionTo(SentenceInformation si)
    {
        //_renderHead = current.GetComponent<MeshRenderer>();

        if (current.TryGetComponent<AssetHolder>(out AssetHolder ah) == true)
        {
            current.GetComponent<AssetHolder>().GetPosition(0);

            switch (si.Action)
            {
                case "musiziert":
                    SpawnObject("Guitar", current.GetComponent<AssetHolder>().GetPosition(0), Quaternion.identity);
                    break;
                case "singt":
                    SpawnObject("Microphone", current.GetComponent<AssetHolder>().GetPosition(0), Quaternion.identity);
                    break;
            }
        } 
        
    }
    public void ChangeMoodTo(SentenceInformation si)
    {
        // _renderHead = head.GetComponent<MeshRenderer>();

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
            switch (si.Mood)
            {


                case "erbost":
                    {
                        //set eyes to ...
                        //set mouth to...
                        _renderHead.material.SetTexture("_DetailTwoAlbedo", eyes[1]);
                        _renderHead.material.SetTexture("_DetailThreeAlbedo", mouth[1]);
                        break;
                    }
                case "aufgeret":
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
