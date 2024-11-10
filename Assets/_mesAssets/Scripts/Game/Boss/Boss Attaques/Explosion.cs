using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _explosionDuration = 2f;

    private bool _damageDone = false;

    public void Start()
    {
        StartCoroutine(DureeDeVie());
    }   

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !_damageDone)
        {
            Player.Instance.DegatsRecusJoueur(_damage);
            _damageDone = true;
        }
    }

    IEnumerator DureeDeVie()
    {
        yield return new WaitForSeconds(_explosionDuration);
        Destroy(gameObject);
    }
}
