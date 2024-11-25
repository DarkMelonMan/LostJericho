using System;
using System.Collections.Generic;
using System.Text;

namespace LabsonCS
{
    public class Armor
    {
        private readonly double baseDefence;
        private readonly double elementDefence;
        private readonly Element defenceType;

        public Armor(double baseDefence, double elementDefence, Element defenceType)
        {
            this.baseDefence = baseDefence;
            this.elementDefence = elementDefence;
            this.defenceType = defenceType;
        }

        public double GetActualDamage(double baseDamage, double elementDamage, Element damageType)
        {
            double damage = baseDamage * (1 - baseDefence / 100);
            if (defenceType == damageType && damageType != Element.NONE)
                damage += elementDamage * (1 - elementDefence / 100);
            else if (damageType != Element.NONE) damage += elementDamage;
            return damage;
        }

        public void Print()
        {
            Console.WriteLine("Player armor: ");
            Console.WriteLine("Base defence: " + baseDefence);
            Console.WriteLine("Elemental defence: " + elementDefence);
            Console.WriteLine("Elemental type of defence: " + defenceType);
        }
    }
}
