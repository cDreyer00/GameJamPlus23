using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float lifeTime;

    void Update()
    {
        Move();

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);
    }

    public void Move()
    {
        transform.position += moveSpeed * Time.deltaTime * transform.forward;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<PlayerController>() != null) return;
        Destroy(gameObject);
        if (!col.TryGetComponent<MeleeEnemy>(out var enemy)) return;
        enemy.Events.onTakeDamage?.Invoke(damage);
    }
}