using UnityEngine;

public class animationScript : MonoBehaviour
{
    private Animator Anim;
    private GameObject mainOb;
    private AIMovement ai;
    private Vector3 velocity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Anim = GetComponent<Animator>();
        mainOb = transform.parent.gameObject;
        ai = mainOb.GetComponent<AIMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //Anim.SetBool("isMoving", true);
        /*
        Debug.Log("Velolcity X: " + ai.velocity.x + "| Velocity Y: " + ai.velocity.y + "| Velocity Z: " + ai.velocity.z);
        if(ai.velocity.x != 0 || ai.velocity.z != 0)
        {
            
        }
        else
        {
            Anim.SetBool("isMoving", false);
        }*/
    }
}
