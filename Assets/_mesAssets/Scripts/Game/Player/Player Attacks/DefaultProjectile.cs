
using UnityEngine;

public class DefaultProjectile : MonoBehaviour
{
    private float _scale;
    public float _degat;
    void Start()
    {
        _scale = Player.Instance.ScaleAttack;
        _degat = Player.Instance.DegatsJoueur;
        transform.localScale = new Vector3(_scale, _scale, _scale);
    }


    void Update()
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
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Boss boss = collision.gameObject.GetComponent<Boss>();
            Boss2 boss2 = collision.gameObject.GetComponent<Boss2>();
            if (Random.Range(1f, 100f) <= Player.Instance.CritChance)
            {
                if (enemy != null)
                {
                    enemy.DegatsOnEnemy(Player.Instance.DegatsJoueur * (1.5f + Player.Instance.CritBonus));
                }
                else if (boss != null)
                {
                    boss.DegatsOnEnemy(Player.Instance.DegatsJoueur * (1.5f + Player.Instance.CritBonus));             
                }
                else if (boss2 != null)
                {
                    boss2.DegatsOnEnemy(Player.Instance.DegatsJoueur * (1.5f + Player.Instance.CritBonus));             
                }
            }
            else
            {
                if (enemy != null)
                {
                    enemy.DegatsOnEnemy(Player.Instance.DegatsJoueur);
                }
                else if (boss != null)
                {
                    boss.DegatsOnEnemy(Player.Instance.DegatsJoueur);
                }
                else if (boss2 != null)
                {
                    boss2.DegatsOnEnemy(Player.Instance.DegatsJoueur);
                }
            } 
            Destroy(gameObject);
        }
     
    }
}

