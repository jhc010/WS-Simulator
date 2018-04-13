using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour {
    private int count = 0;

    public void Output(string input)
    {
        count += 1;
        Text t = this.GetComponent<Text>();
        t.text += "\n [" + count + "] " + input;
    }
}
