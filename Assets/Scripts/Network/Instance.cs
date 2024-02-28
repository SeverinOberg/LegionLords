using UnityEngine;

/// <summary>
/// The local instance of the game software. This is data about the instance we set at the launch of the game.
/// </summary>
public class Instance : MonoBehaviour
{
    public static Instance S { get; private set; }

    public bool IsServer { get; private set; } = false;
    public bool IsClient { get; private set; } = true;

    public MatchType MatchType { get; private set; } = MatchType.OneVSOne;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(InstanceType instanceType)
    {
        if (instanceType == InstanceType.Server)
        {
            IsServer = true;
            IsClient = false;
        }
        else
        {
            IsServer = false;
            IsClient = true;
        }
    }

}

public enum InstanceType
{
    Client,
    Server
}