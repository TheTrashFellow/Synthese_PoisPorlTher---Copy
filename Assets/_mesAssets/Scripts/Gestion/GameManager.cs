using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{  

    public static GameManager Instance;

    public const float CLAMPXLOW = -17.4f;
    public const float CLAMPXHIGH = 18.5f;
    public const float CLAMPYLOW = -8.3f;
    public const float CLAMPYHIGH = 7.4f;
    

    private int _score;
    public int Score => _score;
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
        _score = 0;
        StartCoroutine(ScoreCoroutine());
    }

    public void UpdateScore(int typeValue)
    {
        //Liste d'ajout au score 
        //0 = LevelUp
        //1 = OnPickup
        //2 = Defeat Ennemies
        //3 = Defeat Boss
        //4 = Every Seconds
        switch (typeValue)
        {
            case 0:
                _score += 250;
                break;
            case 1:
                _score += 25;
                break;
            case 2:
                _score += 100;
                break;
            case 3:
                _score += 1000;
                break;
            case 4:
                _score += 5;
                break;
        }
    }

    IEnumerator ScoreCoroutine()
    {
        while (Player.Instance.NbrVies >= 1)
        {
            UpdateScore(4);
            yield return new WaitForSeconds(1);
        }
    }

   
}
