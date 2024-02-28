using UnityEngine;
using System.Collections;

public class DestroyAfterSeconds : MonoBehaviour
{

    [SerializeField] private float _seconds;

    private void Awake()
    {
        StartCoroutine(Init(_seconds));
    }

    private IEnumerator Init(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

}
