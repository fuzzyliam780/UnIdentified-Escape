using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject topHalf;
    public GameObject weapon;

    public Rigidbody rb;
    public int walkSpeed = 10;
    public int backSpeed = 5;
    public int turnSpeed = 20;

    public float Yaw;
    public float Pitch;
    public float Speed_Yaw;
    public float Speed_Pitch;

    public bool MouseControlsView = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v3 = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            v3 += topHalf.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            v3 -= topHalf.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            v3 -= topHalf.transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            v3 += topHalf.transform.right;
        }

        rb.velocity = v3 * walkSpeed;

        if (MouseControlsView){

            Yaw += Speed_Yaw * Input.GetAxis("Mouse X");
            Pitch -= Speed_Pitch * Input.GetAxis("Mouse Y");

            topHalf.transform.eulerAngles = new Vector3(Mathf.Clamp(Pitch, -30, 60), Rnd(Yaw), 0.0f);
        }
    }

    float Rnd(float num)
    {
        float result = Mathf.Round(num);
        float low = result - 0.001f;
        float high = result + 0.001f;

        if ((num > low && num < result) || (num > result && num < high)) return result;


        return num;
    }
}
