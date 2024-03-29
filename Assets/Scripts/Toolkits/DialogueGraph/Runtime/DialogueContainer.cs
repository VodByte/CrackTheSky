﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GHSDialogue
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        public List<TalkerData> TalkData = new List<TalkerData>();
    } 
}