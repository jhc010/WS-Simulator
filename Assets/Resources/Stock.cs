using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Security.Cryptography;
using UnityEngine.Networking;
/// <summary>
/// subclass of <c>Dropzone</c> to handle stock management 
/// </summary>
public class Stock : DropZone
{
    Text text;
    Player player = null;

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
        }
        if (list.Count == 0)
        {
            list = player.stock;
        }

        if (this.name[0] == '_')
        {
            if (text == null)
            {
                GameObject go = GameObject.Find("_StockCount");
                text = go.GetComponent<Text>();
            }
            Draggable[] arr = this.GetComponentsInChildren<Draggable>(true);
            text.text = arr.Length + " cards";
        }
        else
        {
            if (text == null)
            {
                GameObject go = GameObject.Find("StockCount");
                text = go.GetComponent<Text>();
            }
            Draggable[] arr = this.GetComponentsInChildren<Draggable>(true);
            text.text = arr.Length + " cards";
        }
    }

    public void BottomToWr()
    {
        GameObject toMove = list[0];

        GameObject wr;
        if (NetworkServer.active)
        {
            wr = GameObject.Find("WaitingRoom");
        }
        else
        {
            wr = GameObject.Find("_WaitingRoom");
        }

        Wr waiting = wr.GetComponent<Wr>();
        waiting.list.Add(toMove);
        list.RemoveAt(0);
        //toMove.SetActive(true);
        player.Cmd_BotStock(toMove.GetComponent<SyncCard>().netId);
        help_show(toMove);
        
        if (waiting.list.Count > 1)
        {
            //waiting.list[waiting.list.Count - 2].SetActive(false);
            help_hide(waiting.list[waiting.list.Count - 2]);
        }

        GameObject ob = waiting.list[waiting.list.Count - 1];
        player.Cmd_Sync(wr.name, ob.GetComponent<SyncCard>().netId, ob.transform.position,
                        ob.transform.localPosition, ob.transform.rotation, ob.transform.localRotation);
    }

    public override void OnDrop(PointerEventData eventData)
    {

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

    public void Shuffle()
    {
        Debug.Log("Shuffling");
        //list[list.Count - 1].SetActive(false);
        help_hide(list[list.Count - 1]);
        Shuffle<GameObject>(list);
        foreach (GameObject go in list)
        {
            go.transform.SetAsLastSibling();
        }
        //list[list.Count - 1].SetActive(true);
        player.Cmd_Flip(list[list.Count - 1].GetComponent<SyncCard>().netId);
        help_show(list[list.Count - 1]);
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
