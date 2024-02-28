using System.Collections;
using UnityEngine;
using TMPro;

public class AnnouncementManager : MonoBehaviour
{

    public static AnnouncementManager S;

    private TextMeshProUGUI _announcementText;

    private Coroutine _coroutine;
    private WaitForSeconds _cachedWaitForSeconds = new(1);

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _announcementText = GameObject.Find("Announcement Text").GetComponent<TextMeshProUGUI>();
        _announcementText.text = "";
    }

    public void Announce(string announcement)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(DoAnnouncement(announcement));
    }

    private IEnumerator DoAnnouncement(string announcement)
    {
        _announcementText.text = announcement;
        yield return _cachedWaitForSeconds;
        _announcementText.text = "";
    }

}
