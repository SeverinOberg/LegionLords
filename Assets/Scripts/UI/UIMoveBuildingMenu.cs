using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UIMoveBuildingMenu : MonoBehaviour
{

    [SerializeField]
    private float _moveAfterSeconds = 1;

    private void Awake()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(_moveAfterSeconds);
        transform.DOMoveY(-1, 2).SetEase(Ease.OutElastic, 1).OnComplete(() => 
        {
            Destroy(this); 
        });
    }

}
