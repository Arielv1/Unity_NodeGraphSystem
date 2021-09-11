
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

public class GraphViewScreen : GraphView
{
    private List<NodeType> NodesList = new List<NodeType>();
    public GraphViewScreen()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new RectangleSelector());

        var bgGrid = new GridBackground();
        Insert(0, bgGrid);
        bgGrid.StretchToParentSize();
        styleSheets.Add(Resources.Load<StyleSheet>("Graph"));
    }

    public List<NodeType> GetGraphViewNodeList()
    {
        return this.NodesList;
    }
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Add NodeType A", (action) => AddNodeToScreen(CreateNodeType(NodeType.Type.TypeA)));
        evt.menu.AppendAction("Add NodeType B", (action) => AddNodeToScreen(CreateNodeType(NodeType.Type.TypeB)));
        evt.menu.AppendAction("Add NodeType C", (action) => AddNodeToScreen(CreateNodeType(NodeType.Type.TypeC)));
        evt.menu.AppendAction("Add Bridge Node", (action) => AddNodeToScreen(CreateNodeType(NodeType.Type.Bridge)));
    }

   

    private Port GeneratePort(NodeType node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public NodeType CreateNodeType(NodeType.Type type)
    {
        var nodeType = new NodeType(Guid.NewGuid().ToString(), type);
        nodeType.title = type.ToString();
        
        var inputPort = GeneratePort(nodeType, Direction.Input);
        inputPort.portName = "Input";
        nodeType.outputContainer.Add(inputPort);

        var outputPort = GeneratePort(nodeType, Direction.Output);
        outputPort.portName = "Output";
        nodeType.outputContainer.Add(outputPort);

        nodeType.SetPosition(new Rect(200, 200, 150, 150));
        nodeType.RefreshExpandedState();
        nodeType.RefreshPorts();

        return nodeType;
    }

    public void AddNodeToScreen(NodeType node)
    {
        AddElement(node);
        NodesList.Add(node);
    }

    public void RemoveAllNodesFromGraphView()
    {
        DeleteElements(NodesList);
        this.edges.ToList().ForEach((edge) =>
        {
            RemoveElement(edge);
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        NodeType.Type startNodeType = (startPort.node as NodeType).type;
        ports.ForEach((port) =>
        {
            NodeType.Type endNodeType = (port.node as NodeType).type;
            
            if (startPort != port && startPort.node != port.node)
            {
                if (startNodeType == NodeType.Type.Bridge || endNodeType == NodeType.Type.Bridge)
                {
                    compatiblePorts.Add(port);
                }
                else if (startNodeType == endNodeType)
                {
                    compatiblePorts.Add(port);
                }
            }  
            
        });
        return compatiblePorts;
    }
}
