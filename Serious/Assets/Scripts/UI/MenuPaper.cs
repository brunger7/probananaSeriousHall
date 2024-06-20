using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPaper : MonoBehaviour
{
    public GameObject circle;
    public AudioSource aSource;
    public AudioClip pageSound;
    public AudioClip circleSound;

    bool falling;
    bool menu = true;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            menu = false;
            transform.SetAsFirstSibling();
        }
    }
    void Update()
    {
        if (Clipboard.levelStarting && !falling && menu)
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

    public void Button(int index)
    {
        StartCoroutine(ButtonSequence(index, CircleY(index)));
    }
    float CircleY(int index)
    {
        if (index == 9) return -100 * Clipboard.size.y;
        else if (index == 7) return -200 * Clipboard.size.y;
        else return -300 * Clipboard.size.y;
    }
    IEnumerator ButtonSequence(int eventIndex, float circleY)
    {
        GameObject circ = Instantiate(circle, transform);
        Clipboard.PlaySound(aSource, circleSound);
        circ.transform.position += new Vector3(0, circleY, 0);
        yield return new WaitForSecondsRealtime(0.5f);
        if (eventIndex == 7 || eventIndex == 9)
            Clipboard.menuDepth = 2;
        Events.events[eventIndex].Invoke();
        yield return new WaitForSecondsRealtime(2);
        Destroy(circ);
    }
}
