using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float lifeTime = 10f;
    private Vector2 direction;
    private float speed = 1f;
    private bool isRedirected = false;
    private EnemyShooter enemyShooter;
    private Rigidbody2D rb;

    void Start()
    {
        enemyShooter = FindObjectOfType<EnemyShooter>();
        rb = GetComponent<Rigidbody2D>(); 
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (isRedirected)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OneWayPlatform"))
        {
            return;
        }
        if (collision.CompareTag("building"))
        {
            return;
        }

        if (isRedirected && collision.CompareTag("Enemy"))
        {
            Debug.Log("Projétil redirecionado colidiu com o inimigo: " + collision.gameObject.name);
            collision.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Target"))
        {
            if (isRedirected)
            {
                Target target = collision.gameObject.GetComponent<Target>();
                if (target != null)
                {
                    target.OnHit();  // Ensure target's OnHit method is called.
                }
                Destroy(gameObject);  // Destroy shuriken on hit.
            }
            return;
        }

        else
        {
            Debug.Log("Projétil colidiu com: " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }

    public void Redirect(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        speed = enemyShooter.projectileSpeed;

        isRedirected = true;

        gameObject.layer = LayerMask.NameToLayer("Projectile");

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        Debug.Log("Direção após parry: " + direction);
        Debug.Log("Velocidade após parry: " + speed);
    }
}
