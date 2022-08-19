using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
public class Weapon : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] bool canAim;
    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] Vector3 weaponOffset = Vector3.zero;
    [Header("Weapon Attributes")]
    [SerializeField] Bullet bullet = null;
    [SerializeField] float zoomedInFOV = 35f;
    [SerializeField] float zoomedOutFOV = 60f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float bulletSpeed = 8f;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float weaponRange = 5f;
    [SerializeField] int weaponDamage = 10;
    [SerializeField] int magAmount = 5;
    [SerializeField] int maxAmmo = 30;
    [Header("Muzzleflash Settings")]
    [SerializeField] private GameObject muzzlePrefab;
    [SerializeField] private float lightDuration = 0.02f;

    private GameObject weaponClone;
    private Transform firePoint;
    private GameObject muzzleClone;

    private int currentAmmo;
    private int currentMaxAmmo;

    public float GetWeaponRange { get { return weaponRange; } }
    public float GetZoomedInFOV { get { return zoomedInFOV; } }
    public float GetZoomedOutFOV { get { return zoomedOutFOV; } }
    public GameObject GetWeaponPrefab { get { return weaponPrefab; } }

    public Transform GetFirePoint { get { return firePoint; } }
    public int GetCurrentAmmo { get { return currentAmmo; } set { currentAmmo = value; } }
    public int GetMaxAmmo { get { return currentMaxAmmo; } set { currentMaxAmmo = value; } }
    public int GetWeaponDamage { get { return weaponDamage; } set { weaponDamage = value; } }
    public float GetAttackRate { get { return fireRate; } }
    public float GetProjectileSpeed { get { return bulletSpeed; } }

    public void SpawnNewWeapon(Transform parrent, Animator anim, Transform player)
    {
        if (weaponPrefab != null)
        {
            weaponClone = Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity, parrent);
            firePoint = weaponClone.transform.GetChild(0);
            weaponClone.transform.position = parrent.position;
            weaponClone.transform.rotation = weaponPrefab.transform.rotation;
            weaponClone.transform.localRotation = weaponPrefab.transform.rotation;
            weaponClone.transform.localPosition = (Vector3.zero + weaponOffset);
            weaponClone.transform.localScale = weaponPrefab.transform.localScale;
            currentMaxAmmo = maxAmmo;
            currentAmmo = magAmount;
        }
        if (animatorOverride != null)
        {
            anim.runtimeAnimatorController = animatorOverride;
        }
    }

    public void FireTheBullet(Vector3 lookPoint, bool lookTheForward)
    {
        if (firePoint != null)
        {
            if (lookTheForward)
            {
                firePoint.transform.LookAt(lookPoint);
            }
            Bullet bulletClone = Instantiate(bullet, firePoint.position, firePoint.rotation);
            bulletClone.SetBulletSpeed(bulletSpeed);
            bulletClone.SetBulletDamage(weaponDamage);
            currentAmmo--;
        }
    }
    public void Drop()
    {
        Destroy(weaponClone);
    }
    public IEnumerator Reload()
    {
        if (GameManager.unlimitedAmmo)
        {
            currentAmmo = magAmount;
            yield break;
        }
        yield return new WaitForSeconds(reloadTime);
        if (currentMaxAmmo >= magAmount)
        {
            currentAmmo = magAmount;
            currentMaxAmmo -= magAmount;
        }
        else
        {
            currentAmmo = currentMaxAmmo;
            currentMaxAmmo = 0;
        }
    }
    public IEnumerator MuzzleFlash()
    {
        if (muzzlePrefab == null)
        {
            yield break;
        }
        if (muzzleClone == null)
        {
            muzzleClone = Instantiate(muzzlePrefab, firePoint.transform.position, weaponClone.transform.rotation,
                firePoint.transform);
            muzzleClone.transform.localPosition = Vector3.zero;
            muzzleClone.transform.localRotation = Quaternion.Euler(0f,-90f,0f);
        }
        else
        {
            muzzleClone.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(lightDuration);
        muzzleClone.gameObject.SetActive(false);
    }
}
