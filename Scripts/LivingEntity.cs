using System;

namespace LabsonCS
{
    public class LivingEntity
    {
        protected string name;
        protected double healthPoints;

        public LivingEntity(string name, double healthPoints)
        {
            this.name = name;
            this.healthPoints = healthPoints;
        }

        protected void Hurt(double damage)
        {
            healthPoints -= damage;
            if (healthPoints <= 0.0F)
            {
                healthPoints = 0.0F;
                Console.WriteLine(name + " entity is dead!");
            }

        }

        public void Print()
        {
            Console.WriteLine(name + " entity:");
            Console.WriteLine("Health points: " + healthPoints);
        }

        public double GetHealthPoints()
        {
            return healthPoints;
        }
    }

}
