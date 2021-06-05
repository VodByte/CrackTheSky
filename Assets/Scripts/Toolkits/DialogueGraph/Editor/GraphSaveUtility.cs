using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GHSDialogue
{

    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;

        private List<Edge> _edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> _nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();
        private List<string> _talkerList => _targetGraphView.Talkers;

        public static GraphSaveUtility GetInstance(DialogueGraphView targetView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = targetView
            };
        }

        public void SaveGraph(string fileName)
        {
            // File already exits?
            if (System.IO.File.Exists($"Assets/Resources/{fileName}.asset"))
            {
                bool shouldOverwrite = EditorUtility.DisplayDialog("Overwrite existing file?",
                    "File with the same name already exists, overwrite the old one?",
                    "Yes", "Cancel");

                if (!shouldOverwrite) return;
            }

            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            /// Reserve talkers info
            foreach (var n in _talkerList)
            {
                dialogueContainer.TalkData.Add(new TalkerData { Name = n });
            }

            /// Reserve connect info
            var connectedEdges = _edges.Where(edge => edge.input.node != null).ToArray();

            foreach (var edge in connectedEdges)
            {
                var outputNode = edge.output.node as DialogueNode;
                var inputNode = edge.input.node as DialogueNode;
                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    TargetNodeGUID = inputNode.GUID,
                    BasePortIndex = outputNode.outputContainer.IndexOf(edge.output)
                });
            }

            /// Reserve node info. Make entry node be the first element forcely.
            foreach (var node in _nodes)
            {
                var tempNodeData = new DialogueNodeData
                {
                    GUID = node.GUID,
                    DialogueText = node.DialogueText,
                    TalkerName = node.TalkerName,
                    Position = node.GetPosition().position,
                    // Skip first port.Because first one always be default next port
                    PortNames = node.outputContainer.Query<Port>().ToList().Skip(1).Select(
                        x => x.portName).ToList()
                };

                if (tempNodeData.DialogueText == DialogueGraphView.EntryNodeText)
                {
                    dialogueContainer.DialogueNodeData.Insert(0, tempNodeData);
                }
                else
                {
                    dialogueContainer.DialogueNodeData.Add(tempNodeData);
                }
            }

            // Folder exists? If not, create it
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<DialogueContainer>(fileName);
            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found",
                    "Target dialogue graph file does not exists", "OK");
                return;
            }

            ClearGraph();
            CreateGraph();
        }

        private void CreateGraph()
        {
            /// Adjust root node.After ClearGraph, should only hold one root node
            _nodes[0].SetPosition(new Rect(
                _containerCache.DialogueNodeData[0].Position, _targetGraphView.DefaultNodeSize));

            /// Create blackboard first, and create talker list that store in dialogue graph view
            _targetGraphView.OverrideBlackboardInfo(_containerCache.TalkData);

            /// Create nodes BUT ROOTNODE
            foreach (var nodeData in _containerCache.DialogueNodeData.Skip(1))
            {
                /// Create noes
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData);
                /// Set up ports.Notice first port alawys be default next port
                for (int i = 0; i < nodeData.PortNames.Count; i++)
                {
                    _targetGraphView.AddChoicePort(tempNode, nodeData.PortNames[i]);
                }
            }

            /// Connect nodes
            foreach (var baseNode in _nodes)
            {
                // Gather all linkdata output from same node
                var edges = _containerCache.NodeLinks.Where(
                    link => link.BaseNodeGUID == baseNode.GUID).ToList();
                // If cannot find any edge. For example last node
                if (!edges.Any()) continue;
                var ports = baseNode.outputContainer.Query<Port>().ToList();
                for (int i = 0; i < edges.Count; i++)
                {
                    var targetNode = _nodes.First(node => node.GUID == edges[i].TargetNodeGUID);
                    _targetGraphView.ConnectPorts(ports[edges[i].BasePortIndex], (Port)targetNode.inputContainer[0]);
                }
            }
        }

        private void ClearGraph()
        {
            // Assign entry node GUID
            _nodes.Find(node => node.EntryPoint).GUID = _containerCache.DialogueNodeData[0].GUID;

            /// Del others
            foreach (var node in _nodes)
            {
                if (node.EntryPoint) continue;

                // Remove edge
                _edges.Where(edge => edge.input.node == node).ToList().ForEach(
                    edge => _targetGraphView.RemoveElement(edge));

                // Then remove node
                _targetGraphView.RemoveElement(node);
            }
            _targetGraphView._blackboard.Clear();
        }
    } 
}