using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RoutingNetworkLibrary;

public class Node
{
    public Node()
    {
        ConnectedNodes = [];
    }

    public delegate void NewConnectionHandler(Node callingNode);

    public event NewConnectionHandler? NewConnection;

    public Guid Id { get; } = Guid.NewGuid();
    public string? Name { get; set; }
    public Dictionary<Node, Dictionary<Node, int>> ConnectedNodes { get; set; }

    public Func<Dictionary<Node, int>, RoutingResult> RoutingMethod { get; set; } =
        (Dictionary<Node, int> possibleNextNodes) =>
        {
            var result = possibleNextNodes.OrderBy(n => n.Value).First();
            return new RoutingResult(result.Key, result.Value);
        };

    private bool HasNewConnection { get; set; } = false;

    private void CallNewConnection()
    {
        if (HasNewConnection)
        {
            HasNewConnection = false;
            NewConnection?.Invoke(this);
        }
    }

    public Node? Route(Node destination)
    {
        if (!ConnectedNodes.ContainsKey(destination))
        {
            return null;
        }

        return RoutingMethod(ConnectedNodes[destination]).Node;
    }

    public void Connect(Node targetNode)
    {
        ConnectIntern(targetNode);

        if (!targetNode.ConnectedNodes.ContainsKey(this)
            || !targetNode.ConnectedNodes[this].ContainsKey(this))
        {
            targetNode.Connect(this);
        }

        CallNewConnection();
    }

    private void ConnectIntern(Node targetNode, Node? nextNode = null, int cost = 0)
    {
        if (nextNode == null)
        {
            nextNode = targetNode;
            nextNode.NewConnection -= OnNeighborNodeHasNewConnection;
            nextNode.NewConnection += OnNeighborNodeHasNewConnection;
        }

        if (!ConnectedNodes.ContainsKey(targetNode))
        {
            ConnectedNodes[targetNode] = new Dictionary<Node, int>();
        }
        if (!ConnectedNodes[targetNode].ContainsKey(nextNode))
        {
            ConnectedNodes[targetNode].Add(nextNode, int.MaxValue);
        }

        if (ConnectedNodes[targetNode][nextNode] > cost)
        {
            //Debug.WriteLine($"Connect {this.Name}->{targetNode.Name}{(nextNode == null ? "" : $" over {nextNode.Name}")}");
            ConnectedNodes[targetNode][nextNode] = cost;
            HasNewConnection = true;
        }
    }

    private void OnNeighborNodeHasNewConnection(Node callingNode)
    {
        foreach (var connectedNode in callingNode.ConnectedNodes)
        {
            if (connectedNode.Key == this)
            {
                continue;
            }

            foreach (var path in connectedNode.Value)
            {
                if (path.Key == this)
                {
                    continue;
                }

                ConnectIntern(connectedNode.Key, callingNode, path.Value + 1);
            }
        }

        CallNewConnection();
    }

    public IEnumerable<Node> GetBestPathTo(Node destination)
    {
        if (this == destination)
        {
            return new List<Node> { this };
        }

        var nextNode = Route(destination);
        if (nextNode == null)
        {
            return new List<Node>();
        }
        else
        {
            var path = nextNode.GetBestPathTo(destination);
            return new List<Node> { this }.Concat(path);
        }
    }

    public IEnumerable<IEnumerable<Node>> GetAllPathTo(Node destination, IEnumerable<Node?> callingNodes = null)
    {
        if (this == destination)
        {
            return [[this]];
        }

        if (!ConnectedNodes.ContainsKey(destination))
        {
            return [];
        }

        var result = new List<IEnumerable<Node>>();

        var nextNodes = ConnectedNodes[destination]
            .Where(n => (callingNodes?.Contains(n.Key) ?? false) == false)
            .OrderBy(n => n.Value)
            .Select(n => n.Key);

        foreach (var nextNode in nextNodes)
        {
            if (callingNodes == null)
            {
                callingNodes = new List<Node>();
            }

            var allPathOfNextNode = nextNode.GetAllPathTo(destination, callingNodes.Append(this));
            foreach (var path in allPathOfNextNode)
            {
                result.Add(new List<Node> { this }.Concat(path));
            }
        }
        return result;
    }
}

public class RoutingResult(Node node, int cost)
{
    public Node Node { get; } = node;
    public int Cost { get; } = cost;
}