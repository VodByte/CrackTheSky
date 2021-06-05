using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour, ITalkable
{
    private LinkedList<string> strList1 = new LinkedList<string>();
    private LinkedList<string> strList2 = new LinkedList<string>();
    private LinkedList<string> strList3 = new LinkedList<string>();
    private LinkedList<string> strList4 = new LinkedList<string>();

    private bool _isDialogueFinish = false;
    private LinkedListNode<string> curNode = null;
    private InteractiveableMark _mark;
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _mark = GetComponent<InteractiveableMark>();

        strList1.AddLast("……どうしよう");

        strList2.AddLast("<b>牛：</b>よく来てくれました、ぜひ助けてほしいことがあります");
        strList2.AddLast("<b>牛：</b>今まで使ったふいごが壊れていて、\n" +
            "どうしても風を送り、かまどを燃やせることができない");
        strList2.AddLast("<b>牛：</b>お願いです、魔法で助けてください！");

        strList3.AddLast("<b>牛：</b>お願いんだ");

        strList4.AddLast("<b>牛：</b>助かったぞ！");

        curNode = strList1.First;
    }

    private void Start()
    {
        StoryManager.OnGetForestLincense.AddListener(() =>
        {
            curNode = strList2.First;
            _mark.CurType = InteractiveType.Missionable;
        });

        StoryManager.OnSlovedWindMission.AddListener(() =>
            curNode = strList4.First);
    }

    public void Talk()
    {
        if (_mark.CurType != InteractiveType.None)
        {
            _mark.CurType = InteractiveType.None;
        }

        if (curNode.List != strList1)
        {
            _anim.SetBool("shouldStopThinking", true);
            FaceToPlayer();
        }

        var mng = DialogueBoxManager.Instance;
        if (_isDialogueFinish)
        {
            _isDialogueFinish = false;
            mng.CloseDiaolugeBox();

            return;
        }

        mng.UpdateText(curNode.Value, this.transform.position + new Vector3(0, 0.3f, 0));

        var tempNode = curNode.Next;
        if (tempNode == null)
        {
            if (curNode.List == strList2)
            {
                StoryManager.IsGetWindMission = true;
                curNode = strList3.First;
            }

            _isDialogueFinish = true;
        }
        else
        {
            curNode = tempNode;
        }
    }


    public void Surprise()
    {
        _anim.SetTrigger("OnSurprise");
        FaceToPlayer();
    }
    private void FaceToPlayer()
    {
        transform.rotation = Quaternion.LookRotation(
                        Player.Instance.Pos - transform.position,
                        Vector3.up);
    }
}