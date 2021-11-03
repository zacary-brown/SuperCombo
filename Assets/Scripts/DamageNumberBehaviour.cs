using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumberBehaviour : MonoBehaviour
{
    [SerializeField]
    private Text tx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += new Vector3(0.1f, 0.1f);
        tx.CrossFadeAlpha(0.0f, 0.4f, false);
    }
}
