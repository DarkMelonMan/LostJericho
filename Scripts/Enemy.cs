using LabsonCS;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField ]float health;
    [SerializeField] double baseDamage;
    [SerializeField] double elementDamage;
    [SerializeField] Element damageType;
    [SerializeField] Element weakness;
    public MonsterEntity monster;
    [SerializeField] Color hpbarColor;

    // Start is called before the first frame update
    void Start()
    {
        monster = new MonsterEntity("Blob", health, baseDamage, elementDamage, weakness, damageType);   
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) { 
            Destroy(gameObject);
        }
    }


    public void EnemyHit(double _DamageDone) { 
        health -= (float)_DamageDone;
    }
}
