using System;
using System.Collections.Generic;
using System.Text;

namespace LabsonCS
{
    public class Armor
    {
        private readonly float baseDefence;
        private readonly float elementDefence;
        private readonly Element defenceType;
        public Armor(float baseDefence, float elementDefence, Element defenceType)
        {
            this.baseDefence = baseDefence;
            this.elementDefence = elementDefence;
            this.defenceType = defenceType;
        }

        public float GetActualDamage(float baseDamage, float elementDamage, Element damageType)
        {
            float damage = baseDamage * (1 - baseDefence / 100);
            if (defenceType == damageType && damageType != Element.NONE)
                damage += elementDamage * (1 - elementDefence / 100);
            else if (damageType != Element.NONE) damage += elementDamage;
            return damage;
        }
    }
}
