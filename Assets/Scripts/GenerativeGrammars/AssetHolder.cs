using UnityEngine;

public class AssetHolder : MonoBehaviour
{
    [SerializeField] 
    Transform[] assetTransforms;

    //0 = lefthand, 1= righthand, 2= hatpos
    public Transform GetTransform(PositionAtCharacter pos) =>
        assetTransforms[(int)pos];
    

}

public enum PositionAtCharacter
{
    LEFTHAND = 0,
    RIGHTHAND = 1,
    HATPOS = 2
}
