using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// subclass of <c>Dropzone</c> to handle waitingroom management 
/// </summary>
public class Wr : DropZone {
    Text text;
    Player player = null;
    Player player2 = null;

    public GameObject listButton = null;
    public GameObject WRlist = null;
    public GameObject scroll = null;

    void Start()
    {
        if (this.name[0] != '_')
        {
            listButton = GameObject.Find("WRClose");
            listButton.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
        if (player2 == null)
        {
            GameObject ob = GameObject.Find("Player(Clone)");
            if (ob != null)
            {
                player2 = ob.GetComponent<Player>();
            }
        }
        if (list.Count == 0)
        {
            list = player.wr;
        }

        if (this.name[0] == '_')
        {
            if (text == null)
            {
                GameObject go = GameObject.Find("_WRCount");
                text = go.GetComponent<Text>();
            }
            
        }
        else
        {
            if (text == null)
            {
                GameObject go = GameObject.Find("WRCount");
                text = go.GetComponent<Text>();
            }
        }
        if (text)
        {
            Draggable[] arr = this.GetComponentsInChildren<Draggable>(true);
            text.text = arr.Length + " cards";
        }

    }

    public override void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped in Waiting");
        if (!drop)
        {   
            return;
        }

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        
        if (list.Count != 0)
        {
            //list[list.Count - 1].SetActive(false);
            help_hide(list[list.Count - 1]);
        }

        list.Add(eventData.pointerDrag);

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
            t = GameObject.Find("_WaitingRoom").transform;
        }
        for (int i = 1; i < arr.Length; i++)
        {
            Transform c = arr[i];
            Debug.Log(c.name);
            //c.gameObject.SetActive(false);
            list.Add(c.gameObject);
            c.SetParent(t);
            help_hide(c.gameObject);
            RectTransform rt = (RectTransform)c;
            rt.sizeDelta = new Vector2(40f, 56f);
        }
        Destroy(WRlist);
        Destroy(scroll);
        listButton.SetActive(false);
        if (list.Count != 0)
        {
            //list[list.Count - 1].SetActive(true);
            help_show(list[list.Count - 1]);
        }
    }

    public void List()
    {
        Transform info = GameObject.Find("Info").transform;
        listButton.SetActive(true);
        WRlist = (GameObject)Instantiate(Resources.Load("ScrollPanel"), info);
        scroll = (GameObject)Instantiate(Resources.Load("Scrollbar"), info);

        GameObject dest = GameObject.Find("ListWithCards");
        
        foreach (GameObject t in list){
            t.SetActive(true);
            t.transform.SetParent(dest.transform);
        }
        list.Clear();
    }

    public void Refresh()
    {
        string par;
        if (NetworkServer.active)
        {
            par = "Deck";
        }
        else
        {
            par = "_Deck";
        }
        GameObject d = GameObject.Find(par);

        if (player.deck.Count != 0) {
            help_hide(player.deck[player.deck.Count - 1]);
        }
       
        foreach (GameObject ob in list)
        {
            player.deck.Add(ob);
            player.Cmd_Sync(par, ob.GetComponent<SyncCard>().netId,ob.transform.position,
                        ob.transform.localPosition, ob.transform.rotation, ob.transform.localRotation);
            help_hide(ob);
        }
        list.Clear();
        
        d.GetComponent<Deck>().Shuffle();
    }

    public void Level()
    {
        string par;
        if (NetworkServer.active)
        {
            par = "Clock";
        }
        else
        {
            par = "_Clock";
        }
        GameObject clock = GameObject.Find(par);
        Transform[] arr = clock.GetComponentsInChildren<Transform>();

        if (arr.Length == 1) return;

        string wr;
        if (NetworkServer.active)
        {
            wr = "WaitingRoom";
        }
        else
        {
            wr = "_WaitingRoom";
        }
        if (player.wr.Count != 0) {
            help_hide(player.wr[player.wr.Count - 1]);
        }
        
        for (int i = 1; i < arr.Length; i++)
        {
            Transform c = arr[i];
            GameObject ob = c.gameObject;
            player.wr.Add(c.gameObject);
            player.Cmd_Sync(wr, ob.GetComponent<SyncCard>().netId, ob.transform.position,
                        ob.transform.localPosition, ob.transform.rotation, ob.transform.localRotation);
            help_hide(c.gameObject);

        }
        help_show(player.wr[player.wr.Count - 1]);
    }

    private void help_hide(GameObject ob)
    {
        SyncCard c = ob.GetComponent<SyncCard>();
        player.Cmd_Hide(c.netId);
    }

    private void help_show(GameObject ob)
    {
        SyncCard c = ob.GetComponent<SyncCard>();
        player.Cmd_Show(c.netId);
    }
}
