using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject Player;

    public Material normal;
    public Material hurt;
    public MeshRenderer MR;

    bool isHurt = false;
    public int hurtDuration = 5;
    public int hurtFrames;

    public int Health = 10;

    public float Speed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Character");
        hurtFrames = hurtDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if(isHurt)
        {
            hurtFrames--;
            if (hurtFrames == 0)
            {
                MR.material = normal;
                hurtFrames = hurtDuration;
                isHurt = false;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position,Player.transform.position, Speed * Time.deltaTime);

    }

    public void takeDamage()
    {
        Health--;
        if (Health <= 0)
        {
            GameManager.RemoveEnemy(transform.gameObject);
            UIManager.updateScore(5);
        }
        if (isHurt) return;
        MR.material = hurt;
        isHurt = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.layer == 11)
        {
            Destroy(go);
            takeDamage();
        }
    }
}
