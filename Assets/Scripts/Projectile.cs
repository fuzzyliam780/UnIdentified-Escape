using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float drop = 2f;
    Vector3 v3;
    // Start is called before the first frame update
    void Start()
    {
        v3 = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        v3 += transform.forward * speed * Time.deltaTime;
        v3 += -transform.up * drop * Time.deltaTime;

        transform.position += v3;
    }
}
