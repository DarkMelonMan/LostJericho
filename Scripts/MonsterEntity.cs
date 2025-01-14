using System;
using System.Collections.Generic;
using System.Text;

namespace LabsonCS
{
    public class MonsterEntity: LivingEntity {
        double baseDamage;
        double elementDamage;
        Element weakness;
        Element damageType;
    public MonsterEntity(double healthPoints, double baseDamage, double elementDamage, Element weakness, Element damageType): base(healthPoints)
    {
        this.baseDamage = baseDamage;
        this.elementDamage = elementDamage;
        this.weakness = weakness;
        this.damageType = damageType;
    }
    public new void Hurt(double damage)
    {
        Hurt(damage);
    }

    public void AttackPlayer(PlayerEntity player)
    {
        player.Hurt(baseDamage, elementDamage, damageType);
    }

    public Element GetMonsterWeakness()
    {
        return weakness;
    }
}
}
