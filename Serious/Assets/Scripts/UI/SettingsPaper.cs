using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPaper : MonoBehaviour
{
    public static float musicVolume;
    public static float sfxVolume;

    public GameObject circle;
    public AudioSource aSource;
    public AudioClip pageSound;
    public AudioClip circleSound;
    public AudioMixer aMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    bool falling;

    void Start()
    {
        musicSlider.value = musicVolume; 
        sfxSlider.value = sfxVolume;
        transform.SetAsFirstSibling();
    }

    void Update()
    {
        if (Clipboard.menuDepth < 2 && !falling)
            StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        falling = true;
        Clipboard.transitioning = true;
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

    public void ChangeSettings()
    {
        aMixer.SetFloat("MusicVol", musicSlider.value);
        musicVolume = musicSlider.value;
        aMixer.SetFloat("SfxVol", sfxSlider.value);
        sfxVolume = sfxSlider.value;
    }
}
