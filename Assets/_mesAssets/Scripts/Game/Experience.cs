using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.Instance.AddXp(Random.Range(2,5));
            AudioManager.Instance.SoundXP();
            Destroy(gameObject);
        }
    }
}
