using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int _degats = 2;

    private void Update()
    {
        if (!(transform.position.y >= GameManager.CLAMPYLOW && transform.position.y <= GameManager.CLAMPYHIGH)) 
        {
            Destroy(gameObject);
        }
        if (!(transform.position.x >= GameManager.CLAMPXLOW && transform.position.x <= GameManager.CLAMPXHIGH)) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            Player.Instance.DegatsRecusJoueur(_degats);
        }
    }
}
