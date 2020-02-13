using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool debugMode = false;

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
    int RisingRecoilFrames;
    int ReturningRecoilFrames;

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

    void Start()
    {
        max_rounds_per_mag = Magazine.GetComponent<Magazine>().MaximumRounds;
        maxAmmo = max_rounds_per_mag * 10;

        RisingRecoilFrames = RecoilFrames / 2;
        ReturningRecoilFrames = RecoilFrames / 2;

        Reload_LoweringFrames = ReloadFrames / 2;
        Reload_RisingFrames = ReloadFrames / 2;

        currentRoundsInMag = max_rounds_per_mag;
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (FirerateFrames != 0) FirerateFrames--;
        if (RisingRecoilFrames != 0)
        {
            RisingRecoilFrames--;
            transform.Rotate(new Vector3(-0.5f, 0, 0));
        }
        else if (ReturningRecoilFrames != 0)
        {
            ReturningRecoilFrames--;
            transform.Rotate(new Vector3(0.5f, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            inspecting = !inspecting;
            Inspect();
            UIManager.toggleInspectUI();
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

        if (Input.GetKeyDown(KeyCode.R) && !inspecting)
        {
            Reload();
            reloading = true;
        }

        if (reloading)
        {
            Reload();
            
        }
        else if (Input.GetMouseButton(0) && FirerateFrames == 0)
        {
            if (RisingRecoilFrames == 0 && ReturningRecoilFrames == 0)
            {
                if (FireWeapon())
                {
                    FirerateFrames = fireRate;
                    RisingRecoilFrames = RecoilFrames / 2;
                    ReturningRecoilFrames = RecoilFrames / 2;
                }
            }
        }
    }

    void Inspect()
    {
        UIManager.toggleGameplayUI();

        if (inspecting)
        {
            transform.localPosition = insepctingPosition;
            transform.localEulerAngles = insepctingRotation;
        }
        else
        {
            transform.localPosition = restingPosition;
            transform.localEulerAngles = restingRotation;
        }
    }

    bool FireWeapon()
    {
        if (inspecting) return false;

        if (currentRoundsInMag != 0f)
        {
            weaponSound.Play();
            currentRoundsInMag--;

            RaycastHit hit;

            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            if (Physics.Raycast(Barrel.transform.position, transform.forward,out hit, 1000f, layerMask))
            {
                //GameObject projectile = Instantiate<GameObject>(Projectile);
                //projectile.transform.position = Barrel.transform.position;
                //projectile.transform.eulerAngles = transform.eulerAngles;
                //projectile.GetComponent<Projectile>().setTargetPosition(hit.point);
                //Destroy(projectile, 5);

                if (GameManager.DebugMode)
                {
                    Debug.DrawRay(Barrel.transform.position, hit.point,Color.black,5f);
                }

                if (hit.collider.gameObject.layer == 9)
                {
                    hit.collider.gameObject.GetComponent<Enemy>().takeDamage(1 + SkillManager.damageBoost);
                    UIManager.updateScore(1);
                }
            }

                UIManager.updateAmmoCounter(currentRoundsInMag, max_rounds_per_mag, currentAmmo);
            return true;
        }
        else
        {
            Reload();
            reloading = true;
            return false;
        }
        
    }

    void Reload()
    {
        if (reloading)
        {
            if (Reload_LoweringFrames != 0)
            {
                Reload_LoweringFrames--;
                Magazine.transform.position = new Vector3(Magazine.transform.position.x, Magazine.transform.position.y - 0.03f, Magazine.transform.position.z);
            }
            else if (Reload_RisingFrames != 0)
            {
                Reload_RisingFrames--;
                Magazine.transform.position =  new Vector3(Magazine.transform.position.x, Magazine.transform.position.y + 0.03f, Magazine.transform.position.z);
                if(Reload_RisingFrames == 0)
                {
                    if (currentAmmo >= max_rounds_per_mag)
                    {
                        reloading = false;
                        currentAmmo -= max_rounds_per_mag - currentRoundsInMag;
                        currentRoundsInMag = max_rounds_per_mag;
                        UIManager.updateAmmoCounter(currentRoundsInMag, max_rounds_per_mag, currentAmmo);
                    }
                    else if (currentAmmo != 0)
                    {
                        reloading = false;
                        int k = currentRoundsInMag + currentAmmo;
                        if (k > max_rounds_per_mag)
                        {
                            currentRoundsInMag = max_rounds_per_mag;
                            currentAmmo = k - max_rounds_per_mag;
                        }
                        else
                        {
                            currentRoundsInMag = k;
                            currentAmmo = 0;
                        }
                        UIManager.updateAmmoCounter(currentRoundsInMag, max_rounds_per_mag, currentAmmo);
                    }
                    else
                    {
                        reloading = false;
                    }
                }
            }
        }
        else
        {
            Reload_LoweringFrames = ReloadFrames / 2;
            Reload_RisingFrames = ReloadFrames / 2;
        }
        
    }

    private void OnValidate()
    {
        updateAttachments();
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
            if (UIManager.BarrelCycler.activeInHierarchy)
            {
                UIManager.updateBarrekCyclerPOS(Barrel.transform.position);
            }
        }
        else if (AttachedBarrel == Barrels.ShortBarrel)
        {
            barrel_ext_position = BarrelAlt1.GetComponent<Barrel>().Barrel_Ext_Pos;
            weaponSound = BarrelAlt1.GetComponent<Barrel>().weaponSound;
            BarrelAlt1.SetActive(true);
            Barrel.SetActive(false);
            if (UIManager.BarrelCycler.activeInHierarchy)
            {
                UIManager.updateBarrekCyclerPOS(BarrelAlt1.transform.position);
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
            if (UIManager.GripCycler.activeInHierarchy)
            {
                UIManager.updateGripCyclerPOS(Grip.transform.position);
            }
        }
        else if (AttachedGrip == Grips.StraightGrip)
        {
            GripAlt1.SetActive(true);
            Grip.SetActive(false);
            if (UIManager.GripCycler.activeInHierarchy)
            {
                UIManager.updateGripCyclerPOS(GripAlt1.transform.position);
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

