using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int MaxHealth = 30;
    public int Health;
    public int HealthRegenCooldown = 900; //Frames before Regen Starts
    public int HealthRegenFrames;
    public int CooldownBetweenRegen = 270; //Frames Between each point of regen
    public int RegenFrames;
    public int DamageStutter = 60; //Frames Between each point of danage
    public int DamageFrames = 0;

    void Start()
    {
        Health = MaxHealth;
        UIManager.setMaxHealth(MaxHealth);
        HealthRegenFrames = HealthRegenCooldown;
        RegenFrames = CooldownBetweenRegen;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health < MaxHealth && HealthRegenFrames > 0)
        {
            HealthRegenFrames--;
        }
        else if (HealthRegenFrames == 0 && RegenFrames > 0)  
        {
            RegenFrames--;
            if (RegenFrames == 0)
            {
                Health++;
                UIManager.updateHealth(1);
                if (Health == MaxHealth) HealthRegenFrames = HealthRegenCooldown;
                RegenFrames = CooldownBetweenRegen;
            }
        }
        if (DamageFrames > 0)
        {
            DamageFrames--;
        }
    }

    public void takeDamage()
    {
        if (DamageFrames == 0)
        {
            Health--;
            if (Health == 0)
            {
                UIManager.updateRoundResult("You Died!");
                return;
            }
            UIManager.updateHealth(-1);
            DamageFrames = DamageStutter;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        GameObject go = collision.gameObject;

        if (go.layer == 9)
        {
            go.GetComponent<Enemy>().Attacking();
            takeDamage();
        }
        
        
    }
}
