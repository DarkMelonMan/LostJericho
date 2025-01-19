using LabsonCS;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]float maxHealth;
    [SerializeField] float baseDamage;
    [SerializeField] float elementDamage;
    [SerializeField] Element damageType;
    [SerializeField] Element weakness;
    public MonsterEntity monster;
    [SerializeField] float recoilFactor;
    [SerializeField] float recoilLength;
    [SerializeField] Vector2 detectionAreaVector;
    public bool animationAttack;
    public bool animationDeath;
    
    bool isPlayerDead = false;
    [SerializeField] float movementSpeed;

    Area detectionArea;
    Area attackArea;
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;

    Animator animator;
    Rigidbody2D rb;

    float timeSinceAttack = 0.0f; 
    [SerializeField] float timeBetweenAttacks = 1f; 
    [SerializeField] float attackSpeed;
    public bool isAttacking;
    private bool isDying;
    private const float timeBeforeDeath = 5.0f;
    private float deathTimer = 0.0f;
    private bool isPlayerAttacked = false;
    PlayerEntity player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        detectionArea = new Area(transform, detectionAreaVector, attackableLayer);
        attackArea = new Area(SideAttackTransform, SideAttackArea, attackableLayer);
        animator.SetFloat("MovementSpeed", movementSpeed);
        animator.SetFloat("AttackSpeed", attackSpeed);
        monster = new MonsterEntity(maxHealth, movementSpeed, baseDamage, elementDamage, weakness, damageType, animator, rb, detectionArea, attackArea, recoilLength, recoilFactor);
    }

    // Update is called once per frame
    void Update()
    {
        if (monster.HealthPoints <= 0 && !isDying)
        {
            animationDeath = true;
            isDying = true;
            animator.SetTrigger("Death");
        }
        else if (isDying && deathTimer < timeBeforeDeath)
            deathTimer += Time.deltaTime;
        else if (!animationDeath && deathTimer > timeBeforeDeath)
            Destroy(gameObject);
        if (!isDying)
        {
            monster.UpdateRecoilTimer();
            PlayerEntity player = monster.FindPlayer();
            if (monster.IsPlayerDetected && !isPlayerDead)
            {
                if (!isAttacking)
                    transform.position = monster.Move(transform.position, GetComponent<SpriteRenderer>());
                Attack();
                if (player != null)
                {
                    if (animationAttack && Math.Abs(monster.PlayerPosition.x - transform.position.x) <= SideAttackArea.x * 1.5 &&
                        Math.Abs(monster.PlayerPosition.y - transform.position.y) <= SideAttackArea.y * 1.2 && !isPlayerAttacked)
                    {
                        player.Hurt(monster.BaseDamage, monster.ElementDamage, monster.DamageType);
                        isPlayerAttacked = true;
                    }
                    if (player.Dead)
                        isPlayerDead = true;
                }
            }
        }
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (timeSinceAttack > timeBetweenAttacks)
        {
            isPlayerAttacked = false;
            timeSinceAttack = 0;
            monster.Hit();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(transform.position, detectionAreaVector);
    }
}
