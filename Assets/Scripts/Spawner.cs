using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Location
{
    Ceiling,
    Floor
}

public class Spawner : MonoBehaviour
{
    public bool recentlyUsed = false;
    public int Cooldown = 240;
    public int CooldownFrames;
    // Start is called before the first frame update
    void Start()
    {
        CooldownFrames = Cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (recentlyUsed)
        {
            CooldownFrames--;
            if (CooldownFrames == 0)
            {
                recentlyUsed = false;
                CooldownFrames = Cooldown;
            }
        }else if (!recentlyUsed && CooldownFrames < Cooldown)
        {
            CooldownFrames = Cooldown;
        }
    }
}
