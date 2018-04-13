using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Security.Cryptography;
using UnityEngine.Networking;
/// <summary>
/// subclass of <c>Dropzone</c> to handle deck management 
/// </summary>
public class Deck : DropZone
{
    Text tx;

    public GameObject listButton = null;
    public GameObject Decklist = null;
    public GameObject scroll = null;
    public List<GameObject> d = null;
    Player player = null;

    void Start()
    {
        if (this.name[0] == '_')
        {   
            GameObject go = GameObject.Find("_DeckCount");
            tx = go.GetComponent<Text>();
        }
        else
        {
            GameObject go = GameObject.Find("DeckCount");
            tx = go.GetComponent<Text>();
        }
        if (this.name[0] != '_')
        {
            listButton = GameObject.Find("DeckClose");
            listButton.SetActive(false);
        }
    }




    private void Update()
    {
        if(player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
        if (d.Count == 0)
        {
            d = player.deck;
        }
        Draggable[] arr = this.GetComponentsInChildren<Draggable>(true);
        tx.text = arr.Length +" cards";
    }

    public override void OnDrop(PointerEventData eventData)
    {

        if (!drop)
        {
            return;
        }

        Draggable drag = eventData.pointerDrag.GetComponent<Draggable>();
        d.Add(eventData.pointerDrag);

        if (d.Count > 1)
        {
            help_hide(d[d.Count - 2]);
        }

        drag.parentToRet = this.transform;

    }

    public void Close()
    {
        Transform p = Decklist.transform.GetChild(0);
        Transform[] arr = p.GetComponentsInChildren<Transform>();
        Transform deck;
        if (NetworkServer.active)
        {
            deck = this.transform;
        }
        else
        {
            deck = GameObject.Find("_Deck").transform;
        }
        for (int i = 1; i < arr.Length; i++)
        {
            Transform c = arr[i];
            Debug.Log(c.name);
            //c.gameObject.SetActive(false);
            SyncCard syn = c.gameObject.GetComponent<SyncCard>();
            c.gameObject.GetComponent<Image>().sprite = syn.back;
            help_hide(c.gameObject);
            d.Add(c.gameObject);
            c.SetParent(deck);
            RectTransform rt = (RectTransform)c;
            rt.sizeDelta = new Vector2(40f, 56f);
        }
        Destroy(Decklist);
        Destroy(scroll);
        listButton.SetActive(false);
        if (d.Count != 0)
        {
            d[d.Count - 1].SetActive(true);
            help_show(d[d.Count - 1]);
        }
    }

    public void List()
    {
        Transform info = GameObject.Find("Info").transform;
        listButton.SetActive(true);
        Decklist = (GameObject)Instantiate(Resources.Load("ScrollPanel"), info);
        scroll = (GameObject)Instantiate(Resources.Load("Scrollbar"), info);

        GameObject dest = GameObject.Find("ListWithCards");

        foreach (GameObject t in d)
        {
            t.SetActive(true);
            SyncCard syn = t.GetComponent<SyncCard>();
            t.GetComponent<Image>().sprite = syn.front;
            t.transform.SetParent(dest.transform);
        }
        d.Clear();
    }

    public void SendToBottom()
    {
        if (d.Count > 1)
        {
            GameObject toMove = d[d.Count - 1];
            d.RemoveAt(d.Count - 1);
            d.Insert(0, toMove);
            //toMove.SetActive(false);
            player.Cmd_Bot(toMove.GetComponent<SyncCard>().netId);
            help_hide(toMove);
            player.Cmd_Flip(d[d.Count - 1].GetComponent<SyncCard>().netId);
            help_show(d[d.Count - 1]);
            GameObject text = GameObject.Find("ChatText");
            Chat t = text.GetComponent<Chat>();
            t.Output("Sent top card of deck to bottom.");
        }
    }

    public void Shuffle()
    {
        Debug.Log("Shuffling");
        //d[d.Count - 1].SetActive(false);
        help_hide(d[d.Count - 1]);
        Shuffle<GameObject>(d);
        foreach(GameObject go in d)
        {
            go.transform.SetAsLastSibling();
        }
        player.Cmd_Flip(d[d.Count - 1].GetComponent<SyncCard>().netId);
        help_show(d[d.Count - 1]);
        GameObject text = GameObject.Find("ChatText");
        Chat t = text.GetComponent<Chat>();
        t.Output("Deck shuffled");
    }

    private static void Shuffle<T>(IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (System.Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
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
