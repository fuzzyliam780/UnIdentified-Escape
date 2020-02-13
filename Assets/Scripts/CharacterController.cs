using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public bool debugMode = false;
    public GameObject topHalf;
    public GameObject weapon;

    public Rigidbody rb;
    public int acceleration = 1;
    public int walkSpeed = 10;
    public int backSpeed = 5;
    public int turnSpeed = 20;
    public int JumpHeight = 200;
    public int FallSpeed = 10;

    public float Yaw;
    public float Pitch;
    public float Speed_Yaw;
    public float Speed_Pitch;
    public float character_height = 2f;

    private bool grounded;
    private RaycastHit hit;

    void Start()
    {
        toggleMouseLock();
    }

    void Update()
    {
        grounded = isGrounded();
        if (debugMode)
        {
            Debug.Log("Grounded: " + grounded);
        }
        Vector3 v3 = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            acceleration = 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            acceleration = 1;
        }

        if (Input.GetKey(KeyCode.W))                        //Move Forward
        {
            v3 += topHalf.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))                        //Move Back
        {
            v3 -= topHalf.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))                        //Move Left
        {
            v3 -= topHalf.transform.right;
        }
        if (Input.GetKey(KeyCode.D))                        //Move Right
        {
            v3 += topHalf.transform.right;
        }

        v3.y = 0f;                                          //Prevent Random Vertical Movement
        rb.velocity = v3 * walkSpeed * acceleration;


        if (Input.GetKeyDown(KeyCode.Space) & grounded)     //If Space is down and we are gounded, jump up
        {
            rb.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
        }
        if(!grounded)
        {
            rb.AddForce(Vector3.down * FallSpeed);
        }
        

       


        if (!Cursor.visible) //Controls Mouse Movement
        {

            Yaw += Speed_Yaw * Input.GetAxis("Mouse X");
            Pitch -= Speed_Pitch * Input.GetAxis("Mouse Y");

            topHalf.transform.eulerAngles = new Vector3(Mathf.Clamp(Pitch, -30, 60), Rnd(Yaw), 0.0f);
        }
    }

    /**
     * Hold over from a camera controller I mage a while back, may not actually be necessary, but for now it just works.
     */
    float Rnd(float num) 
    {
        float result = Mathf.Round(num);
        float low = result - 0.001f;
        float high = result + 0.001f;

        if ((num > low && num < result) || (num > result && num < high)) return result;


        return num;
    }

    /**
     *  Checks if the character is grounded or not
     */
    private bool isGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, 0.5f * 2, 1 << 10))
        {
            if (debugMode)
            {
                Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.red);
            }
            return true;
        }
        else
        {
            if (debugMode)
            {
                Debug.DrawRay(transform.position, Vector3.down * (0.5f * 2), Color.blue);
            }
            return false;
        }

    }

    public static void toggleMouseLock()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
