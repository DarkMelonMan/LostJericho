using System;

namespace LabsonCS
{
    public class LivingEntity
    {
        protected double healthPoints;

        public LivingEntity(double healthPoints)
        {
            this.healthPoints = healthPoints;
        }

        protected void Hurt(double damage)
        {
            healthPoints -= damage;
            if (healthPoints <= 0.0F)
            {
                healthPoints = 0.0F;
            }

        }

        public double GetHealthPoints()
        {
            return healthPoints;
        }
    }

}
