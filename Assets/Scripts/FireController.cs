using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    public Weapon currentWeapon = null;
    private Transform mainCamera;
    private Animator anim;
    private float lastFire;
    private bool isReloading = false;
    private bool zoomed = false;

    private Transform handTransform;
    private Transform gunHolder;
    private void Awake()
    {
        mainCamera = GameObject.FindWithTag("CinemachineTarget").transform;
        anim = mainCamera.transform.GetChild(0).GetComponent<Animator>();
        gunHolder = mainCamera.transform.GetChild(0);
        handTransform = mainCamera.transform.GetChild(0).GetChild(0).transform;
    }
    void Start()
    {
        SpawnWeapon();
        InputManager.Instance.onTouchStart += FireInput;
    }
  
    private void OnDisable()
    {
        InputManager.Instance.onTouchStart -= FireInput;
    }
    void Update()
    {

        if (currentWeapon != null)
        {
            ReloadInput();
            //AimInput();
        }
    }

    //private void AimInput()
    //{
    //    if (Mouse.current.rightButton.wasPressedThisFrame)
    //    {
    //        if (!zoomed)
    //        {
    //            Camera.main.fieldOfView = currentWeapon.GetZoomedInFOV;
    //            zoomed = true;
    //        }
    //        else
    //        {
    //            Camera.main.fieldOfView = currentWeapon.GetZoomedOutFOV;
    //            zoomed = false;
    //        }
    //    }
    //}

    public void FireInput()
    {
        if (Time.timeSinceLevelLoad - lastFire > currentWeapon.GetAttackRate &&
            currentWeapon.GetCurrentAmmo > 0 && !isReloading)
        {
            lastFire = Time.timeSinceLevelLoad;

            RaycastHit hit;
            bool raycast = Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 50f);
            if (raycast)
            {
                bool lookTheForward = true;

                if (Vector3.Distance(mainCamera.position, hit.point) < 3f)
                {
                    lookTheForward = false;
                }

                currentWeapon.FireTheBullet(hit.point, lookTheForward);
            }
            else
            {

                currentWeapon.FireTheBullet(mainCamera.position + (mainCamera.forward * 30f), true);

            }
            if (anim != null && !anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                anim.SetTrigger("Fire");
            }

            StartCoroutine(currentWeapon.MuzzleFlash());
        }

    }

    private void ReloadInput()
    {
        if (currentWeapon.GetCurrentAmmo <= 0 && currentWeapon.GetMaxAmmo > 0 && !isReloading)
        {
            StartCoroutine(currentWeapon.Reload());
            isReloading = true;
            anim.SetBool("Reload", true);
        }
        if (currentWeapon.GetCurrentAmmo > 0)
        {
            anim.SetBool("Reload", false);
            if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                isReloading = false;
            }
        }
    }

    public void EquipWeapon(Weapon weaponType)
    {
        currentWeapon.Drop();
        currentWeapon = weaponType;
        SpawnWeapon();
    }
    private void SpawnWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }
        currentWeapon.SpawnNewWeapon(handTransform, anim, transform);
    }

    private bool EnemyInRange()
    {
        bool hit = Physics.Raycast(handTransform.position, handTransform.forward, currentWeapon.GetWeaponRange, 1 << LayerMask.NameToLayer("Enemy"));
        Debug.DrawRay(handTransform.position, handTransform.forward * currentWeapon.GetWeaponRange, Color.red);
        return hit;
    }
}
