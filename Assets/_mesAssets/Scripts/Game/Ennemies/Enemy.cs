using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("Settings G�n�ral")]
    [SerializeField] private float _vitesseEnemy = 5f;
    [SerializeField] private float _enemyStyle = 0f;
    [SerializeField] private bool isRanged = false;
    [SerializeField] private GameObject _prefabXp = default;
    [SerializeField] private float vies = 100f;
    [SerializeField] private float freezeDuree = 5f;
    private bool _spellCryoActif = false;
    private GameObject _iceFreeze;
    [Header("Settings Ennemi Type 2")]
    [SerializeField] private float _stopDistance = 4f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashChargingTime = 2f;
    private bool isStopped = false;
    private Vector3 dashTargetPosition;
    [Header("Settings Ennemi Type 3")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject attackPrefab = default;
    [SerializeField] private float attackSpeed = 5f;
    [SerializeField] private GameObject _container = default;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _container = GameObject.FindGameObjectWithTag("Container");
        if (isRanged)
        {
            StartCoroutine(FireCoroutine());
        }
        _iceFreeze = transform.GetChild(0).gameObject;
        if(SpawnManager.Instance.DifficultyLevel > 3) 
        { 
            for(int i = SpawnManager.Instance.DifficultyLevel-3; i > 0; i--)
            {
                float value = UnityEngine.Random.Range(0.04f, 0.06f);
                vies *= value + 1;
            }
        }        
    }
    void Update()
    {
        if (vies > 0)
        {
            MouvementEnemy();
        }
        else
        {
            DestroyEnemy();
        }
    }

    private void MouvementEnemy()
    {
        var vitesse = _vitesseEnemy * Time.deltaTime;
        switch (_enemyStyle)
        {
            case 1:
                if (!_spellCryoActif)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Player.Instance.PositionPlayer, vitesse);
                    FacingDirection();
                }
                break;
            case 2:
                if (!_spellCryoActif) 
                { 
                    Vector3 distancePlayer = Player.Instance.PositionPlayer - transform.position;
                    if (!isStopped)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, Player.Instance.PositionPlayer, vitesse);
                        FacingDirection();
                        if (distancePlayer.magnitude <= _stopDistance)
                        {
                            dashTargetPosition = Player.Instance.PositionPlayer;
                            isStopped = true;
                            StartCoroutine(StartDash());
                        }
                    }
                }
                
                break;
            case 3:
                // Aucun mouvement disponible pour ce type d'ennemi
                FacingDirection();
                break;

        }
    }

    private void FacingDirection()
    {
        if (Player.Instance.PositionPlayer.x > transform.position.x)
        {
            if (transform.localScale.x != 3.5f)
                transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        }
        else if (Player.Instance.PositionPlayer.x < transform.position.x)
        {
            if (transform.localScale.x != -3.5f)
                transform.localScale = new Vector3(-3.5f, 3.5f, 3.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.Instance.DegatsRecusJoueur(5);
            DestroyEnemy();
        }
        else if (collision.gameObject.CompareTag("Cryo"))
        {
            StartCoroutine(FreezeCoroutine());
        }
    }

    private void DestroyEnemy()
    {
        AttaqueJoueur.Instance.DestroyedEnemy(gameObject);        
        SpawnManager.Instance.EnnemieDetruit();
        GameManager.Instance.UpdateScore(2);
        for (int i = 0; i <= UnityEngine.Random.Range(1, 3); i++)
        {
            GameObject exp = Instantiate(_prefabXp, transform.position, Quaternion.identity);
            exp.transform.SetParent(transform.parent);
        }
        if (Player.Instance.GetLootChance(1f))
        {
            Player.Instance.PowerUpSpeed(1);
        }
        if (Player.Instance.GetLootChance(1f))
        {
            Player.Instance.PowerUpDegats();
        }
        Destroy(gameObject);
    }
    public void DegatsOnEnemy(float degats)
    {
        vies -= degats;
    }

    IEnumerator FireCoroutine()
    {
        _animator.SetBool("Idle", true);
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (!_spellCryoActif)
            {
                _animator.SetBool("Idle", false);                
                yield return new WaitForSeconds(attackCooldown);
                _animator.SetBool("Idle", true);
            }
            else 
            {
                yield return new WaitForSeconds(attackCooldown);
            }
        }
    }

    //Appele par animation event de l'enemy 3
    private void Fire()
    {
        Vector3 direction = (Player.Instance.PositionPlayer - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject attack = Instantiate(attackPrefab, transform.position + new Vector3(0f,1f,0f), Quaternion.Euler(0, 0, angle));
        attack.transform.SetParent(_container.transform);
        attack.GetComponent<Rigidbody2D>().velocity = direction * attackSpeed;
    }

    IEnumerator StartDash()
    {
        _animator.SetBool("Idle", false);
        _animator.SetBool("WaitingDash", true);
        yield return new WaitForSeconds(dashChargingTime);

        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        _animator.SetBool("WaitingDash", false);
        while (Time.time - startTime < dashChargingTime && !_spellCryoActif)
        {
            float dashProgress = (Time.time - startTime) / dashChargingTime;
            transform.position = Vector3.Lerp(startPosition, dashTargetPosition, dashProgress * dashSpeed);                
            yield return null;
        }
        isStopped = false;
        _animator.SetBool("Idle", true);              
    }

    IEnumerator FreezeCoroutine()
    {
        _spellCryoActif = true;
        _iceFreeze.SetActive(true);
        _animator.SetBool("Idle", true);
        yield return new WaitForSeconds(freezeDuree);
        _spellCryoActif = false;
        _iceFreeze.SetActive(false);
        _animator.SetBool("Idle", false);
    }
}
