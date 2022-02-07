using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float range;
    [SerializeField] protected int damage;

    [Header("Collider Parameters")]
    [SerializeField] protected float colliderDistance;
    [SerializeField] protected BoxCollider2D boxCollider;

    [Header("Player Parameters")]
    [SerializeField] protected LayerMask playerLayer;
    protected float cooldownTimer = Mathf.Infinity;

    // References
    protected Health playerHealth;
    protected EnemyPatrol enemyPatrol;
    protected Animator anim;

    protected string trigger = "meleeAttack";
    protected bool checkAlive = true;

    // Start is called before the first frame update
    protected void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    // Update is called once per frame
    protected void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger(trigger);
            }
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }

    protected bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, // sets to direction on outside of player
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z), // sets bounds to range
            0, Vector2.left, 0, playerLayer);

        if (checkAlive && hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            if (playerHealth.currentHealth <= 0) return false; // stops seeing if player dead
        }

        return hit.collider != null;
    }

    protected void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }
}