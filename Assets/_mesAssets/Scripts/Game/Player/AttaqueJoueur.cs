using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttaqueJoueur : MonoBehaviour
{
    [SerializeField] private int[] listAttaque;
    [SerializeField] private GameObject[] listPrefabAttack;

    [SerializeField] private GameObject _rangeCircle = default;
    private GameObject _container = default;
    private List<GameObject> _enemies;
    private Collider2D _collider;    
    private Vector3? _closestEnemy;
    //[SerializeField] private float AttackType2Rotation = 100f;

    private bool _playerDead = false;
    public bool PlayerDead
    {
        set { _playerDead = value; }
    }
    Vector3 pivotPoint;

    public static AttaqueJoueur Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _container = GameObject.FindGameObjectWithTag("Container");
        RangeSet(Player.Instance.RangeAttack);
        _collider = _rangeCircle.GetComponent<Collider2D>();
        _enemies = new List<GameObject>();
        _closestEnemy = null;
        StartCoroutine(DefaultAttackType());  
    }

    private void Update()
    {
        pivotPoint = Player.Instance.transform.position + new Vector3(0, 1, 0);
    }
    
    public void RangeSet(float _range)
    {
        float x = _range;
        float y = _range;
        float z = _range;

        _rangeCircle.transform.localScale = new Vector3(x, y, z);
    }

    public void RangeUpgraded(float _increase)
    {
        float x = _increase;
        float y = _increase;
        float z = _increase;

        _rangeCircle.transform.localScale = new Vector3(x,y,z);
    }

    /* Code à implémenter dans le futur :)
    private void ChangeAttackType(int typeAttack) 
    {
        StopAllCoroutines();
        switch (typeAttack)
        {
            case 0: // RangedAttackType1
                StartCoroutine(DefaultAttackType());
                break; 
            case 1: // AttackType2
                StartCoroutine(AttackType2());
                break; 
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
    }*/
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {            
            _enemies.Add(collision.gameObject); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {        
        if(collision.gameObject.CompareTag("Enemy"))
        {            
            _enemies.Remove(collision.gameObject);
            _enemies.RemoveAll(item => item == null);
        }
    }

    public void DestroyedEnemy(GameObject _destroyed)
    {
        if (_enemies.Contains(_destroyed))
        {
            _enemies.Remove(_destroyed);
            _enemies.RemoveAll(item => item == null);
        }                 
    }
   

    private void TrackEnemies()
    {
        _enemies.RemoveAll(item => item == null);
        try
        {
            if( _enemies.Count > 0 && _enemies[0])
            {
                float distance = 9999f;
                foreach (GameObject _position in _enemies)
                {
                    if(_position != null)
                    {
                        float thisDistance = Vector3.Distance(Player.Instance.PositionPlayer, _position.transform.position);

                        if (thisDistance < distance)
                        {
                            distance = thisDistance;
                            _closestEnemy = _position.transform.position;
                        }
                    }
                    else
                    {                        
                        _closestEnemy = null; break;
                    }                   
                }
            }
            else
            {                
                _closestEnemy = null;
            }            
        }
        catch(Exception) 
        {
            _closestEnemy = null;
        }   
        
    }

    IEnumerator DefaultAttackType()
    {
        while (!_playerDead)
        {            
            TrackEnemies();            
            Vector3 _enemy;
            if(_closestEnemy != null)
            {
                _enemy = (Vector3)_closestEnemy;
                _enemy = new Vector3(_enemy.x, _enemy.y-1f, _enemy.z);

                Vector3 direction = (_enemy - Player.Instance.PositionPlayer).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                GameObject attack = Instantiate(listPrefabAttack[0], transform.position + new Vector3(0f, 1f, 0f), Quaternion.Euler(0, 0, angle));
                attack.transform.SetParent(_container.transform);
                attack.GetComponent<Rigidbody2D>().velocity = direction * Player.Instance.VitesseAttack;

                yield return new WaitForSeconds(Player.Instance.CadenceTir);
            }
            else
            {
                yield return new WaitForSeconds(Player.Instance.CadenceTir);
            }                      
        }           
    }
    /*
    IEnumerator AttackType2()
    {
        listPrefabAttack[1].SetActive(true);
        while (!_playerDead)
        {
            listPrefabAttack[1].transform.RotateAround(pivotPoint, Vector3.forward, AttackType2Rotation * Time.deltaTime);
            yield return null;
        }

    }*/
}
