using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGuard : MonoBehaviour, ITalkable
{
    private LinkedList<string> strList1 = new LinkedList<string>();
    private LinkedList<string> strList2 = new LinkedList<string>();
    LinkedListNode<string> curNode = null;

    private bool _isDialogueFinish = false;

    private void Awake()
    {
        strList1.AddLast("<b>門：</b>まず許可貰って来ないと、\n僕は開かないからね…");
        strList2.AddLast("<b>門：</b>許可貰ったみたいね…");
        curNode = strList1.First;
    }

    private void Start()
    {
        StoryManager.OnGetForestLincense.AddListener(
            () => curNode = strList2.First);
    }

    public void Talk()
    {
        var mng = DialogueBoxManager.Instance;
        if (_isDialogueFinish)
        {
            //_anim.SetBool("isThinking", false);
            _isDialogueFinish = false;
            mng.CloseDiaolugeBox();

            return;
        }

        mng.UpdateText(curNode.Value, this.transform.position);

        var tempNode = curNode.Next;
        if (tempNode == null)
        {
            _isDialogueFinish = true;
            curNode = curNode.List.First;
        }
        else
        {
            curNode = tempNode;
        }
    }
}