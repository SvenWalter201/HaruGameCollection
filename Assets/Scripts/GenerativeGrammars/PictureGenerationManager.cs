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
    private MeshRenderer _renderSkyscraper;
    private MeshRenderer _renderBush;
    private MeshRenderer _renderTree;
    private MeshRenderer _renderHouse;

    public GameObject head;
    public GameObject skyscraper;
    public GameObject bush;
    public GameObject tree;
    public GameObject house; 


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
        //GenerateMaterial();
    }
    /*
    private void GenerateMaterial()
    {
        Material ins = Instantiate(baseMaterial);

        //ins = SetTexture();
        GetComponent<MeshRenderer>().material = ins;
    }
    */ 

    //setting the informatioin sent by the grammar generator
    public void SendToPGMAnager(SentenceInformation si)

    {
        GetComponent<LayeredDetailAssetGenerator>();

        if(count < MAX_SENTENCES)
        {
            
            Debug.Log(count);
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
            for(int i= 0; i < MAX_SENTENCES; i ++)
            {
                Debug.Log(this.sentences[i].PrintToString());
                
            }
            count = 0;
        }
        
    }

    private void PaintPicture(SentenceInformation si)
    {
        /* group or single object spawning
        if(si.Singular = false)
        {

        }*/

        PlaceObjectAt(si);
        ChangeMoodTo(si);
        
    }

    private GameObject SpawnObject(string placableObject)
    {
        
        switch (placableObject)
        {
            case "Wolkenkratzer":
                {
                    return Instantiate(skyscraper);
                }
            case "Haus":
                {
                    return Instantiate(house);
                }
            case "Baum":
                {
                    return Instantiate(tree);
                }
            case "Busch":
                {
                    return Instantiate(bush);
                }
            default:
                return Instantiate(head);
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
        float radius = 5f;
        //float X = 0;
        //float Z = 0;

        GameObject gameObject = SpawnObject(si.Person);
        current = gameObject;

        Vector3 position;

        switch (si.Position)
        {
            case "links": case "vorne links":
                {
                    //Vector3 position = left.position + (UnityEngine.Random.insideUnitCircle * radius);
                    position = left.position;
                    //X = UnityEngine.Random.Range(left.position.x + radius, left.position.x - radius);
                    //Z = UnityEngine.Random.Range(left.position.z + radius, left.position.z + radius);
                    break;
                }
            case "rechts": case "vorne rechts":
                {
                    position = right.position;
                    //X = UnityEngine.Random.Range(right.position.x + radius, right.position.x - radius) ;
                    //Z = UnityEngine.Random.Range(right.position.z + radius, right.position.z - radius);
                    break;
                }
            case "im Vordergrung":
                {
                    position = foreground.position;
                    //X = UnityEngine.Random.Range(foreground.position.x + radius, foreground.position.x - radius);
                    //Z = UnityEngine.Random.Range(foreground.position.z + radius, foreground.position.z - radius);
                    break;
                }
            case "im Hintergrund":
                {
                    position = background.position;
                    //X = UnityEngine.Random.Range(background.position.x + radius, background.position.x - radius);
                    //Z = UnityEngine.Random.Range(background.position.z + radius, background.position.z - radius);
                    break;
                }
            case "hinten links":
                {
                    position = backLeft.position;
                    //X = UnityEngine.Random.Range(backLeft.position.x + radius, backLeft.position.x - radius);
                    //Z = UnityEngine.Random.Range(backLeft.position.z + radius, backLeft.position.z + radius);
                    break;
                }
            case "hinten rechts":
                {
                    position = backRright.position;
                    //X = UnityEngine.Random.Range(backLeft.position.x + radius, backLeft.position.x - radius);
                    //Z = UnityEngine.Random.Range(backLeft.position.z + radius, backLeft.position.z - radius);
                    break;
                }
            case "unten":
                {
                    position = down.position;
                    //X = UnityEngine.Random.Range(down.position.x + radius, down.position.x - radius);
                    //Z = UnityEngine.Random.Range(down.position.z + radius, down.position.z - radius);
                    //gameObject.transform.localScale = new Vector3(SCALE_OBJECT, SCALE_OBJECT, SCALE_OBJECT);
                    break;
                }
            default:
                {
                    position = foreground.position;
                    break;
                }

        }

        Collider c = current.GetComponent<Collider>();

        int max_iter = 100;
        int iter = 0;
        while (true && iter < max_iter) //while, colliding generate new position and check if its working
        {
            Vector2 pv = UnityEngine.Random.insideUnitCircle * radius;
            current.transform.position = new Vector3(pv.x, 0, pv.y) + position ;
            Debug.Log(current.transform.position);
            Debug.Log("Placing object!!!!");

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
                    break;
                }
                
            }
        }

        current.transform.localScale = new Vector3(1, 1, 1);

        presentGameObjects.Add(current);
    }
    
    private void SetActionTo(SentenceInformation si)
    {

    }
    public void ChangeMoodTo(SentenceInformation si)
    {
        // _renderHead = head.GetComponent<MeshRenderer>();

        GameObject gameObject = current;
        Debug.Log("Game object " + current);
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
                        _renderHead.material.SetFloat("Vector1_e51c87a81dbc4eb997d73131d765a0b9", 1);
                        break;
                    }
            }
        
    }

}
