using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPicture : MonoBehaviour
{
    [SerializeField] private Material[] mats = new Material[5];

    // Start is called before the first frame update
    void Start()
    {
        Material mat = GetComponent<Renderer>().material;
        GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
    }

}
