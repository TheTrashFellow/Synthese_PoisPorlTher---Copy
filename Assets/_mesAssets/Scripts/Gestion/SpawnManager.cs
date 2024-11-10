using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    
    [SerializeField] private GameObject _spawnerContainer = default;
    

    [Header("Ennemies")]    
    [SerializeField] private float _timeDifficultyUp = 30f;
    //[SerializeField] private float[] _spawnDelai = default;
    [SerializeField] private float _spawnDelai = default;
    [SerializeField] private GameObject[] _enemyPrefabs = default;

    [Header("Boss")]
    [SerializeField] private float _timeBoss = 30f;
    [SerializeField] private GameObject[] _bossesPrefabs = default;

    [Header("Spells")]
    [SerializeField] private GameObject[] _spellPrefabs = default;

    private int _spawnMax = 3;
    private int _spawnActuel = 0;
    private int _countDifficulty = 0;

    private IEnumerator apparitionEnnemiesRoutine;
    private GameObject newBoss;

    private int _countBoss;
    private float _tempsDebut;

    public int DifficultyLevel
    {
        get { return _countDifficulty; }
    }

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

    private void Start()
    {
        _tempsDebut = Time.time;
        _countBoss = 0;
        StartCoroutine(ApparitionEnnemies());        
        StartCoroutine(ApparitionsSpells());
    }

    private void Update()
    {
        if (_countBoss < _timeBoss)
        {
            if ((Time.time - _tempsDebut) >= _timeBoss)
            {                
                SpawnBoss();
            }
        }
        
        if (_countDifficulty < _timeDifficultyUp)
        {
            if ((Time.time - _tempsDebut) >= _timeDifficultyUp)
            {                
                DifficultyUp();
            }
        }
        
    }
    public void EnnemieDetruit()
    {
        _spawnActuel--;
    }    

    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(ApparitionEnnemies());
    }

    IEnumerator ApparitionEnnemies()
    {
        yield return new WaitForSeconds(2f);
        while (Player.Instance.NbrVies >= 1)
        {   if (_spawnActuel < _spawnMax)
            {
                _spawnActuel++;
                int randomEnemy = Random.Range(0, (_enemyPrefabs.Length));
                Vector3 positionSpawn = new Vector3(Random.Range(-17f, 18f), Random.Range(-8f, 7f), 0f);
                GameObject newGameObject = Instantiate(_enemyPrefabs[randomEnemy], positionSpawn, Quaternion.identity);
                newGameObject.transform.parent = _spawnerContainer.transform;

                //Ajoute temps au nom pour les discerner les un des autres
                newGameObject.name = newGameObject.name + Time.time.ToString();
                yield return new WaitForSeconds(_spawnDelai);
            }
            else
            {
                yield return new WaitForSeconds(_spawnDelai);
            }                                 
        }                 
    }    

    IEnumerator ApparitionsSpells()
    {
        yield return new WaitForSeconds(10f);
        while (Player.Instance.NbrVies >= 1)
        {
            Vector3 positionSpell = new Vector3(Random.Range(-15f, 15f), Random.Range(-8f, 4.5f), 0f);
            int randomSpell = Random.Range(0, _spellPrefabs.Length);
            GameObject newSpell = Instantiate(_spellPrefabs[randomSpell], positionSpell, Quaternion.identity);
            newSpell.transform.parent = _spawnerContainer.transform;
            if (Player.Instance.GetLootChance(5))
            {
                yield return new WaitForSeconds(Random.Range(5f, 10f));
            }
            else if (Player.Instance.GetLootChance(10))
            {
                yield return new WaitForSeconds(Random.Range(15f, 20f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(25f, 30f));
            }
        }
    }

    public void DifficultyUp()
    {       

        switch (_countDifficulty)
        {
            case 0:
                _spawnMax += 1;
                _timeDifficultyUp += 30f;
                break;
            case 1:
                _spawnMax += 1;
                _timeDifficultyUp += 30f;
                break;
            case 2:
                _spawnMax += 1;
                _timeDifficultyUp += 30f;
                break;
            case 3:
                _spawnMax += 1;
                _timeDifficultyUp += 45f;
                break;
            case 4:
                _spawnMax += 1;
                _timeDifficultyUp += 45f;
                break;
            default:
                _timeDifficultyUp += 45f;
                _spawnDelai *= 0.99f;
                break;
        }
        if (_timeDifficultyUp > 360)
        {
            _spawnDelai *= 0.99f;
            _countDifficulty++;
        }
        _countDifficulty++;        
    }

    public void SpawnBoss()
    {
        int randomEnemy = Random.Range(0, (_bossesPrefabs.Length));
        Vector3 positionSpawn = new Vector3(Random.Range(-17f, 18f), Random.Range(-8f, 7f), 0f);
        if (_timeBoss >= 300)
        {
            for (int i = 0; i < 2; i++)
            {
                newBoss = Instantiate(_bossesPrefabs[randomEnemy], positionSpawn, Quaternion.identity);
            }
        }
        else 
        {
        newBoss = Instantiate(_bossesPrefabs[randomEnemy], positionSpawn, Quaternion.identity);
        }
        newBoss.transform.parent = _spawnerContainer.transform;
        _timeBoss += 60f;
        _countBoss++;
        AudioManager.Instance.MusicBoss();
    }
}
