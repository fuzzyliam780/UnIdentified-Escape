using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentWeapon
{
    Grimbrand,
    Mauler,
    Firesleet,
    Archtronic,
    Hellwailer,
    Scatterburst
}

public class WeaponStation : MonoBehaviour
{
    public WeaponInfo[] possibleWeapons;
    public GameObject currentWeapon;
    public GameObject container;
    public CurrentWeapon Weapon;
    public Vector3 WeaponRotation;

    // Start is called before the first frame update
    void Start()
    {
        setNewWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentWeapon.activeInHierarchy) currentWeapon.SetActive(true);
    }

    private void OnValidate()
    {
        setNewWeapon();
    }

    void setNewWeapon()
    {
        switch (Weapon)
        {
            case CurrentWeapon.Archtronic:
                setupWeapon(4);
                break;
            case CurrentWeapon.Firesleet:
                setupWeapon(0);
                break;
            case CurrentWeapon.Grimbrand:
                setupWeapon(1);
                break;
            case CurrentWeapon.Hellwailer:
                setupWeapon(5);
                break;
            case CurrentWeapon.Mauler:
                setupWeapon(2);
                break;
            case CurrentWeapon.Scatterburst:
                setupWeapon(3);
                break;
        }
    }

    void setupWeapon(int weaponID)
    {
        Destroy(currentWeapon);
        currentWeapon = Instantiate<GameObject>(possibleWeapons[weaponID].WeaponGameObject);
        currentWeapon.transform.SetParent(this.transform);
        currentWeapon.transform.localPosition = possibleWeapons[weaponID].WeaponPostion;
        currentWeapon.transform.eulerAngles = WeaponRotation;

        container.transform.localScale = possibleWeapons[weaponID].ContainerScale;
    }
}

[System.Serializable]
public struct WeaponInfo
{
    public string Name;
    public GameObject WeaponGameObject;
    public float Cost;
    public Vector3 WeaponPostion;
    public Vector3 ContainerScale;
}
