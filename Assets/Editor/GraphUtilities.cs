using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
public class GraphUtilities
{
    public enum GraphFileActionType
    {
        Save,
        Load
    }
    
    private GraphViewScreen graphView;
    private List<NodeType> graphViewNodes ;
    private List<Edge> graphViewEdges;
    private NodeDataContainer container;
    /*public  GraphUtilities GetInstance(GraphViewScreen targetView)
    {
        return new GraphUtilities
        {
            graphView = targetView,
            graphViewNodes = targetView.nodes.ToList().Cast<NodeType>().ToList(),
            graphViewEdges = targetView.edges.ToList()
        };
    }*/

    public GraphUtilities(GraphViewScreen targetView)
    {
        graphView = targetView;
        graphViewNodes = targetView.nodes.ToList().Cast<NodeType>().ToList();
        graphViewEdges = targetView.edges.ToList();
    }


    public void SaveGraphView(string fileName)
    {
        NodeDataContainer nodeDataContainer = ScriptableObject.CreateInstance<NodeDataContainer>();

        // Get all edges in NodeDataContainer
        graphViewEdges.ForEach((edge) =>
        {
            NodeType outputNode = edge.output.node as NodeType;
            NodeType inputNode = edge.input.node as NodeType;
            nodeDataContainer.NodesConnectionsData.Add(
                new NodeConnectionData(outputNode.GUID, inputNode.GUID, edge.output.portName));
            
        });

        // Get all nodes data into NodeDataContainer
        graphViewNodes.ForEach((node) =>
        {
            nodeDataContainer.NodesData.Add(
                new NodeData(node.GUID, node.type, node.GetPosition().position));
        });

        // Save NodeDataContainer
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        EditorUtility.SetDirty(nodeDataContainer);
        AssetDatabase.CreateAsset(nodeDataContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraphView(string fileName)
    {
        container = Resources.Load<NodeDataContainer>(fileName);
        if (container == null)
        {
            EditorUtility.DisplayDialog("Invalid File", "Could not find file with the given name.", "Close");
            return;
        }
        ClearPresentGraph();
        CreateNodes();
        CreateEdges();
    }

    private void ClearPresentGraph()
    {
        graphView.RemoveAllNodesFromGraphView();
    }

    // Create NodeType for each NodeData in container
    private void CreateNodes()
    {
        container.NodesData.ForEach((node) =>
        {
            NodeType loadedNode = graphView.CreateNodeType(node.type);
            loadedNode.GUID = node.NodeGUID;
            loadedNode.SetPosition(new Rect(node.Position.x, node.Position.y, graphView.NodeSize.x, graphView.NodeSize.y));
            graphView.AddNodeToScreen(loadedNode);
        });
    }

    // Create conenction betwenn node for each NodeDataConnection in container
    private void CreateEdges()
    {
        List<NodeType> allNodes = graphView.GetGraphViewNodeList();
        container.NodesConnectionsData.ForEach(nodeConnection =>
        {
            NodeType baseNode = allNodes.Find(n => n.GUID == nodeConnection.BaseNodeGUID);
            NodeType targetNode = allNodes.Find(n => n.GUID == nodeConnection.TargetNodeGUID);
            LinkNodes(baseNode.outputContainer.Q<Port>(), targetNode.inputContainer.Q<Port>());
        });
    }

    private void LinkNodes(Port output, Port input)
    {
        var edge = new Edge
        {
            output = output,
            input = input
        };
        edge.input.Connect(edge);
        edge.output.Connect(edge);
        graphView.Add(edge);
    }
}

