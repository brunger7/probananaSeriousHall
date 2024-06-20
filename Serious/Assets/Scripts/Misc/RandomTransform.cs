using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomTransform : MonoBehaviour
{
    public Vector2 maxDisplacement;
    public float maxRotation;

    void Start()
    {
        transform.position += new Vector3(Clipboard.size.x * Random.Range(-maxDisplacement.x, maxDisplacement.x), Clipboard.size.y * Random.Range(-maxDisplacement.y, maxDisplacement.y), 0);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-maxRotation, maxRotation));
    }
}
