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
    Material baseMaterial;

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
        if(JsonFileManager.LoadObject("Animations", fileName, out Object o))
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
        g.Stop();
        g.Destroy();
    }

    #endregion ANIM

    public void CreateMaterial(string fileName)
    {
        if(JsonFileManager.LoadPNG(Application.dataPath +  "/Resources/Textures/" + fileName + ".png", out Texture2D tex))
        {
            Material ins = Instantiate(baseMaterial);
            ins.SetTexture("MainTexture", tex);
            
            Material[] mats = mr.materials;
            mats[1] = ins;
            mr.materials = mats;
        }
    }

    public void InitializeCharacter(string textureFileName, string animationFileName)
    {
        CreateMaterial(textureFileName);
        LoadAndPlay(animationFileName);
    }
}
