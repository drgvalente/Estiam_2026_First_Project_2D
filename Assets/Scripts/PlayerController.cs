using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;

    [Header("Shoot")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.3f; // seconds betwen shots

    private float _nextFireTime;
    private Camera _cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsCursor();
        AutoFire();
    }

    void MoveTowardsCursor()
    {
        // Works with mouse OR touch
        Vector3 inputPos;

        if (Input.touchCount > 0) // if player is touching the screen
            inputPos = Input.GetTouch(0).position; // get the first touch point
        else
            inputPos = Input.mousePosition; // get the mouse position

        // Converts screen coordinates to 2D world coordinates
        Vector3 targetWorld = _cam.ScreenToWorldPoint(inputPos);
        targetWorld.z = 0f;

        // Moves the ship towards the cursor with limited speed
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorld,
            speed * Time.deltaTime
        );

        // Keeps the ship inside the game screen
        ClampToScreen();
    }

    void ClampToScreen()
    {
        // Calculates the limits of the camera in world coords
        Vector3 min = _cam.ScreenToWorldPoint(Vector3.zero);
        Vector3 max = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, min.x + 0.5f, max.x - 0.5f);
        pos.y = Mathf.Clamp(pos.y, min.y + 0.5f, max.y - 0.6f); // limit from the center of screen
        transform.position = pos;
    }


    void AutoFire()
    {
        // Shoots aultomatically every 'fireRate' seconds
        if (Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.Log("BulletPrefab or FirePoint not Setup!!");
            return;
        }
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}
