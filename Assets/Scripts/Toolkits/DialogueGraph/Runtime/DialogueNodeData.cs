using System;
using System.Collections.Generic;
using UnityEngine;

namespace GHSDialogue
{
    [Serializable]
    public class DialogueNodeData
    {
        public string GUID;
        public string DialogueText;
        public string TalkerName;
        public Vector2 Position;
        public List<string> PortNames;
    } 
}