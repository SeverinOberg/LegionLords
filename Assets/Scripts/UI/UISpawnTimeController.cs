using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UISpawnTimeController : NetworkBehaviour
{

    /// <summary> Singleton </summary>
    public static UISpawnTimeController S { get; private set; }

    private TextMeshProUGUI _timeText;

    private float _t = 0;
    private float _resetTime = 0;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _timeText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _t -= Time.deltaTime;
        _timeText.text = $"{(int)_t}";

        if (_t > 0) return;
        _t = _resetTime;
    }

    public void SetTimer(float time)
    {
        _t = time;
        _resetTime = time;
    }

    [ClientRpc]
    public void SetSpawnTimeClientRpc(byte time, ClientRpcParams clientRpcParams = default)
    {
        SetTimer(time);
    }

}
