using UnityEngine;
using TMPro;
using DG.Tweening;

public class HUDFloatingText : MonoBehaviour
{

    [SerializeField]
    private float _heightEndValue;
    [SerializeField]
    private float _duration;

    public TextMeshPro Text;

    private float _xRot = 60;

    private void Awake()
    {
        transform.eulerAngles = new Vector3(_xRot, 0, 0);
        transform.DOMoveY(_heightEndValue, _duration).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

}
