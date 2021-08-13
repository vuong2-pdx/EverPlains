using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlimeController : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public PolygonCollider2D pc;
    public Animator animator;
    public Transform target;
    public float range = 5;
    public float speed = 1;
    public int maxHP = 50;
    private int currentHP;
    private bool isFacingRight = false;
    public GameObject attackPoint;
    public float attackRange = 0.5f;
    public float attackTimer;
    public Rigidbody2D goldCoin;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PolygonCollider2D>();
        currentHP = maxHP;
        attackPoint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        distance = Vector2.Distance(target.position, transform.position);

        if (distance < range)
        {
            animator.SetBool("Walk", true);

            if (isFacingRight && target.position.x < transform.position.x)
            {
                FlipSlime();
            }
            else if (!isFacingRight && target.position.x > transform.position.x)
            {
                FlipSlime();
            }

            ChaseTarget();
        }

        animator.SetBool("Walk", false);

        if (distance < attackRange && attackTimer > 1.25)
        {
            Debug.Log("ATTACK RANGE");
            StartCoroutine(AttackCollision());

            attackTimer = 0.0f;
        }

        if (currentHP <= 0)
        {
            Die();
        }

    }

    IEnumerator AttackCollision()
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.7f);
        attackPoint.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackPoint.SetActive(false);
    }

    void ChaseTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void Die()
    {
        DropGold();
        this.enabled = false;
        pc.enabled = false;
    }

    void DropGold()
    {
        int rand = Random.Range(1, 6);
        Vector2 randDirection;
        int randSpeed;

        for (int i = 0; i < rand; ++i)
        {
            randSpeed = Random.Range(100, 200);
            randDirection = Random.insideUnitCircle.normalized;
            Rigidbody2D coin = Instantiate(goldCoin, transform.position, transform.rotation);
            coin.AddRelativeForce(randDirection * 200);
        }
    }

    IEnumerator DamageAnimation()
    {
        transform.localScale = new Vector2(1, 2.5f);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.4f);

        if (currentHP > 0)
        {
            sr.color = Color.white;
            transform.localScale = new Vector2(2.5f, 2.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "AttackPoint")
        {
            TakeDamage(25);
        }
    }

    void TakeDamage(int damage)
    {
        currentHP -= damage;

        // knockback
        if (target.position.x < transform.position.x)
        {
            rb.velocity = new Vector2(5 * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-5 * speed, rb.velocity.y);
        }

        // flash red and squish
        StartCoroutine(DamageAnimation());
    }

    void FlipSlime()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
