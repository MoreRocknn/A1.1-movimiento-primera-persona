using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    Animator anim;
    public float forward;
    public float sideways;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {

    }
}
