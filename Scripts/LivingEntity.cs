using System;
using UnityEngine;

namespace LabsonCS
{
    public class LivingEntity
    {
        public int FacingDirection { get; protected set; } = 1;
        public float HealthPoints { get; protected set; }
        public float MovementSpeed { get; protected set; }
        protected Animator animator;
        protected readonly Rigidbody2D body2d;

        public LivingEntity(float healthPoints, float movementSpeed, Animator animator, Rigidbody2D body2d)
        {
            HealthPoints = healthPoints;
            MovementSpeed = movementSpeed;
            this.animator = animator;
            this.body2d = body2d;
        }

        protected void Hurt(float damage)
        {
            HealthPoints -= damage;
            if (HealthPoints <= 0.0F)
            {
                HealthPoints = 0.0F;
            }
        }
    }
}
