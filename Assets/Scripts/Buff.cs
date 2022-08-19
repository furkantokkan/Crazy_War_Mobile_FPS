using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

[CreateAssetMenu(fileName = "Weapon", menuName = "Buff/New", order = 1)]
public class Buff : ScriptableObject
{
    public enum BuffType
    {
        Health,
        Damage,
        Ammo,
        Speed,
    }

    public GameObject prefab;
    [SerializeField] private int multipiler = 0;
    [SerializeField] private bool isHaveTimeLimit;
    [SerializeField] private float timeLimit = 0;
    

    [SerializeField] private BuffType currentType;

    public IEnumerator GetBuffEffect(GameObject player, GameObject buff)
    {
       
        switch (currentType)
        {
            case BuffType.Health:
                Debug.Log("Done");
                if (isHaveTimeLimit)
                {
                    int oldHealth = player.GetComponent<Health>().GetCurrentHealth();
                    player.GetComponent<Health>().AddHealth(multipiler);
                    yield return new WaitForSeconds(timeLimit);
                    player.GetComponent<Health>().SetHealth(oldHealth);
                    
                }
                else
                {
                    player.GetComponent<Health>().AddHealth(multipiler);
                }

                break;
            case BuffType.Damage:
                if (player.GetComponent<FireController>().currentWeapon == null)
                {
                    yield break;
                }

                if (isHaveTimeLimit)
                {
                    int oldDamage = player.GetComponent<FireController>().currentWeapon.GetWeaponDamage;
                    player.GetComponent<FireController>().currentWeapon.GetWeaponDamage += multipiler;
                    yield return new WaitForSeconds(timeLimit);
                    player.GetComponent<FireController>().currentWeapon.GetWeaponDamage = oldDamage;
                }
                else
                {
                    player.GetComponent<FireController>().currentWeapon.GetWeaponDamage += multipiler;
                }

                break;
            case BuffType.Ammo:

                if (player.GetComponent<FireController>().currentWeapon == null)
                {
                    yield break;
                }
                Debug.Log("Done");

                if (isHaveTimeLimit)
                {
                    int oldAmmo = player.GetComponent<FireController>().currentWeapon.GetMaxAmmo;
                    player.GetComponent<FireController>().currentWeapon.GetMaxAmmo += multipiler;
                    yield return new WaitForSeconds(timeLimit);
                    player.GetComponent<FireController>().currentWeapon.GetMaxAmmo = oldAmmo;
                }
                else
                {
                    player.GetComponent<FireController>().currentWeapon.GetMaxAmmo += multipiler;
                }

                break;
            case BuffType.Speed:
                if (isHaveTimeLimit)
                {
                    float oldSpeed = player.GetComponent<FirstPersonController>().MoveSpeed;
                    player.GetComponent<FirstPersonController>().MoveSpeed += multipiler;
                    yield return new WaitForSeconds(timeLimit);
                    player.GetComponent<FirstPersonController>().MoveSpeed = oldSpeed;
                }
                else
                {
                    player.GetComponent<FirstPersonController>().MoveSpeed += multipiler;
                }

                break;
            default:
                Debug.Log("Case");
                break;
        }

        GameManager.onBuffTake?.Invoke(isHaveTimeLimit, timeLimit);
        Destroy(buff);
    }
}
