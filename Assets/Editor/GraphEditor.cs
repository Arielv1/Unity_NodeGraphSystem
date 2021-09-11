using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphEditor : EditorWindow
{
    private GraphViewScreen graphView;
    private string fileName = "New Graph";
    [MenuItem("Graph/Graph Window")]
    public static void OpenGraphWindow()
    {
        var window = GetWindow<GraphEditor>();
        window.titleContent = new GUIContent("Graph");
    }

    private void OnEnable()
    {
        BuildGraph();
        BuildToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void BuildGraph()
    {
        graphView = new GraphViewScreen
        {
            name = "Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void BuildToolbar()
    {
        var toolbar = new Toolbar();
     
        /* Add NodeTypes - Start */
        var btnRemoveAllNodes = new Button(() =>
        {
            graphView.RemoveAllNodesFromGraphView();
        });
        btnRemoveAllNodes.text = "Clear Nodes";
        toolbar.Add(btnRemoveAllNodes);
        /*
        var btnCreateNewNodeTypeA = new Button(() =>
        {
            graphView.CreateNodeType(NodeType.Type.TypeA);
        });
        btnCreateNewNodeTypeA.text = "Add NodeType A";
        toolbar.Add(btnCreateNewNodeTypeA);

        var btnCreateNewNodeTypeB = new Button(() =>
        {
            graphView.CreateNodeType(NodeType.Type.TypeB);
        });
        btnCreateNewNodeTypeB.text = "Add NodeType B";
        toolbar.Add(btnCreateNewNodeTypeB);

        var btnCreateNewNodeTypeC = new Button(() =>
        {
            graphView.CreateNodeType(NodeType.Type.TypeC);
        });
        btnCreateNewNodeTypeC.text = "Add NodeType C";
        toolbar.Add(btnCreateNewNodeTypeC);

        var btnCreateNewNodeBridge = new Button(() =>
        {
            graphView.CreateNodeType(NodeType.Type.Bridge);
        });
        btnCreateNewNodeBridge.text = "Add Connector Node";
        toolbar.Add(btnCreateNewNodeBridge);
        */
        /* Add NodeTypes - End */

        /* Export & Import - Start */
        var fileNameTextField = new TextField("File Name");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback((e) =>
        {
            fileName = e.newValue; 
        });
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType.Export)) { text = "Export" });
        toolbar.Add(new Button(() => PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType.Import)) { text = "Import" });
        /* Export & Import - End */

        rootVisualElement.Add(toolbar);


    }

    void PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType actionType)
    {
        if (fileName == null || fileName.Length == 0)
        {
            EditorUtility.DisplayDialog("Invalid File Name", "No name was entered or it was invalid.", "Close");
            return;
        }

        var graphUtils = GraphUtilities.GetInstance(graphView);

        switch (actionType)
        {
            case GraphUtilities.GraphFileActionType.Export:
                graphUtils.ExportGraph(fileName);
                break;
            case GraphUtilities.GraphFileActionType.Import:
                graphUtils.ImportGraph(fileName);
                break;
        }
    }
}