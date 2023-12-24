using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class Enemy1 : MonoBehaviour
{
    public GameObject playerObject;
    private Player player;
    public SpriteRenderer sr;
    public Slider hpSlider;
    public Rigidbody2D rb;
    public float maxHp;
    public float hp;
    public float speed;
    public float enemyAgroRange;
    public float damage;
    
    private float oldPosition;

    private float distance;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        hp = maxHp;
        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;
        oldPosition = transform.position.x;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Weapon" && player.canDealDmg)
        {
            hp -= player.damage;
            hpSlider.value = hp;
        }
    }

    void Update()
    {
        Debug.Log(oldPosition + " " + transform.position.x);

        Flip();
        hpSlider.maxValue = maxHp;
        hp = Mathf.Clamp(hp, 0 , maxHp);

        distance = Vector2.Distance(transform.position, playerObject.transform.position);
        Vector2 direction = playerObject.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < enemyAgroRange)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, playerObject.transform.position, speed * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
    }

    private void Flip()
    {
        if (oldPosition < transform.position.x)
        {
            oldPosition = transform.position.x;
            sr.flipX = false;
        }
        
        else if (oldPosition > transform.position.x)
        {
            oldPosition = transform.position.x;
            sr.flipX = true;
        }
    }
}
