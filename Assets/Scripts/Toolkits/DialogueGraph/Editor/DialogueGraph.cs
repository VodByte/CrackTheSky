using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using GHSDialogue;
using System.Linq;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private MiniMap _map;
    private string _fileName = "New Narrative";
    private Rect _lastWindowPos;

    // A method can be called as a menu item using this
    // attribute which marked with static
    [MenuItem("G.H.S/Dialogue Graph")]
    public static void Open()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new UnityEngine.GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        CreateToolbar();
        CreateMiniMap();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void OnGUI()
    {
        /// HACK:Fix minimap wont pin at right place when resize graph view
        if (this.position != _lastWindowPos)
        {
            ResizeMiniMap();
        }

        _lastWindowPos = this.position;
    }
    //=============================================================================================
    //=============================================================================================
    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph",
            style = { flexGrow = 1 }
        };

        rootVisualElement.Add(_graphView);
    }

    private void CreateToolbar()
    {
        var toolbar = new Toolbar();
        // Setup file save/load
        var fileNameTextField = new TextField("Field Name:");
        var fileNameLabel = fileNameTextField.Query<Label>().AtIndex(0);
        var fileNameInput = fileNameTextField.Query("unity-text-input").AtIndex(0); // HACK: Not safe
        fileNameLabel.style.minWidth = 0;
        fileNameLabel.style.color = new StyleColor(Color.black);
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(newString => _fileName = newString.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        var nodeCreateButton = new Button(() =>
        {
            _graphView.CreateDialogueNode("Dialogue Node",
                _graphView.contentViewContainer.WorldToLocal(new Vector2(230, 70)));
        });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        rootVisualElement.Insert(0, toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Enter a valid file name", "OK");
            return;
        }

        var saveUtiliy = GraphSaveUtility.GetInstance(_graphView);
        if (save) saveUtiliy.SaveGraph(_fileName);
        else saveUtiliy.LoadGraph(_fileName);
    }

    private void CreateMiniMap()
    {
        _map = new MiniMap { anchored = true };
        ResizeMiniMap();
        _graphView.Add(_map);
    }

    private void ResizeMiniMap()
    {
        var cords = _graphView.contentViewContainer.WorldToLocal(
            new Vector2(this.position.width - 225, 15));
        _map.SetPosition(new Rect(cords.x, cords.y, 200, 140));
    }
}