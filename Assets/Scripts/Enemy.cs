using UnityEngine;

public enum EnemyType { Descender, SideToSide, Stationary}

public class Enemy : MonoBehaviour
{
    [Header("Behavior Type")]
    public EnemyType type = EnemyType.Descender;

    [Header("Health")]
    public int health = 3;
    public int scoreValue = 100;

    [Header("Movement")]
    public float descendSpeed = 1.5f; // down speed
    public float lateralSpeed = 2.5f; // side to side speed
    public float lateralLimit = 4.5f; // how far it goes to sides

    [Header("Shots (Side to Side and Stationary)")]
    public GameObject bulletPrefab;
    public float minFireInterval = 3f;
    public float maxFireInterval = 8f;

    private int _lateralDir = 1; // 1 = right, -1 = left
    private float _nextFireTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Each enemy starts shooting after diferent random times
        _nextFireTime = Time.time + Random.Range(minFireInterval, maxFireInterval);

        // SideToSide: Lets orient them depending the side they are entering from
        if (type == EnemyType.SideToSide)
            _lateralDir = transform.position.x < 0 ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case EnemyType.Descender:
                Descend();
                break;

            case EnemyType.SideToSide:
                MoveLaterally();
                TryShoot();
                break;

            case EnemyType.Stationary:
                TryShoot();
                break;
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0) Die();
    }

    void Die()
    {
        // TO DO: spawn an explosion effect here later (maybe with particles)
        Destroy(gameObject);
    }

    // BEHAVIORS and SHOOTS

    void Descend()
    {
        transform.Translate(Vector2.down * descendSpeed * Time.deltaTime);
        // removes if leaves the screen by the botton
        if (transform.position.y < -20f)
            Destroy(gameObject);
    }

    void MoveLaterally()
    {
        transform.Translate(Vector2.right * _lateralDir * lateralSpeed * Time.deltaTime);

        // Invert the direction when reaches the limit
        if (transform.position.x > lateralLimit) _lateralDir = -1;
        if (transform.position.x < -lateralLimit) _lateralDir = 1;
    }

    void TryShoot()
    {
        if (bulletPrefab == null) return;

        if(Time.time >= _nextFireTime)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            _nextFireTime = Time.time + Random.Range(minFireInterval, maxFireInterval);
        }
    }
}
