using LabsonCS;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]float maxHealth;
    public float health;
    [SerializeField] double baseDamage;
    [SerializeField] double elementDamage;
    [SerializeField] Element damageType;
    [SerializeField] Element weakness;
    public MonsterEntity monster;
    [SerializeField] float recoilLength;
    [SerializeField] float recoilFactor;

    [SerializeField] bool isRecoiling = false;
    float recoilTimer;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        monster = new MonsterEntity(health, baseDamage, elementDamage, weakness, damageType);
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) { 
            Destroy(gameObject);
        }
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public void EnemyHit(double DamageDone, Vector2 hitDirection, float _hitForce) { 
        health -= (float) DamageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * hitDirection);
        }
    }
}
