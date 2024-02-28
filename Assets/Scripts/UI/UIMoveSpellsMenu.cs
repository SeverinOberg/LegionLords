using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UIMoveSpellsMenu : MonoBehaviour
{

    [SerializeField]
    private float _moveAfterSeconds = 60;

    private void Awake()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(_moveAfterSeconds);
        transform.DOMoveX(-5, 2).SetEase(Ease.OutElastic).OnComplete(() => 
        {
            Destroy(this); 
        });
    }

}
