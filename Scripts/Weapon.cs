using System;
using System.Collections.Generic;
using System.Text;

namespace LabsonCS
{
    public class Weapon
    {
        private readonly float baseDamage;
        private readonly float elementDamage;
        private readonly Element damageType;
       
        public Weapon(float baseDamage, float elementDamage, Element damageType)
        {
            this.baseDamage = baseDamage;
            this.elementDamage = elementDamage;
            this.damageType = damageType;
        }

        public float GetDamage(Element weakness)
        {
            float damage = baseDamage;
            if (weakness == damageType && weakness != Element.NONE)
                damage += elementDamage;
            else if (weakness != Element.NONE)
                damage += elementDamage * 0.5f;
            return damage;
        }
    }
}
