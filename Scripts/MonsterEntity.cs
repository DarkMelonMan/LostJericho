using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LabsonCS
{
    public class MonsterEntity: LivingEntity {
        public float BaseDamage { get; private set; }
        public float ElementDamage { get; private set; }
        public Element Weakness { get; private set; }
        public Element DamageType { get; private set; }
        private readonly Area detectionArea;
        private readonly Area attackArea;
        public bool IsPlayerDetected { get; private set; }
        public float RecoilLength { get; private set; }
        public float RecoilFactor { get; private set; }
        private bool isRecoiling = false;
        private float recoilTimer;
        private PlayerEntity player;
        public Vector2 PlayerPosition { get; private set; }
        public MonsterEntity(float healthPoints, float movementSpeed, float baseDamage, float elementDamage, Element weakness, 
            Element damageType, Animator animator, Rigidbody2D rb, Area detectionArea, Area attackArea, float recoilLength, float recoilFactor): base(healthPoints, movementSpeed, animator, rb)
        {
            BaseDamage = baseDamage;
            ElementDamage = elementDamage;
            Weakness = weakness;
            DamageType = damageType;
            this.animator = animator;
            this.detectionArea = detectionArea;
            this.attackArea = attackArea;
            RecoilFactor = recoilFactor;
            RecoilLength = recoilLength;
        }

        public void AttackPlayer(PlayerEntity player)
        {
            player.Hurt(BaseDamage, ElementDamage, DamageType);
        }
        public void ChangeDirection(SpriteRenderer sprite, float inputX)
        {
            if (inputX > 0 && !animator.GetBool("Hurt"))
            {
                sprite.flipX = false;
                FacingDirection = 1;
                attackArea.ChangeDirection(FacingDirection);
            }
            else if (inputX < 0 && !animator.GetBool("Hurt"))
            {
                sprite.flipX = true;
                FacingDirection = -1;
                attackArea.ChangeDirection(FacingDirection);
            }
        }
        public GameObject DetectPlayer()
        {
            Collider2D[] detectedObjects = Physics2D.OverlapBoxAll(detectionArea.SideTransform.position, detectionArea.SideArea, 0, detectionArea.AreaLayer);

            for (int i = 0; i < detectedObjects.Length; i++)
            {
                if (detectedObjects[i].CompareTag("Player"))
                {
                    IsPlayerDetected = true;
                    PlayerPosition = detectedObjects[i].transform.position;
                    return detectedObjects[i].gameObject;
                }
            }
            IsPlayerDetected = false;
            animator.SetInteger("AnimState", 0);
            return null;
        }
        public void UpdateRecoilTimer()
        {
            if (isRecoiling)
            {
                if (recoilTimer < RecoilLength)
                {
                    recoilTimer += Time.deltaTime;
                }
                else
                {
                    isRecoiling = false;
                    recoilTimer = 0;
                }
            }
        }
        public void Hurt(float DamageDone, Vector2 hitDirection, float hitForce)
        { 
            Hurt(DamageDone);
            if (!isRecoiling)
            {
                body2d.AddForce(-hitForce * RecoilFactor * hitDirection);
                animator.SetTrigger("Hurt");
            }
        }
        public void Hit()
        {
            Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackArea.SideTransform.position, attackArea.SideArea, 0, attackArea.AreaLayer);

            for (int i = 0; i < objectsToHit.Length; i++)
            {
                if (objectsToHit[i].CompareTag("Player"))
                {
                    if (FindPlayer() != null)
                    {
                        animator.SetTrigger("Attack1");
                    };
                }
            }
        }
        public PlayerEntity FindPlayer()
        {
            GameObject playerObject = DetectPlayer();
            HeroKnight hero;
            if (playerObject != null)
            {
                hero = playerObject.GetComponent<HeroKnight>();
                if (hero != null)
                {
                    player = hero.player;
                    return player;
                }
            }
            return null;
        }
        public Vector2 Move(Vector2 position, SpriteRenderer sprite)
        {
            if (Math.Abs(PlayerPosition.x - position.x) > attackArea.SideArea.x * 1.5f)
            {
                animator.SetInteger("AnimState", 1);
                ChangeDirection(sprite, PlayerPosition.x - position.x);
                return new Vector2(Vector2.MoveTowards(position, PlayerPosition, Time.deltaTime * MovementSpeed).x, position.y);
            }
            else
            {
                animator.SetInteger("AnimState", 0);
            }
            return position;
        }
    }
}
