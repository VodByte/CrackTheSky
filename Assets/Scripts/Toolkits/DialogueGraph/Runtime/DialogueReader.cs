using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GHSDialogue
{
    public class DialogueReader
    {
        private class Dialogue
        {
            public string TalkerName;
            public string Text;
            public List<LinkedListNode<Dialogue>> Nexts;
        }

        private readonly DialogueContainer _data;
        private LinkedListNode<Dialogue> _currentNode;
        private bool _waitChoice = false;

        private DialogueReader(DialogueContainer dataContainer)
        {
            _data = dataContainer;

            // Load data and create link lists
            var line = new LinkedList<Dialogue>();
            ExtractDialogueLines(dataContainer, 0, ref line);
            // Skip first one because first one always be entry point
            _currentNode = line.First.Next;
        }

        public static DialogueReader GetInstance(DialogueContainer dataContainer)
        {
            if (dataContainer == null)
            {
                Debug.LogError("Null paramter");
                return null;
            }
            return new DialogueReader(dataContainer);
        }
        ///------------------------------------------------------------------------------------------
        ///------------------------------------------------------------------------------------------
        /// <summary>
        /// Get next dialogue. Paramter count from 1, not 0
        /// </summary>
        /// <param name="choiceIndex">Start with 1</param>
        /// <returns>Dialogue text list. If element cout more than one, that 
        /// means holding choice  text. Null for nothing </returns>
        public List<string> ReadNext(int? choiceIndex = null)
        {
            System.Func<Dialogue, List<string>> ExtractNodeText = (d) =>
            {
                var stringList = new List<string>() { d.TalkerName, d.Text };
                if (d.Nexts != null)
                {
                    for (int i = 0; i < d.Nexts.Count; i++)
                    {
                        stringList.Add(d.Nexts[i].Value.Text);
                    }
                }

                return stringList;
            };
            ///////////////////////////////////////////////
            if (_currentNode == null) return null;

            if (_waitChoice)
            {
                if (choiceIndex == null)
                {
                    Debug.LogError("No choice input");
                }
                else
                {
                    _currentNode = _currentNode.Value.Nexts[(int)choiceIndex].Next;
                }
            }

            var texts = ExtractNodeText(_currentNode.Value);
            if (_currentNode.Value.Nexts == null)
            {
                _currentNode = _currentNode.Next;
                _waitChoice = false;
            }
            else
            {
                _waitChoice = true;
            }

            return texts;
        }

        private void ExtractDialogueLines(DialogueContainer data, int index, ref LinkedList<Dialogue> line)
        {
            while (index != -1)
            {
                /// Save current node data into dialogue line
                var tempDialogue = new Dialogue()
                {
                    TalkerName = data.DialogueNodeData[index].TalkerName,
                    Text = data.DialogueNodeData[index].DialogueText
                };
                line.AddLast(tempDialogue);

                /// Node have choice ports
                int portCount = data.DialogueNodeData[index].PortNames.Count;
                if (portCount != 0)
                {
                    for (int i = 0; i < portCount; i++)
                    {
                        var newLine = new LinkedList<Dialogue>();
                        newLine.AddLast(new Dialogue
                        {
                            Text = data.DialogueNodeData[index].PortNames[i]
                        });
                        if (tempDialogue.Nexts == null)
                        {
                            tempDialogue.Nexts = new List<LinkedListNode<Dialogue>>();
                        }
                        tempDialogue.Nexts.Add(newLine.First);
                        ExtractDialogueLines(data, GetNextIndex(data.DialogueNodeData[index], i + 1), ref newLine);
                    }
                }

                index = GetNextIndex(data.DialogueNodeData[index]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNode">Node</param>
        /// <param name="choiceIndex">Start with 1</param>
        /// <returns>-1 means null</returns>
        private int GetNextIndex(DialogueNodeData currentNode, int? choiceIndex = null)
        {
            /// Find the edge which come from current node and port
            NodeLinkData linkToNext;
            if (choiceIndex != null)
            {
                linkToNext = _data.NodeLinks.Find(
                    x => x.BaseNodeGUID == currentNode.GUID &&
                    x.BasePortIndex == choiceIndex);
            }
            else
            {
                linkToNext = _data.NodeLinks.Find(
                    x => x.BaseNodeGUID == currentNode.GUID);
            }

            if (linkToNext != null)
            {
                return _data.DialogueNodeData.FindIndex(
                    x => x.GUID == linkToNext.TargetNodeGUID);
            }
            else
            {
                return -1;
            }
        }
    } 
}