using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool debugMode = false;

    public GameObject Body;
    public GameObject Stock;
    public GameObject Barrel;
    public GameObject BarrelExt;
    public GameObject Grip;
    public GameObject Magazine;
    public GameObject Sight;
    public GameObject Projectile;

    int max_rounds_per_mag;
    int currentRoundsInMag;

    int maxAmmo;
    int currentAmmo;

    public AudioSource weaponSound;

    //in frames for now
    public int fireRate = 30;
    int FirerateFrames;

    public int RecoilFrames = 10;
    int RisingRecoilFrames;
    int ReturningRecoilFrames;

    bool reloading = false;
    public int ReloadFrames = 30;
    int Reload_LoweringFrames;
    int Reload_RisingFrames;

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

        if (Input.GetKeyDown(KeyCode.R))
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

    bool FireWeapon()
    {
        if (currentRoundsInMag != 0f)
        {
            weaponSound.Play();
            currentRoundsInMag--;

            //GameObject projectile = Instantiate<GameObject>(Projectile);
            //projectile.transform.position = Barrel.transform.position;
            //projectile.transform.eulerAngles = transform.eulerAngles;
            //Destroy(projectile, 5);

            RaycastHit hit;
            if (Physics.Raycast(Barrel.transform.position, transform.forward,out hit, Mathf.Infinity, 1 << 9))
            {
                if (debugMode)
                {
                    Debug.DrawRay(Barrel.transform.position, hit.point,Color.red);
                }

                hit.collider.gameObject.GetComponent<Enemy>().takeDamage();
                UIManager.updateScore(1);
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
}
