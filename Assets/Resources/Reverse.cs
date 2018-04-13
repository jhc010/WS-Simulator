using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reverse : MonoBehaviour
{
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { func(); });
    }

    public void func()
    {
        Transform p = this.transform.parent.parent;
        p.eulerAngles = new Vector3(0, 0, 180);

        GameObject ob = p.gameObject;
        Draggable d = p.gameObject.GetComponent<Draggable>();

        d.player.Cmd_Sync(p.parent.name, ob.GetComponent<SyncCard>().netId, ob.transform.position,
                        ob.transform.localPosition, ob.transform.rotation, ob.transform.localRotation);
    }
}
