using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobPaper : MonoBehaviour
{
    public GameObject circle;
    public AudioSource aSource;
    public AudioClip pageSound;
    public AudioClip circleSound;
    public GameObject jobObject;

    bool falling;

    void Start()
    {
        transform.SetAsFirstSibling();
        for (int i = 0; i < Clipboard.jobName.Count; i++)
        {
            Instantiate(jobObject, transform).GetComponent<JobSelection>().job = i;
        }
    }

    void Update()
    {
        if ((Clipboard.levelStarting || Clipboard.menuDepth < 2) && !falling)
            StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        falling = true;
        Clipboard.menuDepth = 1;
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
        Clipboard.transitioning = false;
        Destroy(gameObject);
    }

    public void Close()
    {
        StartCoroutine(CloseSequence());
    }
    IEnumerator CloseSequence()
    {
        Clipboard.transitioning = true;
        GameObject circ = Instantiate(circle, transform);
        Clipboard.PlaySound(aSource, circleSound);
        circ.transform.position -= new Vector3(0, 400 * Clipboard.size.y, 0);
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(Fall());
    }
}
