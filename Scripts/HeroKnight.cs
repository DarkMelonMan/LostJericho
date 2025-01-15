using LabsonCS;
using UnityEngine;

public class HeroKnight : MonoBehaviour
{

    PlayerEntity player;
    Armor defaultArmor;
    Weapon defaultWeapon;
    [SerializeField] float baseDefence;
    [SerializeField] float elementDefence;
    [SerializeField] Element defenceType;
    [SerializeField] float baseDamage = 1;
    [SerializeField] float elementDamage = 0;
    [SerializeField] Element damageType = Element.NONE;

    [SerializeField] float maxHealthPoints = 100.0f;
    float healthPoints;
    public HealthBar healthBar;
    bool dead = false;

    [SerializeField] float speed = 4.0f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float rollForce = 6.0f;
    [SerializeField] GameObject slideDust;

    private Animator animator;
    private Rigidbody2D body2d;
    private Sensor_HeroKnight groundSensor;
    private Sensor_HeroKnight wallSensorR1;
    private Sensor_HeroKnight wallSensorR2;
    private Sensor_HeroKnight wallSensorL1;
    private Sensor_HeroKnight wallSensorL2;
    private bool isWallSliding = false;
    private bool grounded = false;
    private bool rolling = false;
    private int facingDirection = 1;
    private float delayToIdle = 0.0f;
    private float rollDuration = 8.0f / 14.0f;
    private float rollCurrentTime;

    public static HeroKnight Instance;
    [Header("Attacking")]

    private bool attack = false;
    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;
    private float timeBetween;

    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;
    float sideAttackTransformX;

    Inventory inventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    // Use this for initialization
    void Start()
    {
        sideAttackTransformX = SideAttackTransform.localPosition.x;
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        inventory.hidden = true;
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        defaultArmor = new Armor(baseDefence, elementDefence, defenceType);
        defaultWeapon = new Weapon(baseDamage, elementDamage, damageType);
        healthPoints = maxHealthPoints;
        player = new PlayerEntity(healthPoints, defaultArmor, defaultWeapon);
        healthBar.SetMaxHealth(healthPoints);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            GetInputs();
            Attack();
        }
    }

    void GetInputs()
    {

        attack = Input.GetMouseButtonDown(0);

        // Увеличить таймер, считающий комбо-удары
        timeSinceAttack += Time.deltaTime;

        // Открыть инвентарь
        if (Input.GetKeyDown("tab"))
            inventory.hidden = inventory.hidden ? false : true;

        // Увеличить таймер, считающий время в перекате
        if (rolling)
            rollCurrentTime += Time.deltaTime;

        // Отключить перекат, если время переката окончено
        if (rollCurrentTime > rollDuration)
        {
            rolling = false;
            rollCurrentTime = 0;
        }

        // Проверить, если персонаж только приземлился
        if (!grounded && groundSensor.State())
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        // Проверить, если персонаж начал падать
        if (grounded && !groundSensor.State())
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }

        float inputX = Input.GetAxis("Horizontal");

        // Сменить направление спрайта в зависимости от направления движения
        if (inputX > 0 && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt") && !rolling)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
            SideAttackTransform.localPosition = new Vector2(sideAttackTransformX, SideAttackTransform.localPosition.y);
        }

        else if (inputX < 0 && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt") && !rolling)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
            SideAttackTransform.localPosition = new Vector2(-sideAttackTransformX, SideAttackTransform.localPosition.y);
        }


        // Движение
        if (!rolling && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt"))
            body2d.velocity = new Vector2(inputX * speed, body2d.velocity.y);

        // Установить скорость движения в воздухе
        animator.SetFloat("AirSpeedY", body2d.velocity.y);


        // Скольжение по стене
        isWallSliding = (wallSensorR1.State() && wallSensorR2.State()) || (wallSensorL1.State() && wallSensorL2.State());
        animator.SetBool("WallSlide", isWallSliding);

        // Умереть
        if (healthPoints <= 0 && !rolling)
            Die();

        // Получить урон
        else if (Input.GetKeyDown("q"))
            Hurt();

        // Атаковать
        else if (Input.GetMouseButtonDown(0))
            Attack();

        // Поднять щит
        else if (Input.GetMouseButtonDown(1))
            Block();

        // Убрать щит
        else if (Input.GetMouseButtonUp(1))
            animator.SetBool("IdleBlock", false);

        // Перекат
        else if (Input.GetKeyDown("left shift"))
            Roll();

        // Прыжок
        else if (Input.GetKeyDown("space"))
            Jump();

        // Бег
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }

        // Анимация покоя
        else
        {
            // Prevents flickering transitions to idle
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (facingDirection == 1)
            spawnPosition = wallSensorR2.transform.position;
        else
            spawnPosition = wallSensorL2.transform.position;

        if (slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }
    void Attack()
    {
        if (!rolling && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt"))
        {
            currentAttack++;
            timeSinceAttack += Time.deltaTime;
            if (attack && timeSinceAttack >= timeBetween && timeSinceAttack > 0.25f && !rolling)
            {
                timeSinceAttack = 0;
                // Loop back to one after third attack
                if (currentAttack > 3)
                    currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (timeSinceAttack > 1.0f)
                    currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                animator.SetTrigger("Attack" + currentAttack);

                Hit(SideAttackTransform, SideAttackArea);

            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            Debug.Log("Hit");
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy enemy = objectsToHit[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyHit(player.GetWeapon().GetDamage(enemy.monster.GetMonsterWeakness()), (transform.position - objectsToHit[i].transform.position).normalized, 100);
            };
        }

    }

    void Roll()
    {
        if (!rolling && !isWallSliding && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt"))
        {
            rolling = true;
            animator.SetTrigger("Roll");
            body2d.velocity = new Vector2(facingDirection * rollForce, body2d.velocity.y);
        }
    }

    void Block()
    {
        if (!rolling)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }
    }

    void Jump()
    {
        if (grounded && !rolling && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt"))
        {
            animator.SetTrigger("Jump");
            grounded = false;
            animator.SetBool("Grounded", grounded);
            body2d.velocity = new Vector2(body2d.velocity.x, jumpForce);
            groundSensor.Disable(0.2f);
        }
    }

    void Die()
    {
        animator.SetBool("noBlood", true);
        animator.SetTrigger("Death");
        dead = true;
    }

    void Hurt()
    {
        if (!rolling)
        {
            animator.SetTrigger("Hurt");
            healthPoints -= 5;
            healthBar.SetHealth(healthPoints);
            if (animator.GetBool("IdleBlock"))
                animator.SetBool("IdleBlock", false);
        }
    }
}
