using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static int XP = 0;
    public static int SkillPoints = 0;
    public static int XPToLevelUP = 5;

    public static float fireRateBoost = 0f;
    public static int MoveSpeedBoost = 0;
    public static int damageBoost = 0;
    public static int HealthBoost = 0;


    public static void grantXP(int xpToGrant)
    {
        XP += xpToGrant;
        if (XP == XPToLevelUP)
        {
            XP -= 4;
            SkillPoints++;
            UIManager.updateSkillPointIndicator(SkillPoints);
        }
    }

    public static void grantFireRateBoost()
    {
        if (fireRateBoost == 0f && SkillPoints >= 1)
        {
            fireRateBoost = 0.5f;
            SkillPoints--;
        }
        else
        {
            fireRateBoost = 0f;
        }
    }
    public static void grantMoveSpeedBoost()
    {
        if (MoveSpeedBoost == 0 && SkillPoints >= 1)
        {
            MoveSpeedBoost = 2;
            SkillPoints--;
        }
        else
        {
            MoveSpeedBoost = 0;
        }
    }
    public static void grantDamageBoost()
    {
        if (damageBoost == 0 && SkillPoints >= 1)
        {
            damageBoost = 2;
            SkillPoints--;
        }
        else
        {
            damageBoost = 0;
        }
    }
    public static void grantHealthBoost()
    {
        if (HealthBoost == 0f && SkillPoints >= 1)
        {
            HealthBoost = 5;
            SkillPoints--;
        }
        else
        {
            HealthBoost = 0;
        }
    }
}
