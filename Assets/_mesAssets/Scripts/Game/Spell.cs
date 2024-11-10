using System.Collections;
using UnityEngine;


public class Spell : MonoBehaviour
{
    [SerializeField] private int _spellID = default;  // 0=Attack   1=Cryo    2=Shield
    private Player _player;
    private UIManager _uiManager;

    private void Start()
    {
        StartCoroutine(DestroySpell());
        _uiManager = FindObjectOfType<UIManager>().GetComponent<UIManager>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _uiManager.ActiverImageUISpell(_spellID);
            Player.Instance.SetSpell(_spellID);
            GameManager.Instance.UpdateScore(1);
            Destroy(gameObject);
        }
    }
    
    IEnumerator DestroySpell()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    
}

