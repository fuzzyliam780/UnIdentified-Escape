﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool debugMode = false;

    public Animator WeaponAnimator;

    [Header("Attachment GameObjects")]
    public GameObject Body;
    public GameObject Stock;
    public GameObject Barrel;
    public GameObject BarrelAlt1;
    public GameObject BarrelExt;
    public GameObject Grip;
    public GameObject GripAlt1;
    public GameObject Magazine;
    public GameObject Sight;
    public GameObject Projectile;

    //Inspector Attachment Indexes
    private int index_barrel = 0;
    private int index_grip = 0;

    [Header("Weapon Parts")]
    public Barrels AttachedBarrel;
    public Grips AttachedGrip;

    int max_rounds_per_mag;
    int currentRoundsInMag;

    int maxAmmo;
    int currentAmmo;

    private AudioSource weaponSound;

    [Header("Fire Rate & Recoil")]
    public int fireRate = 30;
    int FirerateFrames;

    public int RecoilFrames = 10;
    public int RisingRecoilFrames;
    public int ReturningRecoilFrames;

    bool reloading = false;
    public int ReloadFrames = 30;
    int Reload_LoweringFrames;
    int Reload_RisingFrames;

    [Header("Inspect")]
    private bool inspecting = false;
    public Vector3 restingPosition = new Vector3(-0.198f, 0.38f, 0.05813932f);
    public Vector3 restingRotation = new Vector3(0f, 0f, 0f);
    public Vector3 insepctingRotation = new Vector3 (-16.937f,-43.395f,6.708f);
    public Vector3 insepctingPosition = new Vector3(-0.588f, 0.553f, 0.471f);

    Vector3 magazine_resting_pos;

    public GameObject GroundImpactEffect;
    public GameObject EnemyImpactEffect;
    public ParticleSystem MuzzleFlash;

    [Header("Weapon Stats")]
    [ShowOnly] public AmmoType AmmoType;
    [ShowOnly] public int Damage;
    [ShowOnly] public int WeaponRange;
    [ShowOnly] public int SightMagnification;
    [ShowOnly] public int MagazineCapacity;
    [ShowOnly] public float Accuracy;
    [ShowOnly] public float FireRate;
    [ShowOnly] public float RecoilReduction;
    [ShowOnly] public float MovementSpeed;
    [ShowOnly] public float ReloadSpeed;

    private float NextTimeToFire = 0;
    private float TimeToAddAmmo = 0;

    void Start()
    {
        updateAttachments();
        CalculateStats();

        //RisingRecoilFrames = RecoilFrames / 2;
        //ReturningRecoilFrames = RecoilFrames / 2;

        //Reload_LoweringFrames = ReloadFrames / 2;
        //Reload_RisingFrames = ReloadFrames / 2;

        maxAmmo = MagazineCapacity * 10;
        currentRoundsInMag = MagazineCapacity;
        currentAmmo = maxAmmo;
    }

    private void OnValidate()
    {
        updateAttachments();
        CalculateStats();
    }

    // Update is called once per frame
    void Update()
    {

        if (WeaponAnimator.GetBool("firing") && !WeaponAnimator.GetCurrentAnimatorStateInfo(0).IsName("fire"))
        {
            WeaponAnimator.SetBool("firing", false);
        }

        //Inspect
        if (Input.GetKeyDown(KeyCode.V))
        {
            Inspect();
        }

        if (Input.GetButton("Fire1") && Time.time >= NextTimeToFire)
        {
            NextTimeToFire = Time.time + 1f / FireRate;
            if (currentRoundsInMag != 0)
            {
                FireWeapon();
            }
            else
            {
                Reload();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !WeaponAnimator.GetBool("reloading"))
        {
            Reload();
        }
        else if (WeaponAnimator.GetBool("reloading") && Time.time >= TimeToAddAmmo)
        {
            ChangeMagazineAmmo(0);
            WeaponAnimator.SetBool("reloading", false);
        }
    }

    void ChangeMagazineAmmo(int delta = -1)
    {
        if (delta != 0)
        {
            currentRoundsInMag += delta;
        }
        else
        {
            //currentRoundsInMag = MagazineCapacity;
            int k = currentRoundsInMag + currentAmmo;
            if (k > MagazineCapacity)
            {
                currentRoundsInMag = MagazineCapacity;
                currentAmmo = k - MagazineCapacity;
            }
            else
            {
                currentRoundsInMag = k;
                currentAmmo = 0;
            }
        }
        UIManager.updateAmmoCounter(currentRoundsInMag, MagazineCapacity, currentAmmo);
    }

    void CalculateStats()
    {
        AmmoType = AmmoType.normal;
        Damage = 0;
        WeaponRange = 0;
        SightMagnification = 0;
        MagazineCapacity = 0;
        Accuracy = 0;
        FireRate = 0;
        ReloadSpeed = 0;
        RecoilReduction = 0;
        MovementSpeed = 0;


        //Magazine
        Damage += Magazine.GetComponent<Magazine>().Damage;
        MagazineCapacity = Magazine.GetComponent<Magazine>().MagazineCapacity;
        MovementSpeed += Magazine.GetComponent<Magazine>().MoveSpeedPenalty;
        ReloadSpeed = Magazine.GetComponent<Magazine>().ReloadSpeed;

        //Sight
        SightMagnification = Sight.GetComponent<Sight>().Magnification;
        WeaponRange += Sight.GetComponent<Sight>().WeaponRange;
        MovementSpeed += Sight.GetComponent<Sight>().MoveSpeedPenalty;

        //Body
        FireRate = Body.GetComponent<Body>().fireRate;
        Accuracy += Body.GetComponent<Body>().BaseAccuracy;
        MovementSpeed += Body.GetComponent<Body>().MoveSpeedPenalty;

        //Barrel
        switch (AttachedBarrel)
        {
            case Barrels.LongBarrel:
                Damage += Barrel.GetComponent<Barrel>().Damage;
                WeaponRange += Barrel.GetComponent<Barrel>().WeaponRange;
                Accuracy -= Barrel.GetComponent<Barrel>().AccuracyModifier;
                MovementSpeed += Barrel.GetComponent<Barrel>().MoveSpeedPenalty;
                break;
            case Barrels.ShortBarrel:
                Damage += BarrelAlt1.GetComponent<Barrel>().Damage;
                WeaponRange += BarrelAlt1.GetComponent<Barrel>().WeaponRange;
                Accuracy += BarrelAlt1.GetComponent<Barrel>().AccuracyModifier;
                MovementSpeed += BarrelAlt1.GetComponent<Barrel>().MoveSpeedPenalty;
                break;
        }

        //Grip
        switch (AttachedGrip)
        {
            case Grips.RoundGrip:
                Accuracy += Grip.GetComponent<Grip>().AccuracyModifier;
                RecoilReduction += Grip.GetComponent<Grip>().RecoilReductionModifier;
                MovementSpeed += Grip.GetComponent<Grip>().MoveSpeedPenalty;
                break;
            case Grips.StraightGrip:
                Accuracy += GripAlt1.GetComponent<Grip>().AccuracyModifier;
                RecoilReduction += GripAlt1.GetComponent<Grip>().RecoilReductionModifier;
                MovementSpeed += GripAlt1.GetComponent<Grip>().MoveSpeedPenalty;
                break;
        }

        //Stock
        RecoilReduction += Stock.GetComponent<Stock>().RecoilReductionModifier;
        MovementSpeed += Stock.GetComponent<Stock>().MoveSpeedPenalty;
    }

    void Inspect()
    {
        UIManager.toggleInspectUI();
        WeaponAnimator.SetBool("inspecting", !WeaponAnimator.GetBool("inspecting"));
        if (WeaponAnimator.GetBool("inspecting")) return;
        switch (AttachedBarrel)
        {
            case Barrels.LongBarrel:
                UIManager.updateBarrekCyclerPOS(Barrel.transform.position);
                break;
            case Barrels.ShortBarrel:
                UIManager.updateBarrekCyclerPOS(BarrelAlt1.transform.position);
                break;
        }
        switch (AttachedGrip)
        {
            case Grips.RoundGrip:
                UIManager.updateGripCyclerPOS(Grip.transform.position);
                break;
            case Grips.StraightGrip:
                UIManager.updateGripCyclerPOS(GripAlt1.transform.position);
                break;
        }
    }

    void FireWeapon()
    {
        if (WeaponAnimator.GetBool("reloading") || WeaponAnimator.GetBool("inspecting")) return;

        MuzzleFlash.Play(); //Play muzzle flash
        weaponSound.Play(); //Play weapon sound
        ChangeMagazineAmmo();
        WeaponAnimator.SetBool("firing", true);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, WeaponRange)) //Fire raycast from current weapon position forward until it hits something or reaches the maximum range
        {//if something is hit
            if (debugMode)
            {
                Debug.Log(hit.transform.name);//Log the name of the hit object
                Debug.DrawLine(transform.position, hit.point, Color.black,2); //Draw a black line in the SCENE view between the current weapon position and the position of the point the raycast hit that will last for 2 seconds
            }

            Enemy tempEnemy = hit.transform.GetComponent<Enemy>();
            GameObject ImpactGO;

            if (tempEnemy != null)// If an enemy is hit
            {
                ImpactGO = Instantiate(EnemyImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                tempEnemy.takeDamage(Damage + SkillManager.damageBoost);
                UIManager.updateScore(1);
            }
            else // If something other than an enemy if hit
            {
                ImpactGO = Instantiate(GroundImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            Destroy(ImpactGO, 2); //Destroys impact effect after 2 seconds
        }
        else if (debugMode) //if nothing is hit
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * WeaponRange, Color.red, 2);
        }
        
    }

    void Reload()
    {
        if (WeaponAnimator.GetBool("inspecting")) return;

        if (currentRoundsInMag < MagazineCapacity)
        {
            TimeToAddAmmo = Time.time + ReloadSpeed;
            WeaponAnimator.SetBool("reloading", true);
        }
    }

    public void updateAttachments()
    {
        Vector3 barrel_ext_position = Vector3.zero;
        if (AttachedBarrel == Barrels.LongBarrel)
        {
            barrel_ext_position = Barrel.GetComponent<Barrel>().Barrel_Ext_Pos;
            weaponSound = Barrel.GetComponent<Barrel>().weaponSound;
            Barrel.SetActive(true);
            BarrelAlt1.SetActive(false);
            if (UIManager.BarrelCycler != null)
            {
                if (UIManager.BarrelCycler.activeInHierarchy)
                {
                    UIManager.updateBarrekCyclerPOS(Barrel.transform.position);
                }
            }
        }
        else if (AttachedBarrel == Barrels.ShortBarrel)
        {
            barrel_ext_position = BarrelAlt1.GetComponent<Barrel>().Barrel_Ext_Pos;
            weaponSound = BarrelAlt1.GetComponent<Barrel>().weaponSound;
            BarrelAlt1.SetActive(true);
            Barrel.SetActive(false);
            if (UIManager.BarrelCycler != null)
            {
                if (UIManager.BarrelCycler.activeInHierarchy)
                {
                    UIManager.updateBarrekCyclerPOS(BarrelAlt1.transform.position);
                }
            }
        }

        if (BarrelExt.activeInHierarchy)
        {
            BarrelExt.transform.localPosition = barrel_ext_position;
        }

        if (AttachedGrip == Grips.RoundGrip)
        {
            Grip.SetActive(true);
            GripAlt1.SetActive(false);
            if (UIManager.BarrelCycler != null)
            {
                if (UIManager.GripCycler.activeInHierarchy)
                {
                    UIManager.updateGripCyclerPOS(Grip.transform.position);
                }
            }
        }
        else if (AttachedGrip == Grips.StraightGrip)
        {
            GripAlt1.SetActive(true);
            Grip.SetActive(false);
            if (UIManager.BarrelCycler != null)
            {
                if (UIManager.GripCycler.activeInHierarchy)
                {
                    UIManager.updateGripCyclerPOS(GripAlt1.transform.position);
                }
            }
        }
    }
    public void CycleBarrelsForward()
    {
        if (index_barrel == 1)//If Max, cycle to beginning
        {
            index_barrel = 0;
        }
        else
        {
            index_barrel++;
        }

        switch (index_barrel)
        {
            case 0:
                AttachedBarrel = Barrels.LongBarrel;
                updateAttachments();
                break;
            case 1:
                AttachedBarrel = Barrels.ShortBarrel;
                updateAttachments();
                break;
        }
    }
    public void CycleGripsForward()
    {
        if (index_grip == 1)//If Max, cycle to beginning
        {
            index_grip = 0;
        }
        else
        {
            index_grip++;
        }

        switch (index_grip)
        {
            case 0:
                AttachedGrip = Grips.RoundGrip;
                updateAttachments();
                break;
            case 1:
                AttachedGrip = Grips.StraightGrip;
                updateAttachments();
                break;
        }
    }
}


public enum Barrels
{
    LongBarrel,
    ShortBarrel
}

public enum Grips
{
    RoundGrip,
    StraightGrip
}
