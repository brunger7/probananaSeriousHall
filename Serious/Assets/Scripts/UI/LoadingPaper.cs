using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingPaper : MonoBehaviour
{
    public AudioSource aSource;
    public AudioClip pageSound;
    public TextMeshProUGUI text;

    bool falling;

    void Start()
    {
        transform.SetAsFirstSibling();
        text.text = $"Job #{ResultsPaper.AddZero(Clipboard.jobGoto)} starting...";
    }

    void Update()
    {
        if (!Clipboard.levelStarting && !falling)
            StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        falling = true;
        Clipboard.PlaySound(aSource, pageSound);
        Vector3 startPos = transform.position;
        float startRotation = transform.eulerAngles.z;
        float rotation = Random.Range(-20, 20);
        float startTime = Time.realtimeSinceStartup;
        for (float t = 0; t < 1; t = Time.realtimeSinceStartup - startTime)
        {
            yield return 0;
            transform.SetPositionAndRotation(startPos + new Vector3(0, (Mathf.Cos(t / 3f) - 1) * 23000 * Clipboard.size.y, 0), Quaternion.Euler(0, 0, startRotation + t * rotation));
        }
        Destroy(gameObject);
    }
}
