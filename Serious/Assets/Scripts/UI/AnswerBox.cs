using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerBox : MonoBehaviour
{
    public Image image;
    public Sprite[] box;
    public TextMeshProUGUI answer;

    int answerId;

    void Start()
    {
        answerId = Int32.Parse("" + gameObject.name[6]);
        answer.text = Clipboard.dialogueAnswers[Clipboard.dialogueIndex][answerId];
    }

    public void Clicked()
    {
        if (DialoguePaper.selectedAnswer != answerId)
        {
            image.sprite = box[1];
            DialoguePaper.selectedAnswer = answerId;
            StartCoroutine(WaitForDisable());
        }
        else
            DialoguePaper.selectedAnswer = -1;
    }

    IEnumerator WaitForDisable()
    {
        yield return new WaitUntil(() => DialoguePaper.selectedAnswer != answerId);
        if (!gameObject.GetComponentInParent<DialoguePaper>().falling)
            image.sprite = box[0];
    }
}
