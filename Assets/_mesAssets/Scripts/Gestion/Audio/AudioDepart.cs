using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDepart : MonoBehaviour
{
    public static AudioDepart Instance;
    [SerializeField] private GameObject _DebutObject = default;
    private AudioSource _DebutAudio;
    private float _volume = 1;
    public float Volume => _volume;

    private bool _firstLoad = true;

    public bool FirstLoad
    {
        
        get { return _firstLoad; }
        set { _firstLoad = value; }
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
    void Start()
    {
        _DebutAudio = _DebutObject.GetComponent<AudioSource>();
        StartCoroutine(SoundCoroutine());
    }

    IEnumerator SoundCoroutine()
    {        
        float _newVolume = 0f;
        float _soundChange = 0f;
        _volume = PlayerPrefs.GetFloat("Volume");
        while (true)
        {            
            _soundChange = Input.GetAxis("Fire3");
            
            if (_soundChange != 0 || FirstLoad)
            {
                if(FirstLoad)
                {
                    if (_volume == 1)
                        _volume = 5;
                    else
                        _volume--;
                }

                switch (_volume)
                {
                    case 1:
                        _newVolume = 0.75f; _volume = 2; break;
                    case 2:
                        _newVolume = 0.5f; _volume = 3; break;
                    case 3:
                        _newVolume = 0.25f; _volume = 4; break;
                    case 4:
                        _newVolume = 0f; _volume = 5; break;
                    case 5:
                        _newVolume = 1f; _volume = 1; break;
                }
                FirstLoad= false;
                _DebutAudio.volume = _newVolume;
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                yield return null;
            }
        }        

    }
}
