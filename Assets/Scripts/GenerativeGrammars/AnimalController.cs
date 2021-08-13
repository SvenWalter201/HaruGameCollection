using UnityEngine;
using UnityEngine.Playables;

public class AnimalController : MonoBehaviour
{

    Renderer mr;

    [SerializeField]
    Material animalBaseMaterial;


    void Awake()
    {
        mr = GetComponentInChildren<Renderer>();
        //CreateMaterial("ConstructionWorker");
    }


    public void CreateColor(string fileName)
    {
        //Debug.Log(Application.dataPath + "/Resources/Textures/ColorPalettes/" + fileName + ".png");
        if (FileManager.LoadObject("Textures/ColorPalettes", fileName, out Object tex))
        {
            Material ins = Instantiate(animalBaseMaterial);
            ins.SetTexture("MainTexture", (Texture2D)tex);

            Material[] mats = mr.materials;
            mats[0] = ins;
            mr.materials = mats;
        }
    }
}