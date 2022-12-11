using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Properties")]
    public Vector2 direction;
    public Rigidbody2D rigidbody2D;
    [Range(1.0f, 100.0f)] 
    public float force;

    public Vector3 offset;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Activate()
    {
        Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position + offset;
        direction = (playerPosition - transform.position).normalized;
        Rotate();
        Move();
        Invoke("DestroyYourself", 2.0f);
    }

    private void Rotate()
    {
        rigidbody2D.AddTorque(Random.Range(5.0f, 15.0f) * direction.x * -1.0f, ForceMode2D.Impulse);
    }

    private void Move()
    {
        rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void DestroyYourself()
    {
        if (gameObject.activeInHierarchy)
        {
            BulletManager.Instance().ReturnBullet(this.gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            DestroyYourself();
        }
    }
}
