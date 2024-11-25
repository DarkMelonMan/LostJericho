namespace LabsonCS
{
    public class PlayerEntity: LivingEntity
    {
    private readonly Armor armor;
    private readonly Weapon weapon;

        public PlayerEntity(string name, double healthPoints, Armor armor, Weapon weapon) : base(name, healthPoints) {
        this.armor = armor;
        this.weapon = weapon;
    }

    public Armor GetArmor()
    {
        return armor;
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void Hurt(double baseDamage, double elementDamage, Element damageType)
    {
        Hurt(armor.GetActualDamage(baseDamage, elementDamage, damageType));
    }

    public void AttackMonster(MonsterEntity monster)
    {
        monster.Hurt(weapon.GetDamage(monster.GetMonsterWeakness()));
    }

    public new void Print()
    {
        base.Print();
        armor.Print();
        weapon.Print();
    }
}
}
