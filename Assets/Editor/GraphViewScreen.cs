
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

public class GraphViewScreen : GraphView
{
    public readonly Vector2 NodeSize = new Vector2(150, 200);
    private float nodePosOffset = 0;
    private float nodePosInit = 100;
    private List<NodeType> NodesList = new List<NodeType>();

    // Interaction with GraphView window screen editor
    public GraphViewScreen()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new RectangleSelector());

        var bgGrid = new GridBackground();
        Insert(0, bgGrid);
        bgGrid.StretchToParentSize();
        styleSheets.Add(Resources.Load<StyleSheet>("GraphBG"));
    }

    public List<NodeType> GetGraphViewNodeList()
    {
        return this.NodesList;
    }

    // Right mouse click actions
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

        // Input port
        var inputPort = GeneratePort(nodeType, Direction.Input);
        inputPort.portName = "Input";
        nodeType.inputContainer.Add(inputPort);

        // Output port
        var outputPort = GeneratePort(nodeType, Direction.Output);
        outputPort.portName = "Output";
        nodeType.outputContainer.Add(outputPort);

        // Spawn position
        nodeType.SetPosition(new Rect(nodePosInit + nodePosOffset, nodePosInit + nodePosOffset, NodeSize.x, NodeSize.y));
        nodePosOffset = nodePosOffset < 100 ? nodePosOffset + 10 : 0;

        nodeType.styleSheets.Add(Resources.Load<StyleSheet>("Node" + type.ToString() + "BG"));

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

    // Nodes can connect to other nodes with the same type.
    // Bridge nodes can be connected and connect to any other node.
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
