using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour, ITalkable
{
    [SerializeField] GameObject[] _guards = null;
    [SerializeField] Vector3 _guardsPostPos = new Vector3();
    [SerializeField] GameObject _gateBlockWall = null;

    private LinkedList<string> strList1 = new LinkedList<string>();
    private LinkedList<string> strList1_1 = new LinkedList<string>();
    private LinkedList<string> strList2 = new LinkedList<string>();
    private LinkedList<string> strList2_1 = new LinkedList<string>();
    LinkedListNode<string> curNode = null;

    private bool _isDialogueFinish = false;
    private InteractiveableMark _mark;
    private Animator _anim;

    public void Awake()
    {
        _mark = GetComponent<InteractiveableMark>();
        _anim = GetComponent<Animator>();

        strList1.AddLast("<b>犬村長：</b>待っていました、私はここの村長です");
        strList1.AddLast("<b>犬村長：</b>君の目的は知っているが、\nでも言えばそれは私たちが知ったことではない");
        strList1.AddLast("<b>犬村長：</b>君が見た通り、ここの川なぜか枯れておる、\n村のみんなこれのせいで今大変なんだ");
        strList1.AddLast("<b>犬村長：</b>魔女の力で解決してくれるなら、\n交換として森に入る許可を下そう{flag1}");

        strList1_1.AddLast("<b>犬村長：</b>頼むぞ");

        strList2.AddLast("<b>犬村長：</b>上流にあんなにでかいのがいたなんて…不思議なことだね");
        strList2.AddLast("<b>犬村長：</b>魔女さん、君に感謝します、\nこれをもらっていてください{flag2}");

        strList2_1.AddLast("<b>犬村長：</b>感謝します");

        curNode = strList1.First;
    }

    public void Start()
    {
        _mark.CurType = InteractiveType.Missionable;
        StoryManager.OnSlovedRiverEvent.AddListener(() => curNode = strList2.First);
    }

    public void Talk()
    {
        if (_mark.CurType != InteractiveType.None)
        {
            _mark.CurType = InteractiveType.None;
        }

        var mng = DialogueBoxManager.Instance;
        if (_isDialogueFinish)
        {
            _isDialogueFinish = false;
            mng.CloseDiaolugeBox();

            if (curNode.List == strList1)
            {
                curNode = strList1_1.First;
            }
            else if (curNode.List == strList2)
            {
                curNode = strList2_1.First;
            }

            return;
        }

        mng.UpdateText(curNode.Value, this.transform.position + new Vector3(0, 0.2f, 0));
        _anim.SetTrigger("OnTalk");

        var tempNode = curNode.Next;
        if (tempNode == null)
        {
            // If reach flag1, give new mission
            if (curNode.Value.EndsWith("{flag1}"))
            {
                StoryManager.IsGetRiverMission = true;
            }
            else if (curNode.Value.EndsWith("{flag2}"))
            {
                StoryManager.IsGetForestLincense = true;
                foreach (var i in _guards)
                {
                    i.transform.position = _guardsPostPos;
                    _gateBlockWall.GetComponent<BoxCollider>().isTrigger = true;
                }
            }
            _isDialogueFinish = true;
            curNode = curNode.List.First;
        }
        else
        {
            curNode = tempNode;
        }
    }
}