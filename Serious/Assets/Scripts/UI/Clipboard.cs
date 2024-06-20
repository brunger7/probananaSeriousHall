using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Clipboard : MonoBehaviour
{
    public static List<string> dialogueIds = new();
    public static List<string> dialogueType = new();
    public static List<string> dialogueSpeaker = new();
    public static List<string> dialoguePortrait = new();
    public static List<string> dialogueText = new();
    public static List<List<string>> dialogueAnswers = new();
    public static List<List<string>> dialogueGotos = new();
    public static List<List<string>> dialogueEvents = new();
    public static int dialogueIndex;
    public static UnityEvent dialogueSpawn = new();
    public static float dialogueDelay;

    public static List<string> jobName = new();
    public static List<List<int>> jobGoals = new();
    public static List<float> jobPb = new();
    public static List<int> jobRank = new();
    public static int jobGoto;

    public static int progress;
    public static int points;
    public static int maxPoints;
    public static float timer;
    public static int menuDepth;
    public static bool levelStarting;
    public static Vector2 size;
    public static Vector2 screenSize;
    public static bool transitioning;

    public bool menu;
    public string startId;
    public int startEvent;
    public int pointsToWin;
    public GameObject dialoguePage;
    public GameObject menuPage;
    public GameObject clipboard;

    void Start()
    {
        ResultsPaper.resultsSpawned = false;
        levelStarting = false;
        maxPoints = pointsToWin;
        points = 0;
        timer = 0;
        menuDepth = 0;
        if (dialogueIds.Count == 0)
            LoadData();
        screenSize = new Vector2(1f / 3840 * Screen.width, 1f / 2160 * Screen.height);
        size = transform.localScale * screenSize;
        StartCoroutine(StartSequence());
    }
    IEnumerator StartSequence()
    {
        if (menu)
            goto startTint;
        yield return 0;
        levelStarting = false;
        Events.events[startEvent].Invoke();
        dialogueSpawn.AddListener(SpawnDialogue);
        dialogueIndex = dialogueIds.IndexOf(startId);
        Instantiate(dialoguePage, clipboard.transform);
        startTint:
        Image tint = GameObject.Find("Tint").GetComponent<Image>();
        tint.color = new(0, 0, 0, 1);
        float startTime = Time.realtimeSinceStartup;
        for (float t = 1; t >= 0; t = 1 - (Time.realtimeSinceStartup - startTime) / 2)
        {
            tint.color = new(0, 0, 0, t);
            yield return 0;
        }
        tint.color = new(0, 0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !transitioning && !menu && !ResultsPaper.resultsSpawned)
        {
            if (menuDepth == 0)
                Events.events[5].Invoke();
            else
                menuDepth--;
        }
        timer += Time.deltaTime;
        if (dialogueDelay > 0)
            dialogueDelay -= Time.deltaTime;
    }

    void SpawnDialogue()
    {
        Instantiate(dialoguePage, clipboard.transform);
    }

    public static void PlaySound(AudioSource source, AudioClip clip, bool loop = false)
    {
        source.loop = loop;
        source.clip = clip;
        source.Play();
    }

    void LoadData()
    {
        StreamReader dialogue = new(Application.streamingAssetsPath + "/Dialogue.txt");
        while (true)
        {
            dialogueIds.Add(dialogue.ReadLine());
            string type = dialogue.ReadLine();
            dialogueType.Add(type);
            dialogueSpeaker.Add(dialogue.ReadLine());
            dialoguePortrait.Add(dialogue.ReadLine());
            dialogueText.Add(dialogue.ReadLine());
            dialogueAnswers.Add(new());
            dialogueGotos.Add(new());
            dialogueEvents.Add(new());
            if (type == "Q")
            {
                int count = Int32.Parse(dialogue.ReadLine());
                for (int q = 0; q < count; q++)
                    dialogueAnswers[^1].Add(dialogue.ReadLine());
                for (int g = 0; g < count; g++)
                    dialogueGotos[^1].Add(dialogue.ReadLine());
                for (int e = 0; e < count; e++)
                    dialogueEvents[^1].Add(dialogue.ReadLine());
            }
            else
            {
                dialogueGotos[^1].Add(dialogue.ReadLine());
                dialogueEvents[^1].Add(dialogue.ReadLine());
            }
            if (dialogue.ReadLine() == null)
                break;
        }
        dialogue.Close();
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SeriousHallProBanana";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (!File.Exists(path + "/JobData"))
        {
            File.Create(path + "/JobData").Close();
            StreamWriter file = new(path + "/JobData");
            file.Write(new StreamReader(Application.streamingAssetsPath + "/DefaultFile.txt").ReadToEnd());
            file.Close();
        }
        StreamReader fileReader = new(path + "/JobData");
        int jobCount = Int32.Parse(fileReader.ReadLine());
        int jobCountReal = Int32.Parse(new StreamReader(Application.streamingAssetsPath + "/DefaultFile.txt").ReadLine());
        string restOfData = fileReader.ReadToEnd();
        fileReader.Close();
        if (jobCount != jobCountReal)
        {
            StreamReader defaultData = new(Application.streamingAssetsPath + "/DefaultFile.txt");
            for (int l = 0; l < jobCount * 8; l++)
            {
                defaultData.ReadLine();
            }
            StreamWriter file = new(path + "/JobData");
            file.WriteLine(jobCountReal);
            file.WriteLine(restOfData);
            file.WriteLine(defaultData.ReadToEnd());
            defaultData.Close();
            file.Close();
        }
        StreamReader jobs = new(path + "/JobData");
        jobs.ReadLine();
        while (true)
        {
            jobName.Add(jobs.ReadLine());
            jobGoals.Add(new() { Int32.Parse(jobs.ReadLine()), Int32.Parse(jobs.ReadLine()), Int32.Parse(jobs.ReadLine()), Int32.Parse(jobs.ReadLine()) });
            jobPb.Add(float.Parse(jobs.ReadLine()));
            jobRank.Add(Int32.Parse(jobs.ReadLine()));
            if (jobs.ReadLine() == null)
                break;
        }
        jobs.Close(); 
    }

    public static void SaveData()
    {
        StreamWriter file = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SeriousHallProBanana/JobData");
        Debug.Log(jobName.Count);
        file.WriteLine(jobName.Count);
        for (int i = 0; i < jobName.Count; i++)
        {
            file.WriteLine(jobName[i]);
            foreach (int g in jobGoals[i])
                file.WriteLine(g);
            file.WriteLine(jobPb[i]);
            file.WriteLine(jobRank[i]); 
            if (i != jobName.Count - 1)
                file.WriteLine();
        }
        file.Close();
    }
}
