using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// subclass of <c>Dropzone</c> to handle marker management 
/// </summary>
public class Marker : DropZone {
    Player player = null;

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

        Draggable drag = eventData.pointerDrag.GetComponent<Draggable>();
        list.Add(eventData.pointerDrag);

        if (list.Count != 0)
        {
            help_hide(list[list.Count - 1]);
        }

        drag.parentToRet = this.transform;

    }

    private void help_hide(GameObject ob)
    {
        SyncCard c = ob.GetComponent<SyncCard>();
        player.Cmd_Hide(c.netId);
    }
}
