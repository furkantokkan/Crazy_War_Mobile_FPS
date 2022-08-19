using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drop : MonoBehaviour
{
    public enum DropType
    {
        Buff,
        Weapon
    }

    [SerializeField] private string id = "";
    [SerializeField] private DropType currentDropType = DropType.Weapon;
    [SerializeField] Weapon weaponToPick;
    [SerializeField] Buff buffDropObject;
    [SerializeField] Vector3 angle;
    void Start()
    {
        if (weaponToPick != null && currentDropType == DropType.Weapon)
        {
            Instantiate(weaponToPick.GetWeaponPrefab, transform.position, transform.rotation, transform);
        }
        if (buffDropObject != null && buffDropObject.prefab != null && currentDropType == DropType.Buff)
        {
            Instantiate(buffDropObject.prefab, transform.position, transform.rotation, transform);
        }
    }
    private void Update()
    {
        transform.Rotate(angle, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && currentDropType == DropType.Weapon)
        {
            other.GetComponent<FireController>().EquipWeapon(weaponToPick);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player" && currentDropType == DropType.Buff)
        {
            StartCoroutine(buffDropObject.GetBuffEffect(other.gameObject, gameObject));
        }
    }
}
