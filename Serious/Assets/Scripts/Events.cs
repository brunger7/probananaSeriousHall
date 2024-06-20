using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Events : MonoBehaviour
{
    public static List<UnityEvent> events = new();
    public static bool canPause = true;
    public static int menuDepth;

    public AudioSource aSource;
    public AudioSource aSourceUi;
    public AudioSource aSourceMusic;
    public AudioMixer aMixer;
    public GameObject[] objects;

    void Start()
    {
        if (events.Count == 0)
            for (int i = 0; i < 12; i++)
                events.Add(new());
        events[0].AddListener(SpawnResults);
        events[1].AddListener(RedLight);
        events[2].AddListener(TurnOnLights);
        events[3].AddListener(AddProgress);
        events[4].AddListener(Job0Start);
        events[5].AddListener(Pause);
        events[6].AddListener(Retry);
        events[7].AddListener(Settings);
        events[8].AddListener(ExitJob);
        events[9].AddListener(SpawnSelection);
        events[10].AddListener(StartJob);
        events[11].AddListener(Quit);
        aMixer.SetFloat("GameVol", 0);
    }

    void SpawnResults()
    {
        Instantiate(objects[0], GameObject.Find("Clipboard").transform);
    }

    void RedLight()
    {
        StartCoroutine(LightFlash());
    }
    IEnumerator LightFlash()
    {
        Light2D light = GameObject.Find("LampLight").GetComponent<Light2D>();
        Image tint = GameObject.Find("Tint").GetComponent<Image>();
        Clipboard.PlaySound(aSource, Resources.Load<AudioClip>("Resource Sfx/LightBig"));
        for (float c = 0; c < 1; c += Time.deltaTime / 2)
        {
            light.color = new Color(1 - 0.2f + c / 10, c / 10, c / 10);
            tint.color = new Color(1, 0, 0, 0.02f - c / 50);
            yield return 0;
        }
    }

    void TurnOnLights()
    {
        Clipboard.dialogueDelay = 5;
        StartCoroutine(LightSequence());
    }
    IEnumerator LightSequence()
    {
        aSourceMusic.Pause();
        yield return new WaitForSeconds(2);
        Clipboard.PlaySound(aSource, Resources.Load<AudioClip>("Resource Sfx/LightSwitch"));
        GameObject.Find("Bulb").SetActive(false);
        Instantiate(objects[1]);
        yield return new WaitForSeconds(2);
        Clipboard.PlaySound(aSource, Resources.Load<AudioClip>("Resource Sfx/Confetti"));
        Clipboard.PlaySound(aSourceMusic, Resources.Load<AudioClip>("Resource Music/It's Showtime!"), true);
    }

    void AddProgress()
    {
        Clipboard.points++;
        Clipboard.progress = Mathf.RoundToInt((float)Clipboard.points / Clipboard.maxPoints * 100f);
    }

    void Job0Start()
    {
        Clipboard.dialogueDelay = 5;
        StartCoroutine(Job0Sequence());
    }
    IEnumerator Job0Sequence()
    {
        yield return new WaitForSeconds(2);
        Clipboard.PlaySound(aSource, Resources.Load<AudioClip>("Resource Sfx/LightBig"));
        GameObject.Find("LampLight").GetComponent<Light2D>().color = new(0.2f, 0.2f, 0.2f, 1);
        GameObject.Find("LampLightTop").GetComponent<Light2D>().color = Color.grey;
    }

    void Pause()
    {
        if (canPause)
        {
            canPause = false;
            if (Time.timeScale == 1)
                StartCoroutine(PauseSequence());
            else
                StartCoroutine(UnpauseSequence());
        }
    }
    IEnumerator PauseSequence()
    {
        Time.timeScale = 0; 
        Clipboard.menuDepth = 1;
        GameObject.Find("Tint").GetComponent<Image>().color = new(0, 0, 0, 0.5f);
        aMixer.SetFloat("GameVol", -80);
        StartCoroutine(RevealPaper(Instantiate(objects[2], GameObject.Find("Clipboard").transform)));
        GameObject.Find("Progress").GetComponent<TextMeshProUGUI>().text = $"Progress: {Clipboard.progress}%";
        GameObject.Find("Time").GetComponent<TextMeshProUGUI>().text = $"Time: {ResultsPaper.SecondsToTime(Clipboard.timer)}";
        yield return new WaitForSecondsRealtime(2);
        canPause = true;
    }
    IEnumerator UnpauseSequence()
    {
        Image tint = GameObject.Find("Tint").GetComponent<Image>();
        float startTime = Time.realtimeSinceStartup;
        for (float t = 0; t < 1; t = (Time.realtimeSinceStartup - startTime) * 2)
        {
            yield return 0;
            Time.timeScale = t;
            tint.color = new(0, 0, 0, 0.5f - t / 4);
        }
        Time.timeScale = 1;
        tint.color = new(0, 0, 0, 0);
        aMixer.SetFloat("GameVol", 0);
        canPause = true;
    }

    void Retry()
    {
        StartCoroutine(RetrySequence());
    }
    IEnumerator RetrySequence()
    {
        Instantiate(objects[4], GameObject.Find("Clipboard").transform);
        Clipboard.levelStarting = true;
        Image tint = GameObject.Find("Tint").GetComponent<Image>();
        float startTime = Time.realtimeSinceStartup;
        for (float t = 0.5f; t < 1; t = (Time.realtimeSinceStartup - startTime) / 2 + 0.5f)
        {
            yield return 0;
            tint.color = new(0, 0, 0, t);
        }
        Time.timeScale = 1;
        SceneManager.LoadScene("Job #" + ResultsPaper.AddZero(Clipboard.jobGoto));
    }

    void Settings()
    {
        StartCoroutine(RevealPaper(Instantiate(objects[3], GameObject.Find("Clipboard").transform)));
    }

    void ExitJob()
    {
        StartCoroutine(ExitSequence());
    }
    IEnumerator ExitSequence()
    {
        Clipboard.menuDepth = 0;
        Clipboard.levelStarting = true;
        Instantiate(objects[5], GameObject.Find("Clipboard").transform);
        Image tint = GameObject.Find("Tint").GetComponent<Image>();
        GameObject board = GameObject.Find("Board");
        RectTransform clipboard = GameObject.Find("Clipboard").GetComponent<RectTransform>();
        RectTransform clip = GameObject.Find("Clip").GetComponent<RectTransform>();
        for (float t = Mathf.PI / 2; t > 0; t -= Time.deltaTime)
        {
            yield return 0;
            tint.color = new(0, 0, 0, 1 - t / (Mathf.PI / 2));
            board.transform.localScale = Vector3.one * (float)(-0.5 * Mathf.Pow(Mathf.Sin(t), 2) + 1.6);
            clipboard.anchoredPosition = new Vector3((float)(-1200 * Mathf.Pow(Mathf.Sin(t), 2)), 750, 0);
            clip.anchoredPosition = new Vector3((float)(-1200 * Mathf.Pow(Mathf.Sin(t), 2)), 750, 0);
        }
        Clipboard.levelStarting = false;
        SceneManager.LoadScene("MainMenu");
    }

    void SpawnSelection()
    {
        StartCoroutine(RevealPaper(Instantiate(objects[2], GameObject.Find("Clipboard").transform)));
    }

    void StartJob()
    {
        StartCoroutine(StartingSequence());
    }
    IEnumerator StartingSequence()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Clipboard.menuDepth = 0;
        Clipboard.levelStarting = true;
        Instantiate(objects[1], GameObject.Find("Clipboard").transform);
        Image tint = GameObject.Find("Tint").GetComponent<Image>();
        GameObject board = GameObject.Find("Board");
        RectTransform clipboard = GameObject.Find("Clipboard").GetComponent<RectTransform>();
        RectTransform clip = GameObject.Find("Clip").GetComponent<RectTransform>();
        for (float t = 0; t < Mathf.PI / 2; t += Time.deltaTime)
        {
            yield return 0;
            tint.color = new(0, 0, 0, t / (Mathf.PI / 2));
            board.transform.localScale = Vector3.one * (float)(-0.5 * Mathf.Pow(Mathf.Sin(t), 2) + 1.6);
            clipboard.anchoredPosition = new Vector3((float)(-1200 * Mathf.Pow(Mathf.Sin(t), 2)), 750, 0);
            clip.anchoredPosition = new Vector3((float)(-1200 * Mathf.Pow(Mathf.Sin(t), 2)), 750, 0);
        }
        SceneManager.LoadScene("Job #" + ResultsPaper.AddZero(Clipboard.jobGoto));
    }

    void Quit()
    {
        Application.Quit();
    }

    IEnumerator RevealPaper(GameObject paper)
    {
        Clipboard.transitioning = true;
        Clipboard.PlaySound(aSourceUi, Resources.Load<AudioClip>("Resource Sfx/PageSlide"));
        Vector3 startPos = paper.transform.position;
        float startTime = Time.realtimeSinceStartup;
        for (float t = 0; t < Mathf.PI / 2; t = (Time.realtimeSinceStartup - startTime) * 2.5f)
        {
            yield return 0;
            paper.transform.position = startPos + new Vector3(Mathf.Pow(Mathf.Sin(t), 2) * 700 * Clipboard.size.x, 0, 0);
        }
        paper.transform.SetAsLastSibling();
        for (float t = Mathf.PI / 2; t < Mathf.PI; t = (Time.realtimeSinceStartup - startTime) * 2.5f)
        {
            yield return 0;
            paper.transform.position = startPos + new Vector3(Mathf.Pow(Mathf.Sin(t), 2) * 700 * Clipboard.size.x, 0, 0);
        }
        Clipboard.transitioning = false;
    }
}
