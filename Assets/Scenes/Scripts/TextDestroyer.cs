using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDestroyer : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 0.5f;
    void Start()
    {
        StartCoroutine(DestroyObject());
    }
    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
