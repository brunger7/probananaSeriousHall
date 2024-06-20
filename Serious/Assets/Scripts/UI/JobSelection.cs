using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JobSelection : MonoBehaviour
{
    public int job;
    public TextMeshProUGUI title;
    public TextMeshProUGUI time;
    public Image stamp;
    public Sprite[] stamps;
    public GameObject circle;
    public AudioSource aSource;
    public AudioClip circleSound;

    void Start()
    {
        transform.position -= new Vector3(0, job % 4 * 150 * Clipboard.size.y, 0);
        title.text = $"Job #{ResultsPaper.AddZero(job)}: {Clipboard.jobName[job]}";
        if (Clipboard.jobPb[job] == 0)
            time.text = "Best Time: N/A";
        else
            time.text = "Best Time: " + ResultsPaper.SecondsToTime(Clipboard.jobPb[job]);
        stamp.sprite = stamps[Clipboard.jobRank[job]];
    }

    public void Clicked()
    {
        if (!Clipboard.transitioning)
        {
            Clipboard.transitioning = true;
            GameObject circ = Instantiate(circle, transform);
            circ.transform.localScale = Vector3.one * 1.45f;
            Clipboard.PlaySound(aSource, circleSound);
            circ.transform.position += new Vector3(0, 200 * Clipboard.size.y, 0);
            Clipboard.jobGoto = job;
            Events.events[10].Invoke();
        }
    }
}
