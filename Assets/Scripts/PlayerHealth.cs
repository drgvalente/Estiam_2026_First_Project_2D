using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxLives = 3;

    [Header("Invincibility after take damage")]
    public float invincibleDuration = 2f; // seconds to blink

    // Event: warns the HUD when HP changes
    public UnityEvent<int> onLivesChanged = new UnityEvent<int>();

    private int _currentLives;
    private bool _isInvincible = false;
    private SpriteRenderer _sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentLives = maxLives;
        _sprite = GetComponentInChildren<SpriteRenderer>();

        // Tells the HUD the start value
        onLivesChanged.Invoke(_currentLives);
    }

    // Called by BulletEnemy when it hits the Player
    public void TakeDamage(int dmg)
    {
        // Ignore damage while invincible
        if (_isInvincible) return;

        _currentLives -= dmg;
        onLivesChanged.Invoke(_currentLives);

        if (_currentLives <= 0)
            Die();
        else
            StartCoroutine(InvincibilityFrames());
    }

    void Die()
    {
        Debug.Log("Player Died");
        // TO DO: Show the Game Over Screen
        // For now, restarts the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator InvincibilityFrames()
    {
        _isInvincible = true;

        float elapsed = 0f;

        while (elapsed < invincibleDuration)
        {
            // alternates visibility every 0.1s to create a blink effect
            _sprite.enabled = !_sprite.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // Ensure that the sprite is visible at the end
        _sprite.enabled = true;
        _isInvincible = false;
    }
}
