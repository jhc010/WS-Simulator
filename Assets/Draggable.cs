using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler {

    public Transform parentToRet = null;
    public Transform canvas = null;

    bool draggable = true;
    GameObject WRMenu;
    GameObject DeckMenu;
    GameObject StockMenu;
    GameObject MemMenu;
    GameObject RotateMenu;
    public Player player;
    bool RotateMenuState = false;
    bool MemMenuState = false;
    bool StockMenuState = false;
    bool WRMenuState = false;
    bool DeckMenuState = false;


    private void Start()
    {

        GameObject go = GameObject.Find("RotateMenu");
        if (go != null)
        {
            go.SetActive(false);
        }
        player = GameObject.Find("Player").GetComponent<Player>();
        WRMenu = player.WRMenu;
        DeckMenu = player.DeckMenu;
        StockMenu = player.StockMenu;
        MemMenu = player.MemMenu;
        RotateMenu = player.RotateMenu;

        canvas = GameObject.Find("Canvas").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        if (Input.GetMouseButton(0) && this.gameObject.GetComponent<SyncCard>().hasAuthority)
        {
            draggable = true;
        }
        else
        {
            draggable = false;
        }
        if (draggable)
        {
            parentToRet = canvas;
            bool marker = false;
            // Check if marker
            try { marker = (this.transform.parent.name.Substring(0, 6) == "Marker"); }
            catch { marker = false; }
            // Should maybe change to switch statement sometime. Action on dragged card based on where it came from
            if (this.transform.parent.name == "WaitingRoom" || this.transform.parent.name == "_WaitingRoom")
            {
                Wr waiting = this.transform.parent.gameObject.GetComponent<Wr>();
                List<GameObject> list = waiting.list;
                list.RemoveAt(list.Count - 1);
                if (list.Count > 0)
                {
                    //waiting.list[waiting.list.Count - 1].SetActive(true);
                    help_show(list[list.Count - 1]);
                }
            }
            else if (this.transform.parent.name == "Deck" || this.transform.parent.name == "_Deck")
            {
                Deck deck = this.transform.parent.gameObject.GetComponent<Deck>();
                List<GameObject> list = deck.d;
                list.RemoveAt(list.Count - 1);
                if (list.Count > 0)
                {
                    //player.deck[player.deck.Count - 1].SetActive(true);
                    help_flip(list[list.Count - 1]);
                    help_show(list[list.Count - 1]);
                }
            }
            else if (this.transform.parent.name == "Stock" || this.transform.parent.name == "_Stock")
            {
                this.transform.Rotate(new Vector3(0, 0, -90));
                Stock stock = this.transform.parent.gameObject.GetComponent<Stock>();
                List<GameObject> list = stock.list;
                list.RemoveAt(list.Count - 1);
                if (stock.list.Count > 0)
                {
                    help_flip(list[list.Count - 1]);
                    help_show(list[list.Count - 1]);
                }
            }
            else if (this.transform.parent.name == "Memory" || this.transform.parent.name == "_Memory")
            {
                Memory mem = this.transform.parent.gameObject.GetComponent<Memory>();
                List<GameObject> list = mem.list;
                list.RemoveAt(list.Count - 1);
                if (mem.list.Count > 0)
                {
                    help_show(list[list.Count - 1]);
                }
            }
            else if (marker)
            {
                Marker mark = this.transform.parent.gameObject.GetComponent<Marker>();
                List<GameObject> list = mark.list;
                list.RemoveAt(list.Count - 1);
                if (mark.list.Count > 0)
                {
                    help_flip(list[list.Count - 1]);
                    help_show(list[list.Count - 1]);
                }
            }
            else if (this.transform.parent.name == "Level" || this.transform.parent.name == "_Level")
            {
                this.transform.Rotate(new Vector3(0, 0, -90));
            }
            else if (this.transform.parent.name == "CX" || this.transform.parent.name == "_CX")
            {
                this.transform.Rotate(new Vector3(0, 0, -90));
            }

            RectTransform rt = (RectTransform)this.transform;
            rt.sizeDelta = new Vector2(40f, 56f);
            this.transform.rotation = Quaternion.identity;

            this.transform.SetParent(canvas);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        // If u can drag it, have it follow the mouse
        if (draggable)
        {
            this.transform.position = eventData.position;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggable)
        {
            // When dropped, action based on where card was dropped.
            if (parentToRet == null)
            {
                parentToRet = canvas;
                Debug.Log("No parent to return to, giving to canvas");
            }
            else if (parentToRet.name == "Info" || parentToRet.name == "Block")
            {
                this.transform.position = canvas.position - new Vector3(-100,50,0);
                parentToRet = GameObject.Find("Board").transform;
                Debug.Log("RESET");
            }

            else if (parentToRet.name == "Level" || parentToRet.name == "_Level")
            {
                Debug.Log("Drop to level");
                this.transform.Rotate(new Vector3(0, 0, 90));
            }

            else if (parentToRet.name == "CX")
            {
                try { GameObject.Find("CX").transform.GetChild(0); }
                catch
                {
                    this.transform.Rotate(new Vector3(0, 0, 90));
                    player.Cmd_Sync(parentToRet.name, this.GetComponent<SyncCard>().netId, this.transform.position,
                    this.transform.localPosition, this.transform.rotation, this.transform.localRotation);
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                    return;
                }
                this.transform.position = canvas.position - new Vector3(-100, 50, 0);
                this.transform.SetParent(canvas);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }

            else if (parentToRet.name == "_CX")
            {
                Debug.Log("Placed in CX Zone");
                try { GameObject.Find("_CX").transform.GetChild(0); }
                catch
                {
                    this.transform.Rotate(new Vector3(0, 0, 90));
                    player.Cmd_Sync(parentToRet.name, this.GetComponent<SyncCard>().netId, this.transform.position,
                        this.transform.localPosition, this.transform.rotation, this.transform.localRotation);
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                    return;
                }
                this.transform.position = canvas.position - new Vector3(0, 50, 0);
                this.transform.SetParent(canvas);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }

            else if (parentToRet.name == "Stock" || parentToRet.name == "_Stock")
            {
                Debug.Log("Placed in Stock");
                this.transform.Rotate(new Vector3(0, 0, 90));
            }

            this.transform.SetParent(parentToRet);

            player.Cmd_Sync(parentToRet.name, this.GetComponent<SyncCard>().netId, this.transform.position,
                        this.transform.localPosition, this.transform.rotation, this.transform.localRotation);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            Debug.Log("NOT DRAGGABLE");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Right click menu handling
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            WRMenu.SetActive(false);
            DeckMenu.SetActive(false);
            StockMenu.SetActive(false);
            StockMenu.transform.eulerAngles = new Vector3(0, 0, 0);
            MemMenu.SetActive(false);
            RotateMenu.SetActive(false);
            RotateMenu.transform.eulerAngles = new Vector3(0, 0, 0);
            
            if (this.transform.parent.name == "WaitingRoom" || this.transform.parent.name == "_WaitingRoom")
            {
                WRMenuState = true;
                WRMenu.transform.rotation = Quaternion.identity;
                WRMenu.SetActive(true);
                WRMenu.transform.position = Input.mousePosition + new Vector3(48,-16,0);
                WRMenu.transform.SetParent(this.transform);
                //Debug.Log("Open Menu");
            }
            else if (this.transform.parent.name == "Deck" || this.transform.parent.name == "_Deck")
            {
                DeckMenuState = true;
                DeckMenu.transform.rotation = Quaternion.identity;
                DeckMenu.SetActive(true);
                DeckMenu.transform.position = Input.mousePosition + new Vector3(15, -32, 0);
                DeckMenu.transform.SetParent(this.transform);
                //Debug.Log("Open Menu");
            }
            else if (this.transform.parent.name == "Stock" || this.transform.parent.name == "_Stock")
            {
                StockMenuState = true;
                StockMenu.transform.rotation = Quaternion.identity;
                StockMenu.SetActive(true);
                StockMenu.transform.position = Input.mousePosition + new Vector3(15, -22, 0);
                StockMenu.transform.SetParent(this.transform);
                //Debug.Log("Open Menu");
            }
            else if (this.transform.parent.name == "Memory" || this.transform.parent.name == "_Memory")
            {
                MemMenuState = true;
                MemMenu.transform.rotation = Quaternion.identity;
                MemMenu.SetActive(true);
                MemMenu.transform.position = Input.mousePosition + new Vector3(47, -16, 0);
                MemMenu.transform.SetParent(this.transform);
                //Debug.Log("Open Menu");
            }
            else if (this.transform.parent.name == "PlayerFrontRow" ||
                this.transform.parent.name == "PlayerBackRow" || 
                this.transform.parent.name == "_PlayerFrontRow" || 
                this.transform.parent.name == "_PlayerBackRow")
            {
                RotateMenuState = true;
                RotateMenu.transform.rotation = Quaternion.identity;
                RotateMenu.SetActive(true);
                RotateMenu.transform.position = Input.mousePosition + new Vector3(18, -32, 0);
                RotateMenu.transform.SetParent(this.transform);
                //Debug.Log("Open Menu");
            }
        }
    }

    
    
    public void OnGUI()
    {
        // Right click menu closing routines
        if (Input.GetMouseButtonDown(0)) {
            if (WRMenuState)
            {
                StartCoroutine(RemoveWRMenu(0.1f));
            }
            if (DeckMenuState)
            {
                StartCoroutine(RemoveDeckMenu(0.1f));
            }
            if (StockMenuState)
            {
                StartCoroutine(RemoveStockMenu(0.1f));
            }
            if (MemMenuState)
            {
                StartCoroutine(RemoveMemMenu(0.1f));
            }
            if (RotateMenuState)
            {
                StartCoroutine(RemoveRotateMenu(0.1f));
            }
        }
    }

    IEnumerator RemoveWRMenu(float time)
    {
        yield return new WaitForSeconds(time);
        WRMenuState = false;
        WRMenu.SetActive(false);
    }

    IEnumerator RemoveDeckMenu(float time)
    {
        yield return new WaitForSeconds(time);
        DeckMenuState = false;
        DeckMenu.SetActive(false);
    }

    IEnumerator RemoveStockMenu(float time)
    {
        yield return new WaitForSeconds(time);
        StockMenuState = false;
        StockMenu.SetActive(false);
    }

    IEnumerator RemoveMemMenu(float time)
    {
        yield return new WaitForSeconds(time);
        MemMenuState = false;
        MemMenu.SetActive(false);
    }

    IEnumerator RemoveRotateMenu(float time)
    {
        yield return new WaitForSeconds(time);
        RotateMenuState = false;
        RotateMenu.SetActive(false);
    }

    private void help_show(GameObject ob)
    {
        SyncCard c = ob.GetComponent<SyncCard>();
        player.Cmd_Show(c.netId);
    }

    private void help_flip(GameObject ob)
    {
        SyncCard c = ob.GetComponent<SyncCard>();
        player.Cmd_Flip(c.netId);
    }
}
