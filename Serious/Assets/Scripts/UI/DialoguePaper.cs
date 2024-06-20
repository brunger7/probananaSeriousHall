using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePaper : MonoBehaviour
{
    public static int selectedAnswer;

    public Image portrait;
    public TextMeshProUGUI speaker;
    public TextMeshProUGUI dialogue;
    public GameObject answerBox;
    public AudioSource aSource;
    public AudioClip pageSound;

    Sprite[] portraits;
    int index = Clipboard.dialogueIndex;
    string type;
    bool waitingForInput;
    AudioClip textSound;
    public bool falling;

    void Start()
    {
        selectedAnswer = -1;
        textSound = Resources.Load<AudioClip>($"Resource Sfx/{Clipboard.dialogueSpeaker[index]}Text");
        portraits = new Sprite[2] { Resources.Load<Sprite>($"Portraits/{Clipboard.dialogueSpeaker[index]}/{Clipboard.dialoguePortrait[index]}0"), Resources.Load<Sprite>($"Portraits/{Clipboard.dialogueSpeaker[index]}/{Clipboard.dialoguePortrait[index]}1") };
        portrait.sprite = portraits[0];
        transform.SetAsFirstSibling();
        if (Clipboard.dialoguePortrait[index] == "Unknown")
            speaker.text = "???";
        else
            speaker.text = Clipboard.dialogueSpeaker[index];
        type = Clipboard.dialogueType[index];
        if (type == "Q")
        {
            for (int a = 0; a < Clipboard.dialogueAnswers[index].Count; a++)
            {
                GameObject selection = Instantiate(answerBox, transform);
                selection.name = "Answer" + a;
                selection.transform.position += new Vector3(0, 75 * a * Clipboard.size.y, 0);
            }
        }
        StartCoroutine(Animate());
        StartCoroutine(WriteDialogue(Clipboard.dialogueText[index], type));
    }

    void Update()
    {
        if (Clipboard.levelStarting && !falling)
            StartCoroutine(Fall());
    }

    IEnumerator WriteDialogue(string text, string type)
    {
        yield return new WaitForSeconds(0.7f);
        yield return new WaitUntil(() => Clipboard.dialogueDelay <= 0);
        Clipboard.PlaySound(aSource, textSound, true);
        for (int c = 0; c < text.Length; c++)
        {
            dialogue.text += text[c];
            if ((text[c] == '.' || text[c] == '?' || text[c] == '!') && c != text.Length - 1)
            {
                aSource.loop = false;
                yield return new WaitForSeconds(TextSpeed(0.5f));
                StartCoroutine(WaitForInput(true));
                yield return new WaitUntil(() => !waitingForInput);
                Clipboard.PlaySound(aSource, textSound, true);
            }
            else if (text[c] == ',')
            {
                aSource.loop = false;
                yield return new WaitForSeconds(TextSpeed(0.5f));
                Clipboard.PlaySound(aSource, textSound, true);
            }
            yield return new WaitForSeconds(TextSpeed(0.05f));
        }
        aSource.loop = false;
        if (type != "Q")
            selectedAnswer = 0;
        StartCoroutine(WaitForInput());
        yield return new WaitUntil(() => !waitingForInput);
        if (Clipboard.dialogueEvents[index][selectedAnswer] != "E")
            Events.events[Int32.Parse(Clipboard.dialogueEvents[index][selectedAnswer])].Invoke();
        yield return new WaitUntil(() => Clipboard.dialogueDelay <= 0);
        if (Clipboard.dialogueGotos[index][selectedAnswer] != "G")
        {
            Clipboard.dialogueIndex = Clipboard.dialogueIds.IndexOf(Clipboard.dialogueGotos[index][selectedAnswer]);
            Clipboard.dialogueSpawn.Invoke();
        }
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        falling = true;
        Clipboard.PlaySound(aSource, pageSound);
        Vector3 startPos = transform.position;
        float startRotation = transform.eulerAngles.z;
        float rotation = UnityEngine.Random.Range(-20, 20);
        float startTime = Time.realtimeSinceStartup;
        for (float t = 0; t < 1; t = Time.realtimeSinceStartup - startTime)
        {
            yield return 0;
            transform.SetPositionAndRotation(startPos + new Vector3(0, (Mathf.Cos(t / 3f) - 1) * 23000 * Clipboard.size.y, 0), Quaternion.Euler(0, 0, startRotation + t * rotation));
        }
        Destroy(gameObject);
    }

    IEnumerator Animate()
    {
        while (true)
        {
            portrait.sprite = portraits[1];
            yield return new WaitForSeconds(0.7f);
            portrait.sprite = portraits[0];
            yield return new WaitForSeconds(0.7f);
        }
    }

    IEnumerator WaitForInput(bool beforeQuestion = false)
    {
        waitingForInput = true;
        string startString = dialogue.text;
        while (true)
        {
            bool canContinue = beforeQuestion || 
                selectedAnswer != -1;
            if (Time.time % 1 < 0.5f)
                dialogue.text = startString;
            else if (canContinue)
                dialogue.text = startString + " (X)";
            else
                dialogue.text = startString + " (Select)";
            if (Input.GetKey(KeyCode.X) && canContinue)
                break;
            yield return 0;
        }
        waitingForInput = false;
        dialogue.text = startString;
    }

    float TextSpeed(float normal)
    {
        if (Input.GetKey(KeyCode.Z)) return normal * 0.4f;
        else return normal;
    }
}
