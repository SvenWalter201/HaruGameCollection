using System.Collections;
using UnityEngine;

public class Tween : MonoBehaviour
{
    [SerializeField]
    Transform end;
    [SerializeField]
    float time = 3f;

    void Start()
    {
        StartCoroutine(TweenPosition(transform, end.position, end.rotation, time));
    }

    IEnumerator TweenPosition(Transform t, Vector3 endPos, Quaternion endRot , float time)
    {
        Vector3 startPos = t.position;
        Quaternion startRot = t.rotation;
        Vector3 dir = (endPos - t.position).normalized;
        float dis = Vector3.Distance(t.position, endPos);
        float currentTime = 0f;
        
        while (Vector3.Distance(t.position, endPos) > 0.1f)
        {
            currentTime += Time.fixedDeltaTime;
            float percentage = currentTime / time;
            Vector3 addedVector = dir * AdjustedSin(percentage) * dis;
            t.position = startPos + addedVector;
            t.rotation = Quaternion.SlerpUnclamped(startRot, endRot, percentage);
            yield return new WaitForFixedUpdate();
        }
        

        static float AdjustedSin(float x) => 
            0.5f * (Mathf.Sin(x * Mathf.PI - 0.5f * Mathf.PI) + 1f);
    
    }
}
