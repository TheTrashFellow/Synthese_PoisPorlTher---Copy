using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GestionFin : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _txtScore = default;
    private int _score;

    private void Start()
    {
        _score = PlayerPrefs.GetInt("Score");
        _txtScore.text = "Votre Pointage: " + _score.ToString();
        int compteur = PlayerPrefs.GetInt("compteur");
    }
    public void ChargerDepart()
    {
        PlayerPrefs.SetFloat("Volume", AudioFin.Instance.Volume);
        AudioDepart.Instance.FirstLoad = true;
        SceneManager.LoadScene(0);
    }
    public void Quitter()
    {
        Application.Quit();
    }
}
