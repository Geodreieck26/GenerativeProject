using UnityEngine;
using System.Collections;

public class BoidVisible : MonoBehaviour {

    public bool visible = true;

    void OnBecameVisible()
    {
        visible = true;
    }

    void OnBecameInvisible()
    {
        visible = false;
    }
}
