using UnityEngine;

public class SpellIndicator : MonoBehaviour
{

    [SerializeField]
    private Material _matError;
    [SerializeField]
    private Material _matOK;

    public void SetRadius(float radius)
    {
        transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);
    }

}
