using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
/// <summary>
/// Class extending NetworkBehaviour to handle Player ownership (TODO) and Client-Host communications
/// </summary>
public class Player : NetworkBehaviour {

    public List<GameObject> deck = new List<GameObject>();
    public List<GameObject> wr = new List<GameObject>();
    public List<GameObject> stock = new List<GameObject>();

    public GameObject WRMenu;
    public GameObject DeckMenu;
    public GameObject StockMenu;
    public GameObject MemMenu;
    public GameObject RotateMenu;

    void Start() {
        if (this.isLocalPlayer)
        {
            this.name = "Player";
            WRMenu = GameObject.Find("WRMenu");
            DeckMenu = GameObject.Find("DeckMenu");
            StockMenu = GameObject.Find("StockMenu");
            MemMenu = GameObject.Find("MemMenu");
            RotateMenu = GameObject.Find("RotateMenu");

            WRMenu.SetActive(false);
            DeckMenu.SetActive(false);
            StockMenu.SetActive(false);
            MemMenu.SetActive(false);
            RotateMenu.SetActive(false);
        }
        
    }

    [Command]
    public void Cmd_BotStock(NetworkInstanceId id)
    {
        Rpc_BotStock(id);
    }

    [ClientRpc]
    public void Rpc_BotStock(NetworkInstanceId id)
    {
        GameObject toMove = ClientScene.FindLocalObject(id);
        GameObject wr = GameObject.Find("WaitingRoom");
        toMove.transform.eulerAngles = new Vector3(0, 0, 0);
        toMove.transform.SetParent(wr.transform);
    }

    [Command]
    public void Cmd_Bot(NetworkInstanceId id)
    {
        Rpc_Bot(id);
    }

    [ClientRpc]
    public void Rpc_Bot(NetworkInstanceId id)
    {
        GameObject toMove = ClientScene.FindLocalObject(id);
        toMove.transform.SetAsFirstSibling();
    }

    [Command]
    public void Cmd_Sync(string par, NetworkInstanceId id, Vector3 t, Vector3 tl, Quaternion r, Quaternion rl)
    {
        Rpc_Sync(par, id, t, tl, r, rl);
    }

    [ClientRpc]
    public void Rpc_Sync(string par, NetworkInstanceId id, Vector3 t, Vector3 tl, Quaternion r, Quaternion rl)
    {
        GameObject ob = ClientScene.FindLocalObject(id);
        Transform parent = GameObject.Find(par).transform;

        RectTransform rt = (RectTransform)ob.transform;
        rt.sizeDelta = new Vector2(40f, 56f);

        ob.transform.SetParent(parent);

        if (!ob.gameObject.GetComponent<SyncCard>().hasAuthority)
        {
            ob.transform.localPosition = tl;
            ob.transform.localRotation = rl;
        }
        SyncCard sync = ob.GetComponent<SyncCard>();

        bool marker = false;
        try {marker = (parent.name.Substring(0, 6) == "Marker");}
        catch { marker = false; }
        if (parent.name == "Board" || parent.name == "Deck" ||
            parent.name == "_Deck" || parent.name == "Stock" ||
            parent.name == "_Stock" || parent.name == "Canvas" ||
            marker)
        {
            ob.GetComponent<Image>().sprite = sync.back;
        }
        else if (parent.name == "Hand" || parent.name == "_Hand")
        {
            if (this.hasAuthority)
            {
                ob.GetComponent<Image>().sprite = sync.front;
            }
            else
            {
                ob.GetComponent<Image>().sprite = sync.back;
            }
        }
        else
        {
            ob.GetComponent<Image>().sprite = sync.front;
        }

        ob.SetActive(true);
    }

    [Command]
    public void Cmd_Flip(NetworkInstanceId id)
    {
        Rpc_Flip(id);
    }

    [Command]
    public void Cmd_Hide(NetworkInstanceId id)
    {
        Rpc_Hide(id);
    }

    [Command]
    public void Cmd_Show(NetworkInstanceId id)
    {
        Rpc_Show(id);
    }

    [ClientRpc]
    public void Rpc_Flip(NetworkInstanceId id)
    {
        GameObject ob = ClientScene.FindLocalObject(id);
        ob.GetComponent<Image>().sprite = ob.GetComponent<SyncCard>().back;
    }

    [ClientRpc]
    public void Rpc_Hide(NetworkInstanceId id)
    {
        GameObject ob = ClientScene.FindLocalObject(id);
        ob.SetActive(false);
    }

    [ClientRpc]
    public void Rpc_Show(NetworkInstanceId id)
    {
        GameObject ob = ClientScene.FindLocalObject(id);
        ob.SetActive(true);
    }

    [Command]
    public void Cmd_Start_Host(string s, NetworkInstanceId pid)
    {
        GameObject d = GameObject.Find("Deck");
        GameObject c = (GameObject)Instantiate(Resources.Load("Card"), d.transform);
        NetworkServer.SpawnWithClientAuthority(c, ClientScene.FindLocalObject(pid));
        NetworkInstanceId id = c.GetComponent<SyncCard>().netId;
        Rpc_Start_Host(s, id);
        Rpc_Hide_Deck();
    }

    [Command]
    public void Cmd_Start_Client(string s, NetworkInstanceId pid)
    {
        GameObject d = GameObject.Find("_Deck");
        GameObject c = (GameObject)Instantiate(Resources.Load("Card"), d.transform);
        NetworkServer.SpawnWithClientAuthority(c, ClientScene.FindLocalObject(pid));
        NetworkInstanceId id = c.GetComponent<SyncCard>().netId;
        Rpc_Start_Client(s, id);
        Rpc_Hide_Deck();
    }

    [ClientRpc]
    public void Rpc_Start_Host(string s, NetworkInstanceId id)
    {
        GameObject d = GameObject.Find("Deck");
        GameObject ob = ClientScene.FindLocalObject(id);
        ob.transform.SetParent(d.transform);
        ob.transform.localScale = new Vector3(1, 1, 1);

        this.deck.Add(ob);
        Texture2D tex = (Texture2D)Resources.Load(s);
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        ob.GetComponent<SyncCard>().front = sp;

        Texture2D backTex = (Texture2D)Resources.Load("0");
        Sprite back = Sprite.Create(backTex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        ob.GetComponent<SyncCard>().back = back;

        ob.GetComponent<Image>().sprite = back;
    }

    [ClientRpc]
    public void Rpc_Start_Client(string s, NetworkInstanceId id)
    {
        GameObject d = GameObject.Find("_Deck");
        GameObject ob = ClientScene.FindLocalObject(id);
        ob.transform.SetParent(d.transform);
        ob.transform.localScale = new Vector3(1, 1, 1);

        this.deck.Add(ob);

        Texture2D tex = (Texture2D)Resources.Load(s);
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        ob.GetComponent<SyncCard>().front = sp;

        Texture2D backTex = (Texture2D)Resources.Load("0");
        Sprite back = Sprite.Create(backTex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        ob.GetComponent<SyncCard>().back = back;

        ob.GetComponent<Image>().sprite = back;
    }



    [ClientRpc]
    public void Rpc_Hide_Deck()
    {
        for (int i = 0; i < this.deck.Count - 1; i++)
        {
            this.deck[i].SetActive(false);
        }
    }
}
