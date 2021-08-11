using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour
{
    Renderer mr;
    AnimationClip clip;
    Animator anim;
    PlayableGraph g;

    [SerializeField]
    Material baseMaterial, faceMaterial;

    void Awake()
    {
        anim = GetComponent<Animator>();
        mr = GetComponentInChildren<Renderer>();
        //CreateMaterial("ConstructionWorker");
    }

    void OnDestroy() =>
        DisposeGraph();

    void OnApplicationQuit() =>
        DisposeGraph();

    #region ANIM

    public void LoadAndPlay(string fileName)
    {
        if(FileManager.LoadObject("Animations", fileName, out Object o))
        {
            clip = (AnimationClip)o;
            PlayCurrent();
        }
    }

    public void PlayCurrent()
    {
        AnimationPlayableUtilities.PlayClip(anim, clip, out PlayableGraph g);
        this.g = g;
    }

    void DisposeGraph()
    {
        if (g.IsValid())
        {
            g.Stop();
            g.Destroy();
        }
    }

    #endregion ANIM

    public void CreateMaterial(string fileName)
    {
        //Application.dataPath +  "/Resources/
        if (FileManager.LoadObject("Textures", fileName , out Object tex))
        {
            Material ins = Instantiate(baseMaterial);
            ins.SetTexture("MainTexture", (Texture2D)tex);
            
            Material[] mats = mr.materials;
            mats[1] = ins;
            mr.materials = mats;
        }
    }

    public void CreateFaceMaterial(string[] fileNames)
    {
        Texture2D[] textures = new Texture2D[fileNames.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            //Application.dataPath + "/Resources/
            if (FileManager.LoadObject("Textures/Facial", fileNames[i], out Object tex))
            {
                textures[i] = (Texture2D)tex;
            }
            else
            {
                Debug.LogWarning("Texture doesn't exist");
                return;
            } 
        }

        Material ins = Instantiate(faceMaterial);

        ins.SetTexture("_MOUTH", textures[0]);
        ins.SetTexture("_EYES", textures[1]);
        ins.SetTexture("_EYEBROWS", textures[2]);

        Material[] mats = mr.materials;
        mats[0] = ins;
        mr.materials = mats;
    }

    public void InitializeCharacter(string textureFileName, string animationFileName, string[] faceTextureFileNames)
    {
        CreateMaterial(textureFileName);
        CreateFaceMaterial(faceTextureFileNames);
        LoadAndPlay(animationFileName);
    }
}
