using LabsonCS;
using UnityEngine;

public class HeroKnight : MonoBehaviour
{

    PlayerEntity player;
    Armor defaultArmor;
    Weapon defaultWeapon;
    [SerializeField] double baseDefence;
    [SerializeField] double elementDefence;
    [SerializeField] Element defenceType;
    [SerializeField] double baseDamage = 1;
    [SerializeField] double elementDamage = 0;
    [SerializeField] Element damageType = Element.NONE;

    [SerializeField] float maxHealthPoints = 100.0f;
    float healthPoints;
    public HealthBar healthBar;
    bool dead = false;

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    public static HeroKnight Instance;
    [Header("Attacking")]

    private bool attack = false;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_timeBetween;

    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;

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
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        inventory.hidden = true;
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        defaultArmor = new Armor(baseDefence, elementDefence, defenceType);
        defaultWeapon = new Weapon(baseDamage, elementDamage, damageType);
        healthPoints = maxHealthPoints;
        player = new PlayerEntity(healthPoints, defaultArmor, defaultWeapon);
        healthBar.SetMaxHealth(healthPoints);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
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
        m_timeSinceAttack += Time.deltaTime;

        // Открыть инвентарь
        if (Input.GetKeyDown("tab"))
            inventory.hidden = inventory.hidden ? false : true;

        // Увеличить таймер, считающий время в перекате
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Отключить перекат, если время переката окончено
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;
            m_rollCurrentTime = 0;
        }

        // Проверить, если персонаж только приземлился
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Проверить, если персонаж начал падать
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        float inputX = Input.GetAxis("Horizontal");

        // Сменить направление спрайта в зависимости от направления движения
        if (inputX > 0 && !m_animator.GetBool("IdleBlock") && !m_animator.GetBool("Hurt") && !m_rolling)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0 && !m_animator.GetBool("IdleBlock") && !m_animator.GetBool("Hurt") && !m_rolling)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Движение
        if (!m_rolling && !m_animator.GetBool("IdleBlock") && !m_animator.GetBool("Hurt"))
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        // Установить скорость движения в воздухе
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);


        // Скольжение по стене
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // Умереть
        if (healthPoints <= 0 && !m_rolling)
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
            m_animator.SetBool("IdleBlock", false);

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
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        // Анимация покоя
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    void Attack()
    {
        if (!m_rolling && !m_animator.GetBool("IdleBlock") && !m_animator.GetBool("Hurt"))
        {
            m_currentAttack++;
            m_timeSinceAttack += Time.deltaTime;
            if (attack && m_timeSinceAttack >= m_timeBetween && m_timeSinceAttack > 0.25f && !m_rolling)
            {
                m_timeSinceAttack = 0;
                // Loop back to one after third attack
                if (m_currentAttack > 3)
                    m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                    m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);

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
        if (!m_rolling && !m_isWallSliding && !m_animator.GetBool("IdleBlock") && !m_animator.GetBool("Hurt"))
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
    }

    void Block()
    {
        if (!m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
    }

    void Jump()
    {
        if (m_grounded && !m_rolling && !m_animator.GetBool("IdleBlock") && !m_animator.GetBool("Hurt"))
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
    }

    void Die()
    {
        m_animator.SetBool("noBlood", m_noBlood);
        m_animator.SetTrigger("Death");
        dead = true;
    }

    void Hurt()
    {
        if (!m_rolling)
        {
            m_animator.SetTrigger("Hurt");
            healthPoints -= 5;
            healthBar.SetHealth(healthPoints);
            if (m_animator.GetBool("IdleBlock"))
                m_animator.SetBool("IdleBlock", false);
        }
    }
}