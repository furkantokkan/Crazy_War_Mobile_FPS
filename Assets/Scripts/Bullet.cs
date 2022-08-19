using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject impact;
    [SerializeField] private int damage;
    private Rigidbody rb;
    private float moveSpeed = 5f;
    private float lifeTime = 10f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void SetBulletSpeed(float amount)
    {
        moveSpeed = amount;
    }
    public void SetBulletDamage(int amount)
    {
        damage = amount;
    }
    void Update()
    {
        rb.velocity = transform.forward * moveSpeed;


        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0f)
        {
            DestroyTheObject(null);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        DestroyTheObject(other);
    }

    private void DestroyTheObject(Collider collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.BroadcastMessage("AddDamage", damage);
            }
        }
        Destroy(gameObject);
        if (impact != null)
        {
            Instantiate(impact, transform.position, transform.rotation);
        }
    }
}
