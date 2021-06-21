using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CurtainOpen : MonoBehaviour
{
    Animator anim;
    bool transition = false;
    float closedTimer;
    [SerializeField] float transitionTime = 5f;
    
     
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("open", transition);
    }

    // Update is called once per frame
    void Update()
    {
       /*
            closedTimer -= Time.deltaTime;
            if(closedTimer<= 0)
            {
                anim.SetBool("open", transition);
                closedTimer = transitionTime;
                transition = !transition;
            }
        */
    }

    public void MoveCurtain()
    {
        transition = !transition;
        anim.SetBool("open", transition);
        
    }

}
