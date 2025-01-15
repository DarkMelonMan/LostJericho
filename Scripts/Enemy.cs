using LabsonCS;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    [SerializeField]float maxHealth;
    public float health;
    [SerializeField] float baseDamage;
    [SerializeField] float elementDamage;
    [SerializeField] Element damageType;
    [SerializeField] Element weakness;
    public MonsterEntity monster;
    [SerializeField] float recoilLength;
    [SerializeField] float recoilFactor;
    [SerializeField] Vector2 DetectionArea;

    [SerializeField] bool isRecoiling = false;
    float recoilTimer;
    Vector2 playerPosition;
    bool isPlayerDetected = false;
    [SerializeField] float movementSpeed;

    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;
    float sideAttackTransformX;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        monster = new MonsterEntity(health, baseDamage, elementDamage, weakness, damageType);
        health = maxHealth;
        sideAttackTransformX = SideAttackTransform.localPosition.x;
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
        DetectPlayer();
        if (isPlayerDetected)
        {
            Move();
        }
    }

    void Move()
    {
        transform.position = new Vector2(Vector2.MoveTowards(transform.position, playerPosition, Time.deltaTime * movementSpeed).x, transform.position.y);
        if (transform.position.x - playerPosition.x > 0)
        {
            SideAttackTransform.localPosition = new Vector2(-sideAttackTransformX, SideAttackTransform.localPosition.y);
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else {
            SideAttackTransform.localPosition = new Vector2(sideAttackTransformX, SideAttackTransform.localPosition.y);
            GetComponent<SpriteRenderer>().flipX = true; 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(transform.position, DetectionArea);
    }

    public void EnemyHit(float DamageDone, Vector2 hitDirection, float hitForce) { 
        health -= DamageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-hitForce * recoilFactor * hitDirection);
        }
    }

    void DetectPlayer()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapBoxAll(transform.position, DetectionArea, 0, attackableLayer);

        for (int i = 0; i < detectedObjects.Length; i++)
        {
            if (detectedObjects[i].CompareTag("Player"))
            {
                isPlayerDetected = true;
                playerPosition = detectedObjects[i].gameObject.transform.position;
                break;
            }
        }
    }
}
