using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using System;



public class GraphUtilities
{
    public enum GraphFileActionType
    {
        Export,
        Import
    }
    
    private GraphViewScreen graphView;
    private List<NodeType> graphViewNodes ;
    private List<Edge> graphViewEdges;
    private NodeDataContainer container;
    public static GraphUtilities GetInstance(GraphViewScreen targetView)
    {
        return new GraphUtilities
        {
            graphView = targetView,
            graphViewNodes = targetView.nodes.ToList().Cast<NodeType>().ToList(),
            graphViewEdges = targetView.edges.ToList()
        };
    }


    public void ExportGraph(string fileName)
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
        AssetDatabase.CreateAsset(nodeDataContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void ImportGraph(string fileName)
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

    private void CreateNodes()
    {
        container.NodesData.ForEach((node) =>
        {
            NodeType loadedNode = graphView.CreateNodeType(node.type);
            loadedNode.GUID = node.NodeGUID;
            loadedNode.SetPosition(new Rect(node.Position.x, node.Position.y, 150, 150));
            graphView.AddNodeToScreen(loadedNode);


            Debug.Log(loadedNode.inputContainer[0]);
        });
    }

    private void CreateEdges()
    {
        /*
        for (int i = 0; i < graphViewNodes.Count; i++)
        {
            List<NodeConnectionData> connections = container.NodesConnectionsData.Where(n => n.BaseNodeGUID == graphViewNodes[i].GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                string baseNodeGUID = connections[j].BaseNodeGUID;
                string targetNodeGUID = connections[j].TargetNodeGUID;
                NodeType targetNode = graphViewNodes.First(n => n.GUID == targetNodeGUID);
                //Debug.Log("Input " + graphViewNodes[i].inputContainer[j].Q<Port>());
                //Debug.Log("OutPut " + graphViewNodes[i].outputContainer[j].Q<Port>());
                
                //LinkNodes(graphViewNodes[i].inputContainer[j].Q<Port>(), graphViewNodes[i].outputContainer[j].Q<Port>());
            }
        }*/
        List<NodeType> allNodes = graphView.GetGraphViewNodeList();
        for (int i = 0; i < container.NodesConnectionsData.Count; i++)
        {
            NodeType baseNode = allNodes.Find(n => n.GUID == container.NodesConnectionsData[i].BaseNodeGUID);
            NodeType targetNode = allNodes.Find(n => n.GUID == container.NodesConnectionsData[i].TargetNodeGUID);
            //Debug.Log(baseNode.outputContainer.Q<Port>());
            //Debug.Log(targetNode.inputContainer.Q<Port>());
            //LinkNodes(baseNode.outputContainer.Q<Port>(), targetNode.inputContainer.Q<Port>());
        }
        /*List<NodeType> allNodes = graphView.GetGraphViewNodeList();
        for (int i = 0; i < allNodes.Count; i++)
        {
            List<NodeConnectionData> connections = container.NodesConnectionsData.Where(n => n.BaseNodeGUID == allNodes[i].GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGUID = connections[j].TargetNodeGUID;
                NodeType targetNode = allNodes.First(n => n.GUID == targetNodeGUID);
                LinkNodes(allNodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
            }
        }*/
        
    }

    private void LinkNodes(Port output, Port input)
    {
        Debug.Log(output);
        Debug.Log(input);
        
        var edge = new Edge
        {
            output = output,
            input = output
        };
        edge.input.Connect(edge);
        edge.output.Connect(edge);
        graphView.Add(edge);
    }
}

