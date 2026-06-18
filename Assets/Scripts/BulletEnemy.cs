using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    private float speed = 6f;
    private int damage = 1;

    // Update is called once per frame
    void Update()
    {
        // Moves down
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // removes if leaves the screen by the botton
        if (transform.position.y < -20f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Call TakeDamage() from PlayerHealth script
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
            Debug.Log("Player Hit!!!!");
            Destroy(gameObject);
        }
    }
}
