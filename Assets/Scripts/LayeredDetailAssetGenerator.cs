using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 414, CS0649  
public class LayeredDetailAssetGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D[] baseAlbedos;
    [SerializeField] private Texture2D[] baseSmoothness;
    [SerializeField] private Texture2D[] detailOneAlbedos;
    [SerializeField] private Texture2D[] detailOneSmoothness;
    [SerializeField] private Texture2D[] detailTwoAlbedos;
    [SerializeField] private Texture2D[] detailTwoSmoothness;
    [SerializeField] private Texture2D[] detailThreeAlbedos;
    [SerializeField] private Texture2D[] detailThreeSmoothness;
    [SerializeField] private Texture2D[] detailFourAlbedos;
    [SerializeField] private Texture2D[] detailFourSmoothness;
    [SerializeField] private Texture2D[] detailFiveAlbedos;
    [SerializeField] private Texture2D[] detailFiveSmoothness;
    [SerializeField] private Texture2D[] detailOneMasks;
    [SerializeField] private Texture2D[] detailTwoMasks;
    [SerializeField] private Texture2D[] detailThreeMasks;
    [SerializeField] private Texture2D[] detailFourMasks; 
    [SerializeField] private Texture2D[] detailFiveMasks;
    [SerializeField] private AnimationCurve baseDistribution;
    [SerializeField] private AnimationCurve detailOneDistribution;
    [SerializeField] private AnimationCurve detailTwoDistribution;
    [SerializeField] private AnimationCurve detailThreeDistribution;
    [SerializeField] private AnimationCurve detailFourDistribution;
    [SerializeField] private AnimationCurve detailFiveDistribution;

    [SerializeField] private Material baseMaterial;

    private void Start()
    {
        GenerateMaterial();
    }
    private void GenerateMaterial()
    {
        Material ins = Instantiate(baseMaterial);

        ins = SetTexture(ins, "_BaseAlbedo", "_BaseSmoothness", baseDistribution, baseAlbedos, baseSmoothness, false);
        ins = SetTexture(ins, "_DetailOneAlbedo", "_DetailOneSmoothness", detailOneDistribution, detailOneAlbedos, detailOneSmoothness, true);
        ins = SetTexture(ins, "_DetailTwoAlbedo", "_DetailTwoSmoothness", detailTwoDistribution, detailTwoAlbedos, detailTwoSmoothness, true);
        ins = SetTexture(ins, "_DetailThreeAlbedo", "_DetailThreeSmoothness", detailThreeDistribution, detailThreeAlbedos, detailThreeSmoothness, true);
        ins = SetTexture(ins, "_DetailFourAlbedo", "_DetailFourSmoothness", detailFourDistribution, detailFourAlbedos, detailFourSmoothness, true);
        ins = SetTexture(ins, "_DetailFiveAlbedo", "_DetailFiveSmoothness", detailFiveDistribution, detailFiveAlbedos, detailFiveSmoothness, true);
        ins = SetMask(ins, "_DetailOneMask", detailOneMasks);
        ins = SetMask(ins, "_DetailTwoMask", detailTwoMasks);
        ins = SetMask(ins, "_DetailThreeMask", detailThreeMasks);
        ins = SetMask(ins, "_DetailFourMask", detailFourMasks);
        ins = SetMask(ins, "_DetailFiveMask", detailFiveMasks);
        GetComponent<MeshRenderer>().material = ins;
    }

    private Material SetTexture(Material m, string albedoName, string smoothnessName, AnimationCurve curve, Texture2D[] possibleAlbedos, Texture2D[] possibleSmoothness, bool addExtra)
    {
        if(possibleAlbedos.Length > 0)
        {
            int index = GetIndexFromAnimationCurve(curve, possibleAlbedos.Length, addExtra);
            if(index != -1)
            {
                m.SetTexture(albedoName, possibleAlbedos[index]);
                if(possibleSmoothness.Length > 0)
                {
                    m.SetTexture(smoothnessName, possibleSmoothness[index]);
                }
            }
        }
        return m;
          
    }

    private Material SetMask(Material m, string maskName, Texture2D[] possibleMasks)
    {

        if (possibleMasks.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, possibleMasks.Length);
            m.SetTexture(maskName, possibleMasks[index]);
        }

        return m;
    }

    private int GetIndexFromAnimationCurve(AnimationCurve curve, int elements, bool addExtra)
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        float sample = curve.Evaluate(r);
        int amount = elements;
        if (addExtra) 
        {
            amount++;
        }
        float percentagePerIndex = 1 / (float)amount;
        float i = percentagePerIndex;
        int index = 0;
        while (sample > i)
        {
            i += percentagePerIndex;
            index++;
        }
        if (index < elements)
        {
            return index;
        }
        return -1;
    }
}
