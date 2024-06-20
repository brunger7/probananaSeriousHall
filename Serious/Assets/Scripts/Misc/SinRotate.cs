using UnityEngine;

public class SinRotate : MonoBehaviour
{
    public float start;
    public float difference;

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, start + difference * Mathf.Sin(Time.time));
    }
}
