using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _spellRamasse = default;
    [SerializeField] private Image[] _spellImage = default;
    [SerializeField] private Slider _sliderVie = default;
    [SerializeField] private Slider _sliderXp = default;
    [SerializeField] private TextMeshProUGUI _txtVies = default;
    [SerializeField] private TextMeshProUGUI _txtXp = default;
    [SerializeField] private TextMeshProUGUI _txtLvl = default;
    [SerializeField] private TextMeshProUGUI _txtPoints = default;
    [SerializeField] private TextMeshProUGUI _txtTemps = default;
    [SerializeField] private Slider _sliderSpells = default;
    [SerializeField] private TextMeshProUGUI _sliderSpellTxt = default;

    [Header("LevelUpPanel")]
    [SerializeField] private GameObject _panelLevelUp = default;
    [SerializeField] private GameObject _backgroundPowerUp = default;
    private List<GameObject> _prefabList = new();
    private List<GameObject> _randomChoices = new();
    private List<int> lastRandom = new();
    private int randomPrefab;
    [SerializeField] private Slider _sliderPowerUp = default;
    [SerializeField] private float _sliderTimerPUs = 5f;
    [SerializeField] private TextMeshProUGUI _sliderTxt = default;
    [SerializeField] private GameObject[] _selectedChoiceImgs = default;

    [Header("StatsPanel")]
    [SerializeField] private GameObject _panelStats = default;
    [SerializeField] private TextMeshProUGUI _txtVitesse = default;
    [SerializeField] private TextMeshProUGUI _txtCadenceAttaque = default;
    [SerializeField] private TextMeshProUGUI _txtChance = default;
    [SerializeField] private TextMeshProUGUI _txtCritBonus = default;
    [SerializeField] private TextMeshProUGUI _txtCritChance = default;
    [SerializeField] private TextMeshProUGUI _txtDegats = default;
    [SerializeField] private TextMeshProUGUI _txtRange = default;
    [SerializeField] private TextMeshProUGUI _txtVitesseAttaque = default;
    [SerializeField] private TextMeshProUGUI _txtScaleAttaque = default;

    private float _tempsDebut;
    private bool _spellEstRamasse = false;
    private int _numSpell = -1;
    private Player _player;
    public int NumSpell => _numSpell;

    private bool _lockScore = false;

    public bool LockScore
    {
        set { _lockScore = value; }
        get { return _lockScore; }
    }

    private void Start()
    {
        _tempsDebut = Time.time;
        foreach (Transform child in _panelLevelUp.transform)
        {
            _prefabList.Add(child.gameObject);
        }

    }
    void Update()
    {
        if (!_lockScore)
        {
            GestionScore();
            GestionTemps();
        }
    }
    
    public void SetVieMaximum(int vie)
    {
        _sliderVie.maxValue = vie;
    }
    public void GestionBarreDeVie(int vie)
    {
        _sliderVie.value = vie;
        _txtVies.text = vie.ToString() + "/100";
    }

    public void SetXpMaximum(int xp)
    {
        _sliderXp.maxValue = xp;
    }
    public void GestionBarreXp(int xp)
    {
        _sliderXp.value = xp;
        float tempValue = _sliderXp.value / _sliderXp.maxValue * 100;
        tempValue = Mathf.Round(tempValue);
        _txtXp.text = tempValue.ToString() + "%";
    }

    public void GestionLevel()
    {
        _txtLvl.text = "LVL " + Player.Instance.Levels;
    }

    public void GestionTemps()
    {
        float _minutes = Mathf.FloorToInt((Time.time - _tempsDebut) / 60f);
        float _seconds = Mathf.FloorToInt((Time.time - _tempsDebut) % 60f);
        string _timeText = string.Format("{0:00}:{1:00}", _minutes, _seconds);

        _txtTemps.text = "Temps " + _timeText;
    }
    public void GestionScore()
    {
        _txtPoints.text = "Pointage: " + GameManager.Instance.Score;
    }

    public void ActiverImageUISpell(int noSpell)
    {
        _spellRamasse = _spellImage[noSpell];
        _spellRamasse.gameObject.SetActive(true);
        _spellEstRamasse = true;
        _numSpell = noSpell;
    }
    IEnumerator GestionSliderSpell()
    {
        _sliderSpells.gameObject.SetActive(true);
        _sliderSpells.maxValue = 8;
        _sliderSpells.value = 8;
        float elapsedTime = 0f;

        while (_sliderSpells.value > 0)
        {
            elapsedTime += Time.deltaTime;
            _sliderSpells.value = Mathf.Max(0, 8 - elapsedTime);
            _sliderSpellTxt.text = string.Format("{0:N0}", _sliderSpells.value);
            yield return null;
        }

        _sliderSpells.gameObject.SetActive(false);
    }

    public void StartCoroutineSliderSpell()
    {
        StartCoroutine(GestionSliderSpell());
    }
    public void DesactiverImageUISpell()
    {
        _spellRamasse.gameObject.SetActive(false);
        _spellEstRamasse = false;
    }

    public bool isSpellRamasse()
    {
        return _spellEstRamasse;
    }

    IEnumerator UpdateStats()
    {
        _panelStats.SetActive(true);
        _txtVitesse.text = "Mobilité : " + Math.Round(Player.Instance.VitesseJoueur,1).ToString();
        _txtChance.text = "Chance : " + Math.Round(Player.Instance.ChanceJoueur,1).ToString();
        _txtCritBonus.text = "Létalité : " + Math.Round(Player.Instance.CritBonus).ToString();
        _txtCritChance.text = "Précision : " + Math.Round(Player.Instance.CritChance,1).ToString();
        _txtDegats.text = "Dégâts : " + Math.Round(Player.Instance.DegatsJoueur).ToString();
        _txtRange.text = "Portée : " + Math.Round(Player.Instance.RangeAttack,1).ToString();
        _txtVitesseAttaque.text = "Inertie : " + Math.Round(Player.Instance.VitesseAttack,1).ToString();
        _txtCadenceAttaque.text = "Cadence : " + Math.Round(Player.Instance.CadenceTir).ToString();
        _txtScaleAttaque.text = "Potence : " + Math.Round(Player.Instance.ScaleAttack,1).ToString();
        yield return new WaitForSeconds(5f);
        _panelStats.SetActive(false);
    }

    public void LevelUp()
    {
        if (lastRandom != null)
            lastRandom.Clear();
        if (_randomChoices != null)
            _randomChoices.Clear();
        _panelLevelUp.SetActive(true);
        _backgroundPowerUp.SetActive(true);
        Time.timeScale = 0f;
        for (int i = 0; i < 3; i++)
        {
            do
            {
                randomPrefab = UnityEngine.Random.Range(0, _prefabList.Count);
            }
            while (lastRandom.Contains(randomPrefab));
            lastRandom.Add(randomPrefab);
            _randomChoices.Add(_prefabList[randomPrefab]);
            _randomChoices[i].SetActive(true);
        }
        ReclassingElements();
        AudioManager.Instance.SoundLevelUP();
        StartCoroutine(StartTimer());
    }

    private void ReclassingElements()
    {
        List<GameObject> _listTemp = new List<GameObject>();
        foreach (GameObject i in _prefabList)
        {
            if (_randomChoices.Contains(i))
            {
                _listTemp.Add(i);
            }
        }
        _randomChoices = _listTemp;
    }

    public void ChoixAleatoire()
    {
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }

    public void ChoixFini()
    {
        EventSystem.current.SetSelectedGameObject(null);
        _panelLevelUp.SetActive(false);
        StartCoroutine(UpdateStats());
        _backgroundPowerUp.SetActive(false);
        _prefabList.ForEach(prefab => prefab.SetActive(false));
        _sliderPowerUp.gameObject.SetActive(false);
        foreach (GameObject img in _selectedChoiceImgs)
        {
            img.SetActive(false);
        }
        foreach (Transform child in _panelLevelUp.transform)
        {
            if (child.gameObject.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(child.gameObject);
                break;
            }
        }
        Time.timeScale = 1f;
    }
    public void ChoixDegats()
    {
        Player.Instance.PowerUpDegats();
        ChoixFini();
    }
    public void ChoixMobilite()
    {
        Player.Instance.PowerUpSpeed(0);
        ChoixFini();
    }
    public void ChoixPortee()
    {
        Player.Instance.AddRangeAttack();
        ChoixFini();
    }
    public void ChoixInertie()
    {
        Player.Instance.AddSpeedAttack();
        ChoixFini();
    }
    public void ChoixLetalite()
    {
        Player.Instance.AddCritBonus();
        ChoixFini();
    }
    public void ChoixPrecision()
    {
        Player.Instance.AddPourcentageCrit();
        ChoixFini();
    }
    public void ChoixCadence()
    {
        Player.Instance.AddCadenceTir();
        ChoixFini();
    }
    public void ChoixChance()
    {
        Player.Instance.AddChance();
        ChoixFini();
    }
    public void ChoixPotence()
    {
        Player.Instance.AddScaleRangeAttack();
        ChoixFini();
    }

    private void SelectedChoice()
    {        
        if (EventSystem.current.currentSelectedGameObject == _randomChoices[0])
        {
            _selectedChoiceImgs[0].SetActive(true);
            _selectedChoiceImgs[1].SetActive(false);
            _selectedChoiceImgs[2].SetActive(false);
        }
        else if (EventSystem.current.currentSelectedGameObject == _randomChoices[1])
        {
            _selectedChoiceImgs[0].SetActive(false);
            _selectedChoiceImgs[1].SetActive(true);
            _selectedChoiceImgs[2].SetActive(false);
        }
        else if (EventSystem.current.currentSelectedGameObject == _randomChoices[2])
        {
            _selectedChoiceImgs[0].SetActive(false);
            _selectedChoiceImgs[1].SetActive(false);
            _selectedChoiceImgs[2].SetActive(true);
        }

    }

    private bool ChoiceMade()
    {
        if (Input.GetAxis("Fire1") != 0)
            return true;
        else
            return false;
    }

    IEnumerator StartTimer()
    {        

        for (int i = 0; i < _randomChoices.Count; i++)
        {
            int _left = 0;
            if (i == 0)
                _left = 2;
            else
                _left = i - 1;

            int _right = 0;
            if (i == 2)
                _right = 0;
            else
                _right = i + 1;

            Navigation nav = new Navigation()
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = _randomChoices[_left].GetComponent<Button>(),
                selectOnRight = _randomChoices[_right].GetComponent<Button>()
            };
            _randomChoices[i].GetComponent<Button>().navigation = nav;
        }

        _sliderPowerUp.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_randomChoices[0]);

        float time = 0f;
        bool choice = false;
        if (Player.Instance.Levels > 4)
        {
            _sliderPowerUp.maxValue = _sliderTimerPUs;
            _sliderPowerUp.value = _sliderTimerPUs;
            while (time < _sliderTimerPUs && !choice)
            {
                SelectedChoice();
                time += Time.unscaledDeltaTime;
                _sliderPowerUp.value = _sliderTimerPUs - time;
                _sliderTxt.text = string.Format("{0:N0}", _sliderTimerPUs - time);
                choice = ChoiceMade();
                yield return null;
            }
        }
        else
        {
            float _earlyLvlTimer = 10f;
            _sliderPowerUp.maxValue = 10f;
            _sliderPowerUp.value = 10f;
            while (time < 10f && !choice)
            {
                SelectedChoice();
                time += Time.unscaledDeltaTime;
                _sliderPowerUp.value = _earlyLvlTimer - time;
                _sliderTxt.text = string.Format("{0:N0}", _earlyLvlTimer - time);
                choice = ChoiceMade();
                yield return null;
            }
        }


        if (choice)
            ChoixFini();
        else
            ChoixAleatoire();
        yield return null;
    }
}
