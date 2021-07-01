using UnityEngine;
using System.Collections;
public class SimpleCharacterMovement : InputSource
{
    [SerializeField, Range(0f,100f)]
    float movementSpeed = 10f;

    public override void TestForInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        transform.position += new Vector3(x, 0, z) * movementSpeed * Time.deltaTime;
    }



}
