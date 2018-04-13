using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// subclass of <c>Dropzone</c> to handle memory management 
/// </summary>
public class Memory : DropZone
{
    Text text;
    Player player;

    public GameObject listButton = null;
    public GameObject WRlist = null;
    public GameObject scroll = null;
    public GameObject menu = null;

    void Start()
    {

        if (this.name[0] != '_')
        {
            listButton = GameObject.Find("MemClose");
            listButton.SetActive(false);
        }
        list = new List<GameObject>();
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }

    }

    public override void OnDrop(PointerEventData eventData)
    {

        if (!drop)
        {
            return;
        }

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();

        if (list.Count > 0)
        {
            help_hide(list[list.Count - 1]);
        }

        list.Add(eventData.pointerDrag);
        Debug.Log(eventData.pointerDrag.name);

        if (d != null)
        {
            d.parentToRet = this.transform;
        }

    }

    public void Close()
    {
        Transform p = WRlist.transform.GetChild(0);
        Transform[] arr = p.GetComponentsInChildren<Transform>();
        Transform t;

        if (NetworkServer.active)
        {
            t = this.transform;
        }
        else
        {
            t = GameObject.Find("_Memory").transform;
        }

        for (int i = 1; i < arr.Length; i++)
        {
            Transform c = arr[i];
            Debug.Log(c.name);
            c.gameObject.SetActive(false);
            list.Add(c.gameObject);
            c.SetParent(t);
            RectTransform rt = (RectTransform)c;
            rt.sizeDelta = new Vector2(40f, 56f);
        }
        Destroy(WRlist);
        Destroy(scroll);
        listButton.SetActive(false);
        if (list.Count != 0)
        {
            list[list.Count - 1].SetActive(true);
        }
    }

    public void List()
    {
        Transform info = GameObject.Find("Info").transform;
        listButton.SetActive(true);
        WRlist = (GameObject)Instantiate(Resources.Load("ScrollPanel"), info);
        scroll = (GameObject)Instantiate(Resources.Load("Scrollbar"), info);

        GameObject dest = GameObject.Find("ListWithCards");

        if (NetworkServer.active)
        {
            foreach (GameObject t in list)
            {
                t.SetActive(true);
                t.transform.SetParent(dest.transform);
            }
            list.Clear();
        }
        else
        {
            foreach (GameObject t in GameObject.Find("_Memory").GetComponent<Memory>().list)
            {
                t.SetActive(true);
                t.transform.SetParent(dest.transform);
            }
            list.Clear();
        }
    }

    private void help_hide(GameObject ob)
    {
        SyncCard c = ob.GetComponent<SyncCard>();
        player.Cmd_Hide(c.netId);
    }
}
