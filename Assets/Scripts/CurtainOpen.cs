using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CurtainOpen : MonoBehaviour
{
    Animator anim;
    bool transition = false;
    
     
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("open", transition);
    }

    // Update is called once per frame

    public void MoveCurtain()
    {
        transition = !transition;
        anim.SetBool("open", transition);
        
    }

}
