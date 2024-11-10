
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Propri�t�s du joueur")]
    [SerializeField] private float _vitesseJoueur = 10f;
    [SerializeField] private int _nbrVies = 100;
    [SerializeField] private float _DegatsJoueur = 1f;
    [SerializeField] private float _chanceJoueur = 1f;
    [SerializeField] private float _pourcentageCrit = 0f;
    [SerializeField] private float _critBonus = 0f;
    [SerializeField] private float _vitesseAttack = 1f;
    [SerializeField] private float _scaleAttack = 1f;
    [SerializeField] private float _rangeAttack = 1f;
    [SerializeField] private float _cadenceTir = 1f;
    [SerializeField] private int _XpValueLevelUp = 30;
    
    private int _levels;
    private int _XpValue;

    
    public int Levels => _levels;
    public int NbrVies => _nbrVies;
    public float DegatsJoueur => _DegatsJoueur;
    public float ChanceJoueur => _chanceJoueur;
    public float CritChance => _pourcentageCrit;
    public float CritBonus => _critBonus;
    public float VitesseAttack => _vitesseAttack;
    public float ScaleAttack => _scaleAttack;
    public float RangeAttack => _rangeAttack;
    public float VitesseJoueur => _vitesseJoueur;

    public float CadenceTir => _cadenceTir;


    private UIManager _uiManager;    
    private Animator _animator;

    private int _spellID = -1;
    private int _spellIDTransition = -1;
    private GameObject _shield;
    private GameObject _cryo;
    private float _tempDegat;
    private bool _spellActiv = false;


    public Vector3 PositionPlayer => transform.position;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _uiManager = FindObjectOfType<UIManager>().GetComponent<UIManager>();            
            _shield = transform.GetChild(0).gameObject;
            _cryo = transform.GetChild(1).gameObject;
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Start()
    {
        _levels = 1;
        _XpValue = 0;
        _uiManager.SetVieMaximum(_nbrVies);
        _uiManager.SetXpMaximum(_XpValueLevelUp);

        _animator = GetComponent<Animator>();
        if (PlayerPrefs.HasKey("compteur"))
        {
            int compteur = PlayerPrefs.GetInt("compteur");
            compteur += 1;
            PlayerPrefs.SetInt("compteur", compteur);
        }
        else
        {
            PlayerPrefs.SetInt("compteur", 1);
        }
    }

    void Update()
    {
        if (NbrVies > 0)
        {
            MouvementJoueur();
            CastingAnimation();
        }
        else if (NbrVies <= 0)
        {
            PlayerPrefs.SetInt("Score", GameManager.Instance.Score);
            PlayerPrefs.Save();
        }

    }


    private void MouvementJoueur()
    {
        float mouvementX = Input.GetAxis("Horizontal");
        float mouvementY = Input.GetAxis("Vertical");
        Vector2 direction = new(mouvementX, mouvementY);
        direction.Normalize();

        transform.Translate(direction * Time.deltaTime * _vitesseJoueur);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, GameManager.CLAMPXLOW, GameManager.CLAMPXHIGH), Mathf.Clamp(transform.position.y, GameManager.CLAMPYLOW, GameManager.CLAMPYHIGH), 0f);

        if (direction.x < 0f)
        {
            _animator.SetBool("Moving", true);
            if (transform.localScale.x == 4.5f)
            {
                transform.localScale = new Vector3(-4.5f, 4.5f, 4.5f);
            }
        }
        else if (direction.x > 0f)
        {
            _animator.SetBool("Moving", true);
            if (transform.localScale.x == -4.5f)
            {
                transform.localScale = new Vector3(4.5f, 4.5f, 4.5f);
            }
        }
        else if (direction.y != 0f)
        {
            _animator.SetBool("Moving", true);
        }
        else
        {
            _animator.SetBool("Moving", false);
        }
    }

    public void SetSpell(int spellID)
    {
        _spellID = spellID;
    }

    public void CastingAnimation()
    {
        if (Input.GetAxis("Fire2") != 0 && !_spellActiv && _spellID != -1)
        {
            _uiManager.DesactiverImageUISpell();
            _spellActiv = true;
            _spellIDTransition = _spellID;
            _animator.SetTrigger("Casting");
        }
    }

    public void CallGestionSliderSpell()
    {
        _uiManager.StartCoroutineSliderSpell();
    }

    public void CastingSpell()
    {
        switch (_spellIDTransition)
        {
            case 0:
                AttackSpell();
                break;
            case 1:
                CryoSpell();
                break;
            case 2:
                ShieldSpell();
                break;
        }
    }


    private void DeathPlayer()
    {
        //Animation event call la methode de EndScene du game manager
        _uiManager.LockScore = true;
        AttaqueJoueur.Instance.PlayerDead = true;

        GetComponent<PolygonCollider2D>().enabled = false;

        _animator.SetTrigger("Death");
    }

    public void EndScene()
    {
        int noScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(noScene + 1);
    }

    public void AddXp(int value)
    {
        _XpValue += value;
        if (_XpValue >= _XpValueLevelUp)
        {
            _XpValue -= _XpValueLevelUp;
            if (_XpValueLevelUp < 120)
            {
                _XpValueLevelUp += 5;
            }
            _levels += 1;
            _uiManager.SetXpMaximum(_XpValueLevelUp);
            _uiManager.GestionLevel();
            GameManager.Instance.UpdateScore(0);
            _uiManager.LevelUp();
        }
        _uiManager.GestionBarreXp(_XpValue);
    }

    public bool GetLootChance(float dropChance)
    {
        float _roll = Random.Range(0f, 1f);
        
        if ( _roll <= (dropChance / 100) * _chanceJoueur)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    

    public void AttackSpell()
    {        
        _tempDegat = _DegatsJoueur;
        _DegatsJoueur *= 2f;
        StartCoroutine(DureeSpell());
    }

    public void CryoSpell()
    {
        _tempDegat = _DegatsJoueur;        
        _cryo.SetActive(true);
        StartCoroutine(DureeSpell());
    }

    public void ShieldSpell()
    {
        _spellActiv = true;
        _tempDegat = _DegatsJoueur;
        _shield.SetActive(true);
        StartCoroutine(DureeSpell());
    }

    public IEnumerator DureeSpell()
    {
        _spellID = -1;
        yield return new WaitForSeconds(8f);
        _cryo.SetActive(false);
        _shield.SetActive(false);
        _DegatsJoueur = _tempDegat;
        _spellActiv = false;
    }
   
    public void DegatsRecusJoueur(int damage)
    {
        if (!_shield.activeSelf)
        {
            _nbrVies -= damage;
            _uiManager.GestionBarreDeVie(_nbrVies);
        }

        if (_nbrVies < 1)
        {
            DeathPlayer();
        }
    }

    
    public void PowerUpSpeed(int entry)
    {
        if(entry == 1)
        {
            _vitesseJoueur *= 1.03f;
        }
        else
        {
            float value = Random.Range(0.05f, 0.10f);
            _vitesseJoueur *= value + 1;
        }        
    }

    public void PowerUpDegats()
    {
        float value = Random.Range(0.03f, 0.08f);
        if (!_spellActiv)
        {
            _DegatsJoueur *= value + 1;
        }
        else
        {            
            _tempDegat *= value + 1;
        }
    }
    public void AddChance()
    {
        float value = Random.Range(1f, 2f);
        _chanceJoueur += value;
    }
    public void AddCadenceTir()
    {
        float value = Random.Range(-0.05f, -0.10f);
        _cadenceTir *= value + 1;
    }
    public void AddRangeAttack()
    {
        float value = Random.Range(0.1f, 0.15f);
        _rangeAttack *= value + 1;
        AttaqueJoueur.Instance.RangeUpgraded(_rangeAttack);
    }
    public void AddCritBonus()
    {
        float value = Random.Range(2f, 8f);
        _critBonus += value;
    }
    public void AddPourcentageCrit()
    {
        if(_pourcentageCrit == 0)
        {
            float value = Random.Range(5f, 10f);
            _pourcentageCrit += value;
        }
        else
        {
            float value = Random.Range(0.10f, 0.15f);
            _pourcentageCrit *= value + 1;
        }
    }
    public void AddScaleRangeAttack()
    {
        float value = Random.Range(0.2f, 0.4f);
        _scaleAttack *= value + 1;
    }
    public void AddSpeedAttack()
    {
        float value = Random.Range(0.1f, 0.2f);
        _vitesseAttack *= value + 1;
    }
}