using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioManager")]
    [SerializeField] private AudioClip[] _listAudio = default;
    [SerializeField] private AudioClip[] _listSounds = default;
    [SerializeField] private GameObject _soundObject = default;
    [SerializeField] private GameObject _levelUPObject = default;
    public static AudioManager Instance;
    private AudioSource _backgroundMusic;
    private AudioSource _gameSounds;
    private AudioSource _levelUPAudio;
    private float _volume;
    private bool FirstLoad = true;
    public float Volume => _volume; 
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
        _volume= AudioDepart.Instance.Volume;        
    }


    void Start()
    {
        _backgroundMusic = GetComponent<AudioSource>();
        _gameSounds = _soundObject.GetComponent<AudioSource>();
        _levelUPAudio = _levelUPObject.GetComponent<AudioSource>();
        _backgroundMusic.volume = _volume;
        _gameSounds.volume = _volume;
        _levelUPAudio.volume = _volume;
        StartCoroutine(SoundCoroutine());
    }

    public float GetVolume()
    {
        return _volume;
    }

    IEnumerator SoundCoroutine()
    {
        float _newVolume = 0f;
        while (true)
        {
            float _soundChange = Input.GetAxis("Fire3");
            

            if (_soundChange != 0 || FirstLoad)
            {
                if (FirstLoad)
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
                    default : _newVolume= 0.5f; _volume = 5;  break;
                }

                _backgroundMusic.volume = _newVolume;
                _gameSounds.volume = _newVolume;
                _levelUPAudio.volume = _newVolume;
                FirstLoad = false;
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                yield return null;
            }
        }
       
    }

    float _timeLeftAt = 0f;
    

    public void SoundBoss1Death() 
    {
        _gameSounds.clip = _listSounds[0];
        _gameSounds.Play();
    }

    public void SoundBoss2Death()
    {
        _gameSounds.clip = _listSounds[1];
        _gameSounds.Play();
    }

    public void SoundLevelUP()
    {
        _levelUPAudio.clip = _listSounds[2];
        _levelUPAudio.Play();
    }

    public void SoundXP()
    {
        _gameSounds.clip = _listSounds[3];
        _gameSounds.Play();
    }

    public void MusicDefault()
    {
        _backgroundMusic.Stop();
        _backgroundMusic.clip = _listAudio[0];
        _backgroundMusic.time = _timeLeftAt;
        _backgroundMusic.Play();
    }

    public void MusicBoss()
    {
        _timeLeftAt = _backgroundMusic.time;
        _backgroundMusic.Stop();
        _backgroundMusic.clip = _listAudio[2];
        _backgroundMusic.Play();
    }
   
}
