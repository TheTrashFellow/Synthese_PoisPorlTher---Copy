using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class Blast : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] public GameObject trailPrefab = default;
    [SerializeField] private GameObject _container = default;
    public float frameRate = 0.1f;

    private void Start()
    {
        _container = GameObject.FindGameObjectWithTag("Container");
        StartCoroutine(InstantiateTrail());
    }
    private void Update()
    {
        if (!(transform.position.y >= GameManager.CLAMPYLOW && transform.position.y <= GameManager.CLAMPYHIGH) ||
            !(transform.position.x >= GameManager.CLAMPXLOW && transform.position.x <= GameManager.CLAMPXHIGH))
            Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.Instance.DegatsRecusJoueur(_damage);
            Destroy(gameObject);
        }
    }

    private IEnumerator InstantiateTrail()
    {        
        while (true)
        {
            Vector3 blastPosition = transform.position;

            float trailOffsetY = trailPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
            float additionalOffset = 1.0f;
            Vector3 trailPosition = blastPosition - new Vector3(0f, trailOffsetY / 2 + additionalOffset, 0f);

            GameObject trail = Instantiate(trailPrefab, trailPosition, Quaternion.identity);

            trail.transform.SetParent(_container.transform);
            Destroy(trail, 4f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
