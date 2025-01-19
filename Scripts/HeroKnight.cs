using LabsonCS;
using UnityEngine;

public class HeroKnight : MonoBehaviour
{

    public PlayerEntity player { get; private set; }
    Armor defaultArmor;
    Weapon defaultWeapon;

    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject slideDust;

    [Header("Attacking")]
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;

    [Header("Stats")]
    [SerializeField] float maxHealthPoints = 100.0f;
    [SerializeField] float speed = 4.0f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float rollForce = 6.0f;

    [Header("Damage Stats")]
    [SerializeField] float baseDefence;
    [SerializeField] float elementDefence;
    [SerializeField] Element defenceType;
    [SerializeField] float baseDamage = 1;
    [SerializeField] float elementDamage = 0;
    [SerializeField] Element damageType = Element.NONE;


    private Animator animator;
    private Rigidbody2D body2d;
    private PlayerSensor groundSensor;
    private PlayerSensor wallSensorR1;
    private PlayerSensor wallSensorR2;
    private PlayerSensor wallSensorL1;
    private PlayerSensor wallSensorL2;
    private bool isWallSliding = false;

    public static HeroKnight Instance;

    private bool attack = false;
    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;
    private const float timeBetweenAttacks = 0.25f;


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
        Area attackArea = new Area(SideAttackTransform, SideAttackArea, attackableLayer);
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        inventory = GetComponent<Inventory>();
        groundSensor = transform.Find("GroundSensor").GetComponent<PlayerSensor>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<PlayerSensor>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<PlayerSensor>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<PlayerSensor>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<PlayerSensor>();
        defaultArmor = new Armor(baseDefence, elementDefence, defenceType);
        defaultWeapon = new Weapon(baseDamage, elementDamage, damageType);
        player = new PlayerEntity(maxHealthPoints, speed, rollForce, jumpForce, defaultArmor, 
            defaultWeapon, healthBar, animator, body2d, inventory, groundSensor, attackArea);
        player.HideInventory();
        player.HealthBar.SetMaxHealth(player.HealthPoints);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.Dead)
        {
            GetInputs();
            Attack();
        }
    }

    void GetInputs()
    {

        attack = Input.GetMouseButtonDown(0);

        // Увеличить таймер, считающий комбо-удары

        // Открыть инвентарь
        if (Input.GetKeyDown(KeyCode.Tab))
            if (player.IsInventoryHidden()) player.ShowInventory();
            else player.HideInventory();

        // Увеличить таймер, считающий время в перекате
        player.IncreaseRollTimer();

        // Отключить перекат, если время переката окончено
        player.UpdateRollingStatus();

        player.UpdateGroundSensorState();

        float inputX = Input.GetAxis("Horizontal");

        // Сменить направление спрайта в зависимости от направления движения
        player.ChangeDirection(GetComponent<SpriteRenderer>(), inputX);

        // Движение
        player.Move(inputX);

        // Установить скорость движения в воздухе
        animator.SetFloat("AirSpeedY", body2d.velocity.y);

        // Скольжение по стене
        isWallSliding = (wallSensorR1.State() && wallSensorR2.State()) || (wallSensorL1.State() && wallSensorL2.State());
        animator.SetBool("WallSlide", isWallSliding);

        // Умереть
        if (player.HealthPoints <= 0 && !player.Rolling)
            player.Die();

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
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !isWallSliding)
            player.Roll();

        // Прыжок
        else if (Input.GetKeyDown(KeyCode.Space))
            player.Jump();

        // Бег
        else if (Mathf.Abs(inputX) > Mathf.Epsilon) 
            player.ResetIdleDelayTimer();

        // Анимация покоя
        else player.TransitionToIdle();
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (player.FacingDirection == 1)
            spawnPosition = wallSensorR2.transform.position;
        else
            spawnPosition = wallSensorL2.transform.position;

        if (slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(player.FacingDirection, 1, 1);
        }
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (!player.Rolling && !animator.GetBool("IdleBlock") && !animator.GetBool("Hurt"))
        {
            if (attack && timeSinceAttack >= timeBetweenAttacks && !player.Rolling)
            {
                currentAttack++;
                timeSinceAttack = 0;
                // Loop back to one after third attack
                if (currentAttack > 3)
                    currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (timeSinceAttack > 1.0f)
                    currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                animator.SetTrigger("Attack" + currentAttack);

                player.Hit(transform.position);
            }
        }
    }

    void Block()
    {
        if (!player.Rolling)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }
    }
}