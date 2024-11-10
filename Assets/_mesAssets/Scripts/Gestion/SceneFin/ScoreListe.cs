using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ScoreListe : MonoBehaviour
{

    private Transform _entryContainer;
    private Transform _entryTemplate;
    private List<Transform> _highScoreEntryTransformList;
    [SerializeField] private Button _button = default;
    [SerializeField] private GameObject _buttonRetour = default;
    [SerializeField] private TMP_Text _text = default;
    [SerializeField] private GameObject _saisieNom = default;
    [SerializeField] private GameObject _txtErreur = default;
    [SerializeField] private GameObject _lettreDepart = default;
    [SerializeField] private GameObject _classement = default;
    [SerializeField] private GameObject _textScore = default;

    private HighScores highScores;
    private string _texteTemp = "";

    private void Awake()
    {        
        GenererTableHighScore();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (highScores._highScoreEntryList.Count >= 5)
            {
                if (PlayerPrefs.GetInt("Score") > highScores._highScoreEntryList[4].score)
                {
                    _saisieNom.SetActive(true);
                    _buttonRetour.SetActive(false);
                    Button btn = _button.GetComponent<Button>();
                    btn.onClick.AddListener(EnregistrerNom);
                    StartCoroutine(DelaiSaisie());
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(_buttonRetour);
                }
            }
            else
            {
                _saisieNom.SetActive(true);
                _buttonRetour.SetActive(false);
                Button btn = _button.GetComponent<Button>();
                btn.onClick.AddListener(EnregistrerNom);
                StartCoroutine(DelaiSaisie());
            }
        }
    }

    IEnumerator DelaiSaisie()
    {
        yield return new WaitForSeconds(60f);
        Annuler();
    }

    private void Start()
    {
        if (_saisieNom != null)
        {
            if (_saisieNom.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_lettreDepart);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            StartCoroutine(RetourDebut());
        }
    }

    IEnumerator RetourDebut()
    {
        yield return new WaitForSeconds(300.0f);
        SceneManager.LoadScene(0);
    }

    public void AjouterLettre(string lettre)
    {
        if (_saisieNom.activeSelf)
        {
            if (lettre == "Espace")
            {
                _texteTemp += " ";
            }
            else if (lettre == "?")
            {
                _texteTemp = _texteTemp.Remove(_texteTemp.Length - 1);
            }
            else
            {
                if (_texteTemp.Length <= 10)
                {
                    _texteTemp += lettre;
                }
            }
            _text.text = _texteTemp;
        }
    }


    private void Update()
    {
        try
        {
            if (_saisieNom.activeSelf)
            {
                _classement.SetActive(false);
            }
            else
            {
                _classement.SetActive(true);
            }
    }
        catch (UnassignedReferenceException)
        {

        }
    }
    private void GenererTableHighScore()
    {
        _entryContainer = transform.Find("HighScoreEntryContainer");
        _entryTemplate = _entryContainer.Find("HighScoreEntryTemplate");
        _entryTemplate.gameObject.SetActive(false);
        string jsonString = PlayerPrefs.GetString("highScoreTable");
        highScores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highScores == null)
        {
            AddHighScoreEntry(100, "CEGEPTR");
        }

        for (int i = 0; i < highScores._highScoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highScores._highScoreEntryList.Count; j++)
            {
                if (highScores._highScoreEntryList[j].score > highScores._highScoreEntryList[i].score)
                {
                    HighScoreEntry tmp = highScores._highScoreEntryList[i];
                    highScores._highScoreEntryList[i] = highScores._highScoreEntryList[j];
                    highScores._highScoreEntryList[j] = tmp;
                }
            }
        }
        _highScoreEntryTransformList = new List<Transform>();
        int compteur = 1;
        foreach (HighScoreEntry highScoreEntry in highScores._highScoreEntryList)
        {
            if (compteur <= 5)
            {
                CreateHighScoreEntryTransform(highScoreEntry, _entryContainer, _highScoreEntryTransformList);
            }
            compteur++;
        }
    }


    private void EnregistrerNom()
    {
        bool valide = false;
        string saisie = _text.text;
        foreach (char c in saisie)
        {
            if (c != ' ')
            {
                valide = true;
            }
        }

        if (!string.IsNullOrEmpty(saisie) && valide)
        {
            AddHighScoreEntry(PlayerPrefs.GetInt("Score"), saisie);
            _saisieNom.SetActive(false);
            _classement.SetActive(true);
            _buttonRetour.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_buttonRetour);
            _txtErreur.SetActive(false);
            foreach (Transform child in _entryContainer.transform)
            {
                if (child.name != "HighScoreEntryTemplate")
                {
                    Destroy(child.gameObject);
                }
            }
            GenererTableHighScore();
        }
        else
        {
            _txtErreur.SetActive(true);
        }

    }

    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 50f;
        Transform entryTransform = Instantiate(_entryTemplate, container);
        RectTransform entryRectTranform = entryTransform.GetComponent<RectTransform>();
        entryRectTranform.anchoredPosition = new Vector2(0f, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
            default:
                rankString = rank + "TH"; break;
        }
        entryTransform.Find("TxtPos").GetComponent<Text>().text = rankString;

        int score = highScoreEntry.score;
        entryTransform.Find("TxtScore").GetComponent<Text>().text = score.ToString();

        string name = highScoreEntry.name;
        entryTransform.Find("TxtName").GetComponent<Text>().text = name;

        if (rank == 1)
        {
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(255, 210, 3, 71);
        }
        else if (rank == 2)
        {
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(203, 201, 193, 71);
        }
        else if (rank == 3)
        {
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(176, 114, 26, 71);
        }
        else
        {
            entryTransform.Find("background").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }




        transformList.Add(entryTransform);
    }

    public void AddHighScoreEntry(int p_score, string p_name)
    {
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = p_score, name = p_name };
        string jsonString = PlayerPrefs.GetString("highScoreTable");
        highScores = JsonUtility.FromJson<HighScores>(jsonString);

        if (highScores == null)  
        {
            highScores = new HighScores()
            {
                _highScoreEntryList = new List<HighScoreEntry>()
            };
        }

        highScores._highScoreEntryList.Add(highScoreEntry);

        string json = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString("highScoreTable", json);
        PlayerPrefs.Save();

    }

    private class HighScores
    {
        public List<HighScoreEntry> _highScoreEntryList;

    }


    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }

    public void Annuler()
    {
        _saisieNom.SetActive(false);
        _classement.SetActive(true);
        _buttonRetour.SetActive(true);
        _textScore.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_buttonRetour);
        _txtErreur.SetActive(false);
    }
}
