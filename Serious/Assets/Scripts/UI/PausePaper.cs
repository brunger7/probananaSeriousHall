using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePaper : MonoBehaviour
{
    public GameObject circle;
    public AudioSource aSource;
    public AudioClip pageSound;
    public AudioClip circleSound;
    public TextMeshProUGUI title;

    bool falling;

    void Start()
    {
        transform.SetAsFirstSibling();
        title.text = $"{SceneManager.GetActiveScene().name} PAUSED";
    }

    void Update()
    {
        if (Clipboard.menuDepth < 1 && !falling)
        {
            Events.events[5].Invoke();
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        falling = true;
        Clipboard.transitioning = true;
        Clipboard.menuDepth = 0;
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

    public void Button(int button)
    {
        if (button != 0 || Events.canPause)
            StartCoroutine(ButtonSequence(5 + button, -100 * Clipboard.size.y * button));
    }
    IEnumerator ButtonSequence(int eventIndex, float circleY)
    {
        GameObject circ = Instantiate(circle, transform);
        Clipboard.PlaySound(aSource, circleSound);
        circ.transform.position += new Vector3(0, circleY, 0);
        if (eventIndex == 7)
        {
            Clipboard.transitioning = true;
            Clipboard.menuDepth = 2;
        }
        yield return new WaitForSecondsRealtime(0.5f);
        if (eventIndex < 7)
            StartCoroutine(Fall());
        Events.events[eventIndex].Invoke();
        yield return new WaitForSecondsRealtime(2);
        Destroy(circ);
    }
}
