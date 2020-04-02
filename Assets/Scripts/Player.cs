using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int MaxHealth = 30;
    public int Health;
    public float HealthRegenCooldown = 3; //Time before Regen Starts
    public float HealthRegenInterval = 0.75f;
    public float DamageInterval = 0.5f; //Time Between each point of danage
    public float TimeToStartHealing;
    public float TimeToNextHeal;
    public float TimeToNextDamage;
    public bool isHealing = false;
    public GameObject[] possibleWeapons;
    int activeSlot;
    public GameObject slot1;
    int slot1Index;
    public GameObject slot2;
    int slot2Index;
    public float SwitchInterval = 0.5f;
    public float TimetoSwitch;
    bool changingWeapons = false;

    void Start()
    {
        Health = MaxHealth;
        UIManager.setMaxHealth(MaxHealth);
        TimeToStartHealing = 0;

        activeSlot = 1;
        //slot1.SetActive(false);
        switch (slot1.name)
        {
            case "FireSleet_Modular":
                slot1Index = 1;
                break;
            case "Mauler_Modular":
                slot1Index = 2;
                break;
            case "Grimbrand_Modular":
                slot1Index = 3;
                break;
            case "ScatterBurst_Modular":
                slot1Index = 4;
                break;
            case "Archtronic_Modular":
                slot1Index = 5;
                break;
            default:
                slot1Index = 0;
                break;
        }

        slot1.GetComponent<Weapon>().WeaponAnimator.SetInteger("Weapon", slot1Index);

        switch (slot2.name)
        {
            case "FireSleet_Modular":
                slot2Index = 1;
                break;
            case "Mauler_Modular":
                slot2Index = 2;
                break;
            case "Grimbrand_Modular":
                slot2Index = 3;
                break;
            case "ScatterBurst_Modular":
                slot2Index = 4;
                break;
            case "Archtronic_Modular":
                slot2Index = 5;
                break;
            default:
                slot2Index = 0;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!slot1.activeInHierarchy && activeSlot == 1)
        {
            slot1.SetActive(true);
        }

        if (changingWeapons)
        {
            if (Time.time >= TimetoSwitch)
            {
                switch (activeSlot)
                {
                    case 1:
                        slot1.SetActive(true);
                        slot2.SetActive(false);
                        break;
                    case 2:
                        slot1.SetActive(false);
                        slot2.SetActive(true);
                        break;
                }
                changingWeapons = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && activeSlot != 1)
        {
            slot1.GetComponent<Weapon>().WeaponAnimator.SetInteger("Weapon", slot1Index);
            TimetoSwitch = Time.time + SwitchInterval;
            activeSlot = 1;
            changingWeapons = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && activeSlot != 2)
        {
            slot2.GetComponent<Weapon>().WeaponAnimator.SetInteger("Weapon", slot2Index);
            TimetoSwitch = Time.time + SwitchInterval;
            activeSlot = 2;
            changingWeapons = true;
        }

        if (Health == MaxHealth)
        {
            isHealing = false;
        }
        else
        {
            if(Time.time >= TimeToNextHeal)
            {
                Health++;
                UIManager.updateHealth(1);
                TimeToNextHeal = Time.time + TimeToNextHeal;
            }
        }
            
        //if (Health < MaxHealth && HealthRegenFrames > 0)
        //{
        //    HealthRegenFrames--;
        //}
        //else if (HealthRegenFrames == 0 && RegenFrames > 0)  
        //{
        //    RegenFrames--;
        //    if (RegenFrames == 0)
        //    {
        //        if (Health == MaxHealth) HealthRegenFrames = HealthRegenCooldown;
        //        RegenFrames = CooldownBetweenRegen;
        //    }
        //}
        //if (DamageFrames > 0)
        //{
        //    DamageFrames--;
        //}
    }

    public void takeDamage()
    {
        if (Time.time >= TimeToNextDamage)
        {
            Health--;
            if (Health == 0)
            {
                UIManager.updateRoundResult("You Died!");
                return;
            }
            UIManager.updateHealth(-1);
            TimeToNextDamage = Time.time + DamageInterval;
            if (Health == MaxHealth - 1)
            {
                TimeToStartHealing = Time.time + TimeToStartHealing;
                isHealing = true;
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        GameObject go = collision.gameObject;

        if (go.layer == 9)
        {
            go.GetComponent<TestEnemy>().Attacking();
            takeDamage();
        }
        
        
    }
}
