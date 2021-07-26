using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WindowController : MonoBehaviour
{
    [SerializeField]
    WindowPair[] windowPairs;

    MeshFilter mF;
    MeshRenderer mR;

    void Start()
    {
        mF = GetComponent<MeshFilter>();
        mR = GetComponent<MeshRenderer>();
    }

    public IEnumerator OpenWindows(int index, float time)
    {
        float remainingTime = time;
        WindowPair pair = windowPairs[index];
        while(remainingTime > 0f)
        {
            pair.left.Rotate(new Vector3(0, 170f * 1/time * Time.deltaTime, 0));
            pair.right.Rotate(new Vector3(0, -170f * 1/time * Time.deltaTime, 0));
            remainingTime -= Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator CloseWindows(int index, float time)
    {
        float remainingTime = time;
        WindowPair pair = windowPairs[index];
        while (remainingTime > 0f)
        {
            pair.left.Rotate(new Vector3(0, -170f * 1 / time * Time.deltaTime, 0));
            pair.right.Rotate(new Vector3(0, 170f * 1 / time * Time.deltaTime, 0));
            remainingTime -= Time.deltaTime;
            yield return null;
        }
    }

    public void BeginShowPose(MemoryCardHouse card)
    {
        BodyDisplay.Instance.DisplayHumanoid(card.pose.motion[0]);
        Material[] mats = mR.materials;
        mats[card.index + 1].SetInt("_ShowTex", 1);
        mR.materials = mats;
    }

    public void StopShowPose(MemoryCardHouse card)
    {
        Material[] mats = mR.materials;
        mats[card.index + 1].SetInt("_ShowTex", 0);
        mR.materials = mats;
    }

    public void BeginOutline(MemoryCardHouse card)
    {
        Material[] mats = mR.materials;
        mats[card.index + 1].SetInt("_LightsOn", 1);
        mR.materials = mats;
    }

    public void StopOutline(MemoryCardHouse card)
    {
        Material[] mats = mR.materials;
        mats[card.index + 1].SetInt("_LightsOn", 0);
        mR.materials = mats;
    }

}

[System.Serializable]
public class WindowPair
{
    
    public Transform left, right;


}