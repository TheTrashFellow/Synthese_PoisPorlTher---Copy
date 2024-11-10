using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestionScene : MonoBehaviour
{
    [SerializeField] private GameObject _boutonDemarrer = default;
    [SerializeField] private GameObject _boutonRetourInstructions = default;
    [SerializeField] private GameObject _boutonRetourClassement = default;
    [SerializeField] private GameObject _pannelClassement = default;
    [SerializeField] private GameObject _pannelInstructions = default;
    [SerializeField] private GameObject _gameObjectBoutons = default;
    [SerializeField] private Image _logoCegep = default;
    private void Start()
    {
        int compteur = PlayerPrefs.GetInt("compteur");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_boutonDemarrer);
    }
    public void Quitter()
    {
        Application.Quit();
    }

    public void ChangerScene()
    {
        int noScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(noScene + 1);
    }
    public void AfficherInstructions()
    {
        _pannelInstructions.SetActive(true);
        _gameObjectBoutons.SetActive(false);
        _pannelClassement.SetActive(false);
        _logoCegep.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_boutonRetourInstructions);
    }

    public void AfficherClassement()
    {
        _pannelClassement.SetActive(true);
        _gameObjectBoutons.SetActive(false);
        _pannelInstructions.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_boutonRetourClassement); 
    }

    public void RetourMenu()
    { 
        _gameObjectBoutons.SetActive(true);
        _pannelClassement.SetActive(false);
        _pannelInstructions.SetActive(false);
        _logoCegep.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_boutonDemarrer);
    }
}
