using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace GHSDialogue
{
    public class DialogueGraphView : GraphView
    {
        public readonly static string EntryNodeText = "E$n%tr%y P$o%in%t";
        public readonly static string NameOfNextPort = "NextPort";
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
        public readonly DialogueGraph _window;
        public readonly Blackboard _blackboard;
        public List<string> Talkers = new List<string> { " " }; // First elemnt always be " "

        public DialogueGraphView(DialogueGraph window)
        {
            _window = window;
            
            // Manipulators
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.InsertAction(
                    0,
                    "Add Node",
                    action =>
                    {
                        var pos = this.contentViewContainer.WorldToLocal(
                            action.eventInfo.localMousePosition);
                        var node = CreateDialogueNode("Dialogue Node", pos);
                        AddElement(node);
                        var textField = node.Q<TextField>();
                        // Edit mode immediately
                        textField.Children().ToList()[0].Focus();
                    });
            }));
            // Zoom setup
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            /// Background setup
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraphStyles"));
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            /// Create blackboard
            _blackboard = new Blackboard(this) { title = "Talkers" };
            _blackboard.contentContainer.style.flexWrap = Wrap.Wrap;
            _blackboard.addItemRequested = bb =>
            {
                var container = new VisualElement();
                container.style.alignItems = Align.Center;
                container.style.flexDirection = FlexDirection.Row;
                var field = new BlackboardField() { text = "John Doe" };
                var bnt = new Button(() => RemoveTalker(container)) { text = "X" };
                container.Add(bnt);
                container.Add(field);
                bb.Add(container);
                Talkers.Add(field.text);
            };
            _blackboard.editTextRequested = (bb, element, arg) =>
            {
                var targetField = (BlackboardField)element;
                // Not allow repeate
                if (arg != targetField.text && Talkers.Contains(arg))
                {
                    EditorUtility.DisplayDialog("Error", "This  name already exists, please chose another one.",
                            "OK");
                    return;
                }
                var index = _blackboard.IndexOf(element.parent);
                if (index + 1 >= Talkers.Count) return;
                Talkers[index + 1] = arg;
                targetField.text = arg;
            };
            _blackboard.SetPosition(new Rect(10, 15, 200, 300));
            Add(_blackboard);
            // Create root node
            AddElement(CreateRootNode());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node && port.enabledSelf)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }
        //=============================================================================================
        //=============================================================================================
        public DialogueNode CreateDialogueNode(string dialogueText,
            Vector2? loc = null, string GUID = null, string talkerName = null)
        {
            var dialogueNode = new DialogueNode()
            {
                title = "Dialogue Node",
                DialogueText = dialogueText,
                GUID = GUID ?? Guid.NewGuid().ToString()
            };
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("NodeStyle"));

            /// Set up default ports: Input&Next
            var inputPort = CreatePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);
            AddNextPort(dialogueNode);

            /// Add choice port button
            var button = new Button(() => AddChoicePort(dialogueNode));
            button.text = "Add Choice";
            dialogueNode.titleButtonContainer.Add(button);

            /// Textfield for dialogue text
            var textField = new TextField(string.Empty) { multiline = true };
            textField.RegisterValueChangedCallback(evt =>
            {
                // Input text shouldnot be same with entry node text
                if (evt.newValue == EntryNodeText)
                {
                    textField.SetValueWithoutNotify("");
                    return;
                }
                dialogueNode.DialogueText = evt.newValue;
            });
            textField.RegisterCallback<FocusInEvent>(evt => Input.imeCompositionMode = IMECompositionMode.On);
            textField.RegisterCallback<FocusOutEvent>(evt => Input.imeCompositionMode = IMECompositionMode.Auto);
            textField.SetValueWithoutNotify(dialogueText);
            dialogueNode.mainContainer.Add(textField);

            /// Select talker
            var charSelector = new PopupField<string>(Talkers, 0);
            charSelector.style.alignSelf = Align.FlexEnd;
            charSelector.RegisterValueChangedCallback(evt =>
            {
                dialogueNode.TalkerName = evt.newValue;
            });
            dialogueNode.contentContainer.Add(charSelector);

            /// Setup
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(loc ?? Vector2.zero, DefaultNodeSize));
            AddElement(dialogueNode);

            return dialogueNode;
        }

        public void OverrideBlackboardInfo(List<TalkerData> data)
        {
            for (int i = 1; i < data.Count; i++)    // Skip first one, beacuse its " "
            {
                _blackboard.addItemRequested.Invoke(_blackboard);
                var bbField = _blackboard.contentContainer.ElementAt(i - 1).Q<BlackboardField>();
                bbField.text = data[i].Name;
            }
            Talkers = new List<string>(data.Select(x => x.Name));
        }

        public DialogueNode CreateDialogueNode(DialogueNodeData nodeData)
        {
            var tempNode = CreateDialogueNode(
                nodeData.DialogueText, nodeData.Position, nodeData.GUID);
            //tempNode.TalkerName = nodeData.TalkerName;
            var popuoField = tempNode.contentContainer.Q<PopupField<string>>();
            popuoField.value = Talkers.ElementAt(
                Talkers.FindIndex( x => x == nodeData.TalkerName));
            return tempNode;
        }

       public void RemoveTalker(VisualElement container)
        {
            // Check each node if holding this name
            nodes.ForEach(x =>
                {
                    var popField = x.contentContainer.Q<PopupField<string>>();
                    if (popField == null) return;
                    BlackboardField bbField = container.Q<BlackboardField>();
                    if (bbField.text == popField.value)
                    {
                        popField.value = Talkers[0];
                    }
                });
            // Remove from list
            Talkers.RemoveAt(container.parent.IndexOf(container) + 1);
            container.RemoveFromHierarchy();
            Talkers.ForEach(x => Debug.Log(x));
        }

        public void AddChoicePort(DialogueNode node, string overridePortName = "")
        {
            /// Disable default next port
            var firstPort = node.outputContainer.Q<Port>();
            bool isNextPort = firstPort.name == NameOfNextPort;
            if (isNextPort)
            {
                firstPort.SetEnabled(false);

                /// Disconnect and remove edge from next port
                if (firstPort.connected)
                {
                    var edge = edges.ToList().First(x => x.output == firstPort);
                    firstPort.DisconnectAll();
                    edge.input.Disconnect(edge);
                    RemoveElement(edge);
                }
            }

            var choicePort = CreatePort(node, Direction.Output);

            /// Remove default port lable
            var oldLable = choicePort.contentContainer.Q<Label>("type");
            choicePort.contentContainer.Remove(oldLable);

            /// HACK: Make space before circle mark.Otherwise line trigger will bais into textfield
            /// so that hard to drag line out
            choicePort.contentContainer.Add(new Label("   "));

            /// Decide port name
            var outputPortCount = node.outputContainer.Query("connector").ToList().Count;
            choicePort.portName = string.IsNullOrEmpty(overridePortName) ?
                $"Choice {outputPortCount}" : overridePortName;

            /// Add port input field & del button
            var textField = new TextField
            {
                name = string.Empty,
                value = choicePort.portName,
                multiline = true
            };
            textField.RegisterValueChangedCallback(evt => choicePort.portName = evt.newValue);
            textField.RegisterCallback<FocusInEvent>(evt => Input.imeCompositionMode = IMECompositionMode.On);
            textField.RegisterCallback<FocusOutEvent>(evt => Input.imeCompositionMode = IMECompositionMode.Auto);
            choicePort.contentContainer.Add(textField);
            var deleteBnt = new Button(() => RemovePort(node, choicePort)){text = "X"};
            choicePort.contentContainer.Add(deleteBnt);

            /// Setup
            node.outputContainer.Add(choicePort);
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        public void AddNextPort(DialogueNode node)
        {
            var nextPort = CreatePort(node, Direction.Output);
            nextPort.portName = "Next";
            nextPort.name = NameOfNextPort; // Next port defined by uielemet name
            node.outputContainer.Add(nextPort);
        }

        public void ConnectPorts(Port outport, Port inport)
        {
            var tempEdge = new Edge
            {
                output = outport,
                input = inport
            };

            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            this.Add(tempEdge);
        }

        private void RemovePort(DialogueNode node, Port port)
        {
            var targetEdges = edges.ToList().Where(x => x.output.node == port.node &&
                x.output.portName == port.portName);

            if (targetEdges.Any())
            {
                var edge = targetEdges.First();
                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }
            node.outputContainer.Remove(port);

            /// If donot exist any ports, enable default next port
            if (node.outputContainer.childCount == 1)
            {
                var onlyPort = node.outputContainer.Q<Port>();
                if (onlyPort.name == NameOfNextPort)
                {
                    onlyPort.SetEnabled(true);
                }
            }

            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private DialogueNode CreateRootNode()
        {
            var node = new DialogueNode()
            {
                title = "Start",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = EntryNodeText,
                EntryPoint = true
            };
            node.capabilities -= Capabilities.Deletable;
            node.styleSheets.Add(Resources.Load<StyleSheet>("NodeStyle"));
            AddNextPort(node);

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, DefaultNodeSize.x, DefaultNodeSize.y));
            return node;
        }

        private Port CreatePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
    } 
}