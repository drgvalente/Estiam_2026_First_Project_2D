using UnityEngine;

public class BulletPlayer : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    public int damage = 1;

    // Update is called once per frame
    void Update()
    {
        // Moves up every frame
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // Removes if leaves the screen thru the top
        if (transform.position.y > 10f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collide with an Enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
