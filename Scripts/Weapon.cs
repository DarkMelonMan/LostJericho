using System;
using System.Collections.Generic;
using System.Text;

namespace LabsonCS
{
    public class Weapon
    {
        private readonly double baseDamage;
        private readonly double elementDamage;
        private readonly Element damageType;

        public Weapon(double baseDamage, double elementDamage, Element damageType)
        {
            this.baseDamage = baseDamage;
            this.elementDamage = elementDamage;
            this.damageType = damageType;
        }

        public double GetDamage(Element weakness)
        {
            double damage = baseDamage;
            if (weakness == damageType && weakness != Element.NONE)
                damage += elementDamage;
            else if (weakness != Element.NONE)
                damage += elementDamage * 0.5;
            return damage;
        }

        public void Print()
        {
            Console.WriteLine("Player weapon: ");
            Console.WriteLine("Base damage: " + baseDamage);
            Console.WriteLine("Elemental damage: " + elementDamage);
            Console.WriteLine("Elemental type of damage: " + damageType);
        }
    }
}
