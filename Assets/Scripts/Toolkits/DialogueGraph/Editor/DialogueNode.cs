using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

public class DialogueNode : Node
{
	public string GUID;
	public string DialogueText;
	public bool EntryPoint = false;
	public string TalkerName;
}