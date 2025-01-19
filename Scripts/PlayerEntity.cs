using UnityEngine;

namespace LabsonCS
{
    public class PlayerEntity : LivingEntity
    {

        public Armor Armor { get; private set; }
        public Weapon Weapon { get; private set; }
        public float RollForce { get; private set; }
        public float JumpForce { get; private set; }
        public HealthBar HealthBar { get; private set; }
        public bool Rolling { get; private set; } = false;
        private float rollCurrentTime;
        private const float rollDuration = 8.0f / 14.0f;
        public bool Dead { get; private set; } = false;
        private readonly Inventory inventory;
        private bool grounded = false;
        private readonly PlayerSensor groundSensor;
        private Area attackArea;
        private float delayToIdle = 0.0f;
        public PlayerEntity(float healthPoints, float movementSpeed, float rollForce, float jumpForce, Armor armor, Weapon weapon, 
            HealthBar healthBar, Animator animator, Rigidbody2D rb, Inventory inventory, PlayerSensor groundSensor, Area attackArea) : base(healthPoints, movementSpeed, animator, rb)
        {
            Armor = armor;
            Weapon = weapon;
            RollForce = rollForce;
            JumpForce = jumpForce;
            HealthBar = healthBar;
            this.animator = animator;
            this.inventory = inventory;
            this.groundSensor = groundSensor;
            this.attackArea = attackArea;
        }

        public void Hurt(float baseDamage, float elementDamage, Element damageType)
        {
            if (!Rolling && !Dead)
            {
                animator.SetTrigger("Hurt");
                Hurt(Armor.GetActualDamage(baseDamage, elementDamage, damageType));
                HealthBar.SetHealth(HealthPoints);
                if (animator.GetBool("IdleBlock"))
                    animator.SetBool("IdleBlock", false);
            }
        }

        public void Roll()
        {
            if (!Rolling && !animator.GetBool("Hurt") && !animator.GetBool("IdleBlock"))
            {
                Rolling = true;
                animator.SetTrigger("Roll");
                body2d.velocity = new Vector2(FacingDirection * RollForce, body2d.velocity.y);
            }
        }

        public void Jump()
        {
            if (grounded && !Rolling && !animator.GetBool("Hurt") && !animator.GetBool("IdleBlock"))
            {
                animator.SetTrigger("Jump");
                grounded = false;
                animator.SetBool("Grounded", grounded);
                body2d.velocity = new Vector2(body2d.velocity.x, JumpForce);
                groundSensor.Disable(0.2f);
            }
        }
        public void IncreaseRollTimer()
        {
            if (Rolling)
                rollCurrentTime += Time.deltaTime;
        }

        public void UpdateRollingStatus()
        {
            if (rollCurrentTime > rollDuration)
            {
                Rolling = false;
                rollCurrentTime = 0;
            }
        }
        public void ChangeDirection(SpriteRenderer sprite, float inputX)
        {
            if (inputX > 0 && !Rolling && !animator.GetBool("Hurt") && !animator.GetBool("IdleBlock"))
            {
                sprite.flipX = false;
                FacingDirection = 1;
                attackArea.ChangeDirection(FacingDirection);
            }
            else if (inputX < 0 && !Rolling && !animator.GetBool("Hurt") && !animator.GetBool("IdleBlock"))
            {
                sprite.flipX = true;
                FacingDirection = -1;
                attackArea.ChangeDirection(FacingDirection);
            }
        }
        public void Die()
        {
            animator.SetTrigger("Death");
            Dead = true;
        }
        public void HideInventory() { inventory.hidden = true; }

        public void ShowInventory() { inventory.hidden = false; }

        public bool IsInventoryHidden() {  return inventory.hidden; }

        public void UpdateGroundSensorState()
        {
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
        }
        public void Move(float inputX)
        {
            if (!Rolling && !animator.GetBool("Hurt") && !animator.GetBool("IdleBlock"))
                body2d.velocity = new Vector2(inputX * MovementSpeed, body2d.velocity.y);
        }

        public void Hit(Vector3 position)
        {
            Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackArea.SideTransform.position, attackArea.SideArea, 0, attackArea.AreaLayer);
            for (int i = 0; i < objectsToHit.Length; i++)
            {
                Enemy enemy = objectsToHit[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    MonsterEntity monster = enemy.monster;
                    monster?.Hurt(Weapon.GetDamage(enemy.monster.Weakness), (position - objectsToHit[i].transform.position).normalized, 100);
                }
            }
        }
        // Переход из бега в состояние покоя
        public void TransitionToIdle()
        {
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                animator.SetInteger("AnimState", 0);
        }
        // Сущность начала движение
        public void ResetIdleDelayTimer()
        {
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }
    }
}
