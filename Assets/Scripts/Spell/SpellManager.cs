using UnityEngine;

public class SpellManager : MonoBehaviour
{

    public static SpellManager Singleton;

    [SerializeField] 
    private SpellSO[] _spells;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public SpellSO Get(SpellID id)
    {
        for (int i = 0; i < _spells.Length; i++)
        {
            if (_spells[i].ID == id)
            {
                return _spells[i];
            }
        }

        return null;
    }

}
