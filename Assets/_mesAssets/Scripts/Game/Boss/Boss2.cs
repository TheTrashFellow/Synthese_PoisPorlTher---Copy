using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss2 : MonoBehaviour
{
    [Header("Settings Général")]
    
    [SerializeField] private float vies = 50f;
    [SerializeField] private float freezeDuree = 5f;
    [SerializeField] private Slider _sliderVie = default;
    [SerializeField] private GameObject _container = default;
    [SerializeField] private float attackSpeedBlast = 4f;
    [SerializeField] private GameObject _deathParticules = default;

    private Animator _animator;
    private GameObject _iceFreeze;
    private bool _spellCryoActif = false;
    private bool _isCoroutineRunning = false;
    private ParticleSystem _death;

    [Header("Settings Boss Type 2")]
    [SerializeField] private float _vitesseEnemy = 5f;
    [SerializeField] private float _attackDistance = 8f;
    [SerializeField] private float _waitAttack = 2f;
    [SerializeField] private GameObject blastPrefab = default;
    [SerializeField] private float _blastDistance = 5f;

    bool _vivant = true;

    private void Start()
    {
        SetVieMaximum(vies);
        GestionBarreVie();
        _iceFreeze = transform.GetChild(0).gameObject;
        _animator = GetComponent<Animator>();
        _container = GameObject.FindGameObjectWithTag("Container");
        _death = _deathParticules.GetComponent<ParticleSystem>();
        if (SpawnManager.Instance.DifficultyLevel > 3)
        {
            for (int i = SpawnManager.Instance.DifficultyLevel - 3; i > 0; i--)
            {
                float value = UnityEngine.Random.Range(0.04f, 0.06f);
                vies *= value + 1;
            }
        }
    }

    void Update()
    {
        if (_vivant)
        {
            MouvementBoss();
        }
    
    }

    private void MouvementBoss()
    {

        if (!_spellCryoActif && !_isCoroutineRunning)
        {
            FacingDirection();
            StartCoroutine(StartAttack());
        }
    }

    private void FacingDirection()
    {
        if (Player.Instance.PositionPlayer.x > transform.position.x)
        {
            if (transform.localScale.x != 5.5f)
                transform.localScale = new Vector3(5.5f, 5.5f, 5.5f);
        }
        else if (Player.Instance.PositionPlayer.x < transform.position.x)
        {
            if (transform.localScale.x != -5.5f)
                transform.localScale = new Vector3(-5.5f, 5.5f, 5.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.Instance.DegatsRecusJoueur(5);
            DegatsOnEnemy(20);
            GestionBarreVie();
        }
        else if (collision.gameObject.CompareTag("Cryo"))
        {
            _spellCryoActif = true;
            StartCoroutine(FreezeCoroutine());
        }
    }
    public void DegatsOnEnemy(float degats)
    {
        if (_vivant)
        {
            vies -= degats;
            GestionBarreVie();
            if (vies <= 0)
            {
                DeathBoss();
            }
        }
    }
    public void SetVieMaximum(float vie)
    {
        _sliderVie.maxValue = vie;
    }
    public void GestionBarreVie()
    {
        _sliderVie.value = vies;
    }

    IEnumerator StartAttack()
    {
        _isCoroutineRunning = true;
        var vitesse = _vitesseEnemy * Time.deltaTime;
        Vector3 distancePlayer = Player.Instance.PositionPlayer - transform.position;

        if (!_spellCryoActif && distancePlayer.magnitude <= _attackDistance)
        {
            _animator.SetBool("Idle", true);
            _animator.SetBool("Attack", false);
            yield return new WaitForSeconds(_waitAttack);

            if (!_spellCryoActif)
            {
                _animator.SetBool("Attack", true);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.Instance.PositionPlayer, vitesse);
            _animator.SetBool("Attack", false);
            _animator.SetBool("Idle", false);
        }
        _isCoroutineRunning = false;
    }

    public void CreateBlast()
    {
        Vector3 direction = (Player.Instance.PositionPlayer - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotationOriginale = blastPrefab.transform.rotation;
        Vector3 position = (transform.position + new Vector3(0f, 2f, 0f)) + direction * _blastDistance;
        GameObject blast = Instantiate(blastPrefab, position, Quaternion.Euler(0, 0, angle) * rotationOriginale);
        blast.GetComponent<Rigidbody2D>().velocity = direction * attackSpeedBlast;
        blast.transform.SetParent(_container.transform);
    }

    IEnumerator FreezeCoroutine()
    {
        if(_animator.GetBool("Death") == false)
        {
            _iceFreeze.SetActive(true);
            _animator.SetTrigger("Frozen");
            yield return new WaitForSeconds(freezeDuree);
            _spellCryoActif = false;
            _iceFreeze.SetActive(false);
        }        
    }
    private void DeathBoss()
    {
        _vivant = false;
        _death.Play();
        _iceFreeze.SetActive(false);
        AudioManager.Instance.SoundBoss2Death();
        _animator.SetTrigger("Death");
        StartCoroutine(DebugDeath());
    }

    //Call par l'animation
    private void DestroyBoss()
    {
        GameManager.Instance.UpdateScore(3);
        AudioManager.Instance.MusicDefault();
        Player.Instance.AddXp(50);
        Destroy(gameObject);
    }

    IEnumerator DebugDeath()
    {
        yield return new WaitForSeconds(5f);
        DestroyBoss();
    }
}
