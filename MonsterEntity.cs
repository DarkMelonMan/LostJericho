using System;
using System.Collections.Generic;
using System.Text;

namespace LabsonCS
{
    public class MonsterEntity: LivingEntity
    {
    double baseDamage;
    double elementDamage;
    Element weakness;
    Element damageType;

    public MonsterEntity(string name, double healthPoints, double movementSpeed, double baseDamage, double elementDamage, Element weakness, Element damageType): base(name, healthPoints, movementSpeed)
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

    public new void Print()
    {
        base.Print();
        Console.WriteLine("Base damage: " + baseDamage);
        Console.WriteLine("Elemental damage: " + elementDamage);
        Console.WriteLine("Elemental weakness type: " + weakness);
        Console.WriteLine("Elemental type of damage: " + damageType);
    }
    public Element GetMonsterWeakness()
    {
        return weakness;
    }
}
}
