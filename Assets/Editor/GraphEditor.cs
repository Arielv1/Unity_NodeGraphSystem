using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;
public class GraphEditor : EditorWindow
{
    private GraphViewScreen graphView;
    private Toolbar toolbar;
    private string fileName = "New Graph";
    [MenuItem("Graph/Graph Window")]
    public static void OpenGraphWindow()
    {
        var window = GetWindow<GraphEditor>();
        window.titleContent = new GUIContent("Graph View");
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
            name = "GraphView"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    void CreateButtonForToolbar(Toolbar toolbar, Action action, string btnName)
    {
        toolbar.Add(new Button((action)) { text = btnName });
    }

    private void BuildToolbar()
    {
        toolbar = new Toolbar();

        // Btn clear nodes
        CreateButtonForToolbar(toolbar, () => graphView.RemoveAllNodesFromGraphView(), "Clear Nodes");

        TextField fileNameTextField = new TextField("File Name");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback((e) =>
        {
            fileName = e.newValue; 
        });
        toolbar.Add(fileNameTextField);
    
        // Btns save & load
        CreateButtonForToolbar(toolbar, () => PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType.Save), "Save");
        CreateButtonForToolbar(toolbar, () => { // Reloads GraphView, then loads the graph - without reloading view the edges will disconnect from nodes
            OnDisable();
            OnEnable(); 
            PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType.Load); 
            }
            , "Load");

        graphView.Add(toolbar);
    }

    // Performs action based on file - for now save & load file, if more actions are added in future need to add their case in switch statement.
    void PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType actionType)
    {
        if (fileName == null || fileName.Length == 0)
        {
            EditorUtility.DisplayDialog("Invalid File Name", "No name was entered or it was invalid.", "Close");
            return;
        }

        GraphUtilities graphUtils = GraphUtilities.GetInstance(graphView);

        switch (actionType)
        {
            case GraphUtilities.GraphFileActionType.Save:
                graphUtils.SaveGraphView(fileName);
                break;
            case GraphUtilities.GraphFileActionType.Load:
                graphUtils.LoadGraphView(fileName);
                break;
        }
    }
}