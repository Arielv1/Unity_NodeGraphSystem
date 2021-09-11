using UnityEditor.Experimental.GraphView;
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
        window.titleContent = new GUIContent("Graph View");
    }

    private void OnEnable()
    {
        BuildGraph();
        BuildToolbar();
        BuildMinimap();
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

    private void BuildToolbar()
    {
        var toolbar = new Toolbar();
     
        var btnRemoveAllNodes = new Button(() =>
        {
            graphView.RemoveAllNodesFromGraphView();
        });
        btnRemoveAllNodes.text = "Clear Nodes";
        toolbar.Add(btnRemoveAllNodes);

        /* Export & Import - Start */
        var fileNameTextField = new TextField("File Name");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback((e) =>
        {
            fileName = e.newValue; 
        });
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType.Save)) { text = "Save" });
        toolbar.Add(new Button(() => PerformGraphViewOnFileAction(GraphUtilities.GraphFileActionType.Load)) { text = "Load" });
        /* Export & Import - End */

        rootVisualElement.Add(toolbar);
    }

    void BuildMinimap()
    {
        MiniMap minimap = new MiniMap();
        minimap.anchored = true;
        minimap.SetPosition(new Rect(50, 50, 200, 200));
        graphView.Add(minimap);
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
            case GraphUtilities.GraphFileActionType.Save:
                graphUtils.SaveGraphView(fileName);
                break;
            case GraphUtilities.GraphFileActionType.Load:
                graphUtils.LoadGraphView(fileName);
                break;
        }
    }
}