using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFin : MonoBehaviour
{
    public static AudioFin Instance;
    [SerializeField] private GameObject _FinObject = default;
    private AudioSource _FinAudio;
    private float _volume;
    public float Volume => _volume;
    private bool FirstLoad = true;
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
        _volume = AudioManager.Instance.Volume;
    }
    void Start()
    {
        _FinAudio = _FinObject.GetComponent<AudioSource>();
        _FinAudio.volume = _volume;
        StartCoroutine(SoundCoroutine());
    }

    IEnumerator SoundCoroutine()
    {
        while (true)
        {
            float _soundChange = Input.GetAxis("Fire3");
            float _newVolume = 0f;

            if (_soundChange != 0 || FirstLoad)
            {

                if (FirstLoad)
                {
                    _volume = AudioManager.Instance.Volume;
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

                _FinAudio.volume = _newVolume;
                FirstLoad = false;
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                yield return null;
            }
        }

    }
}
