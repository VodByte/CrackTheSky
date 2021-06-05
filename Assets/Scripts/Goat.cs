using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Goat : Character, ITalkable
{
    public bool CanTalk = true;

    [SerializeField] private GameObject _house = null;
    [SerializeField] private Vector3 _housePosBias = new Vector3();

    private bool _isDialogueFinish = false;
    private bool _afterFlag1 = false;
    private Animator _anim;
    private InteractiveableMark _mark;

    private LinkedList<string> strList1 = new LinkedList<string> ();
    private LinkedList<string> strList1_1 = new LinkedList<string> ();
    private LinkedList<string> strList1_2 = new LinkedList<string> ();
    private LinkedList<string> strList2 = new LinkedList<string> ();
    private LinkedList<string> strList3 = new LinkedList<string> ();

    private LinkedListNode<string> curNode = null;

    public void Awake()
    {
        _anim = GetComponent<Animator>();
        _mark = GetComponent<InteractiveableMark>();

        strList1.AddLast("<b>羊：</b>ちょっとあそこの君、その格好見たら、魔女だろう？{flag1}");

        strList1_1.AddLast("<b>羊：</b>こいつを何とかしてくれー、その魔法なんやらで、\n" +
            "どうしても言う事聞いてくれないから");
        strList1_1.AddLast("<b>燃え上がる屋敷：</b><color=\"red\">そっちこそ掃除も片付けも全然しないくせに、\n" +
            "もうあんたなんざ入れない!!");

        strList1_2.AddLast("<b>羊：</b>えー？魔女なのに魔法ひとつも出せないの？情けねぇなー");
        strList1_2.AddLast("<b>羊：</b>ここに来る途中に、<color=\"blue\">昔の魔女小屋</color>あるから、\n" +
            "早くあそこ回ってきな");

        strList2.AddLast("<b>羊：</b>あんた弱そうにみえるけどやるじゃん、またねー");

        strList3.AddLast("<b>羊：</b>土足で上がっただけなのにぃぃぃぃ！");

        curNode = strList1.First;
    }

    public void Start()
    {
        _mark.CurType = InteractiveType.Missionable;

        StoryManager.OnGetWaterMagic.AddListener(() =>
            {
                if (_afterFlag1)
                {
                    curNode = strList1_1.First;
                }
            });

        StoryManager.OnSlovedHouseMission.AddListener(() => curNode = strList2.First);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Is conversion over</returns>
    public void Talk()
    {
        if (!CanTalk) return;

        if (_mark.CurType != InteractiveType.None)
        {
            _mark.CurType = InteractiveType.None;
        }

        var mng = DialogueBoxManager.Instance;
        if (_isDialogueFinish)
        {
            _anim.SetBool("isThinking", false);
            _isDialogueFinish = false;
            mng.CloseDiaolugeBox();

            // Finish all dialogues, start timeline
            if (curNode.List == strList2)
            {
                GetComponent<PlayableDirector>().Play();
                curNode = strList3.First;
            }
            
            return;
        }

        // Speak anim
        _anim.SetBool("isThinking", true);

        ///Check who is talking
        if (curNode.Value.Contains("燃え上がる屋敷"))
        {
            mng.UpdateText(curNode.Value, _house.transform.position + _housePosBias);
        }
        else
        {
            mng.UpdateText(curNode.Value, this.transform.position);
        }

        /// Move to next node.If already the last one,
        /// return to first one.
        var tempNode = curNode.Next;
        if (tempNode == null)
        {
            // If reach flag1, get next path
            if (!_afterFlag1 && curNode.Value.EndsWith("{flag1}"))
            {
                if (StoryManager.IsHoldWaterMagic)
                {
                    curNode = strList1_1.First;
                }
                else
                {
                    curNode = strList1_2.First;
                }

                StoryManager.IsGetHouseMission = true;
                _afterFlag1 = true;
                return;
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