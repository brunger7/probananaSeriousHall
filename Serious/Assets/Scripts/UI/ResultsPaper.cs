using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ResultsPaper : MonoBehaviour
{
    public static bool resultsSpawned;

    public TextMeshProUGUI results;
    public TextMeshProUGUI time;
    public GameObject stamp;
    public GameObject gradeStamp;

    public Sprite[] stamps;
    public Sprite[] gradeStamps;
    public int[] gradeTimes;
    public AudioSource aSource;
    public AudioClip[] sounds;
    public GameObject circle;
    public AudioClip circleSound;

    Image stampImage;
    Image gradeStampImage;
    string timeString;
    int grade;

    void Start()
    {
        resultsSpawned = true;
        float completionTime = Clipboard.timer;
        timeString = SecondsToTime(Clipboard.timer);
        stampImage = stamp.GetComponent<Image>();
        gradeStampImage = gradeStamp.GetComponent<Image>();
        if (Clipboard.progress == 100)
        {
            stampImage.sprite = stamps[1];
            grade = 4;
            for (int t = 0; t < 4; t++)
                if (gradeTimes[t] > completionTime)
                {
                    grade = t;
                    break;
                }
            if (completionTime < Clipboard.jobPb[Clipboard.jobGoto] || Clipboard.jobPb[Clipboard.jobGoto] == 0)
            {
                Clipboard.jobPb[Clipboard.jobGoto] = completionTime;
                Clipboard.jobRank[Clipboard.jobGoto] = grade;
                Clipboard.SaveData();
            }
        }
        else
        {
            grade = 5;
            stampImage.sprite = stamps[0];
        }
        gradeStampImage.sprite = gradeStamps[grade];
        transform.SetAsFirstSibling();
        StartCoroutine(WriteResults());
    }

    IEnumerator WriteResults()
    {
        yield return new WaitForSeconds(1);
        Clipboard.PlaySound(aSource, sounds[3], true);
        for (int p = 0; p < Clipboard.progress; p++)
        {
            results.text = $"Progress: {p}%";
            yield return new WaitForSeconds(0.02f);
        }
        aSource.loop = false;
        results.text = $"Progress: {Clipboard.progress}%";
        yield return new WaitForSeconds(1);
        Clipboard.PlaySound(aSource, sounds[1]);
        for (float t = 0; t < 1; t += Time.deltaTime * 4)
        {
            stamp.transform.localScale = Vector3.one * 2 - Vector3.one * t;
            stampImage.color = new Color(1, 1, 1, t);
            yield return 0;
        }
        stamp.transform.localScale = Vector3.one;
        stampImage.color = Color.white;
        yield return new WaitForSeconds(1);
        Clipboard.PlaySound(aSource, sounds[3], true);
        for (int c = 0; c < timeString.Length; c++)
        {
            time.text += timeString[c];
            yield return new WaitForSeconds(0.05f);
        }
        aSource.loop = false;
        yield return new WaitForSeconds(0.3f);
        Clipboard.PlaySound(aSource, sounds[2]);
        for (float t = 0; t < 1; t += Time.deltaTime * 3)
        {
            gradeStamp.transform.localScale = Vector3.one * 2 - Vector3.one * t;
            gradeStampImage.color = new Color(1, 1, 1, t);
            yield return 0;
        }
        gradeStamp.transform.localScale = Vector3.one;
        gradeStampImage.color = Color.white;
    }

    IEnumerator Fall()
    {
        Clipboard.PlaySound(aSource, sounds[0]);
        Vector3 startPos = transform.position;
        float startRotation = transform.eulerAngles.z;
        float rotation = UnityEngine.Random.Range(-20, 20);
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            yield return 0;
            transform.SetPositionAndRotation(startPos + new Vector3(0, (Mathf.Cos(t / 3f) - 1) * 23000 * Clipboard.size.y, 0), Quaternion.Euler(0, 0, startRotation + t * rotation));
        }
        Destroy(gameObject);
    }

    public static string AddZero(float input)
    {
        if (input < 10) return "0" + input;
        else return input + "";
    }

    public static string SecondsToTime(float seconds)
    {
        return $"{Mathf.Floor(seconds / 60)}:{AddZero(Mathf.Floor(1000 * (seconds % 60)) / 1000)}";
    }

    public void Button(int button)
    {
        StartCoroutine(ButtonSequence(button, (-150 + 300 * button) * Clipboard.size.y));
    }
    IEnumerator ButtonSequence(int button, float circleX)
    {
        GameObject circ = Instantiate(circle, transform);
        Clipboard.PlaySound(aSource, circleSound);
        circ.transform.position += new Vector3(circleX, -375 * Clipboard.size.y, 0);
        circ.transform.localScale = new Vector3(0.7f, 1, 1);
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(Fall());
        if (button == 0)
            Events.events[6].Invoke();
        else
            Events.events[8].Invoke();
    }
}
