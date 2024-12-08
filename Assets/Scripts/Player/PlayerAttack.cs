using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Existing variables
    [SerializeField] private Transform playerTransform;
    private Camera mainCamera;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Animator playerAnimator;
    private bool isAttacking = false;
    [SerializeField] public bool hasParry = true;
    [SerializeField] public float delay = 0.4f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float parry = 3f;
    [SerializeField] private float rectangleHeight = 3f;
    [SerializeField] private int rayCount = 5;

    public Transform weaponTransform;
    private SpriteRenderer weaponSpriteRenderer;

    [SerializeField] private AudioSource weaponAudioSource;
    [SerializeField] private AudioClip normalAttackSound;
    [SerializeField] private AudioClip hitSound;


    private void OnEnable()
    {
        isAttacking = false;
    }
    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        weaponSpriteRenderer = weaponTransform.GetComponent<SpriteRenderer>();

        if (weaponAudioSource == null)
        {
            weaponAudioSource = weaponTransform.GetComponent<AudioSource>();
        }

        // Set animators to use UnscaledTime
        weaponAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void Update()
    {
        if (FindObjectOfType<PlayerHealth>().health <= 0 || FindObjectOfType<Death>().isDead)
            return;

        if (isAttacking)
            return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 direction = mousePosition - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (angle > 90 || angle < -90)
        {
            weaponTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            weaponTransform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetButtonDown("Slash"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        playerAnimator.SetTrigger("isAttacking");
        weaponAnimator.SetTrigger("Attack");
        isAttacking = true;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDirection = (mousePosition - (Vector2)playerTransform.position).normalized;

        Vector2 perpendicularDirection = Vector2.Perpendicular(attackDirection);
        float spacing = rectangleHeight / (rayCount - 1);

        float mouseDistance = Vector2.Distance(playerTransform.position, mousePosition);

        float effectiveRange = Mathf.Min(attackRange, mouseDistance);

        bool targetHit = false;

        for (int i = 0; i < rayCount; i++)
        {
            if (targetHit) break;

            Vector2 rayOrigin = (Vector2)playerTransform.position + perpendicularDirection * (i * spacing - rectangleHeight / 2);

            Debug.DrawRay(rayOrigin, attackDirection * effectiveRange, Color.red, 0.5f);

            RaycastHit2D obstacleHit = Physics2D.Raycast(rayOrigin, attackDirection, effectiveRange, obstacleLayer);
            if (obstacleHit.collider != null)
            {
                Debug.Log("Attack blocked by: " + obstacleHit.collider.name);
                break;
            }

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, attackDirection, effectiveRange, targetLayer);
            if (hit.collider != null)
            {
                targetHit = true;
                Debug.Log("Hit: " + hit.collider.name);

                weaponAudioSource.clip = hitSound;
                weaponAudioSource.Play();

                HandleHit(hit.collider);
            }
        }

        if (!targetHit)
        {
            weaponAudioSource.clip = normalAttackSound;
            weaponAudioSource.Play();
        }

        StartCoroutine(DelayAttack());
    }





    private void HandleHit(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            collider.gameObject.SetActive(false);
        }
        else if (collider.CompareTag("Boss"))
        {
            var bossHealth = collider.GetComponent<BossHealth>();
            bossHealth?.TakeDamage(3);
        }
        else if (collider.CompareTag("Boss_2"))
        {
            var bossHealth = collider.GetComponent<BossHealth>();
            bossHealth?.TakeDamage(3);
        }
        else if (collider.CompareTag("Weakspot"))
        {
            var bossHealth = collider.GetComponent<KatsuroHealth>();
            bossHealth?.TakeDamage(10);
        }
        else if (collider.CompareTag("Armor"))
        {
            var bossHealth = collider.GetComponent<Katsuro_1st_Phase>();
            bossHealth?.TakeDamage(10);
        }
        else if (collider.CompareTag("Projectile"))
        {
            if (hasParry)
            {
                StartCoroutine(DelayParry());
                var shuriken = collider.GetComponent<Shuriken>();
                shuriken?.Redirect((mainCamera.ScreenToWorldPoint(Input.mousePosition) - playerTransform.position).normalized);
                hasParry = false;
            }
            else
            {
                Debug.Log("No parry available");
            }
        }
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSecondsRealtime(delay); 
        isAttacking = false;
    }

    private IEnumerator DelayParry()
    {
        yield return new WaitForSecondsRealtime(parry); 
        hasParry = true;
    }
}
