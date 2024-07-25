using System.Linq;

namespace RoutingNetworkLibrary;

[Obsolete]
public class Node_Synch
{
  public delegate void NewConnectionHandler(Node_Synch targetNode, Node_Synch callingNode, int cost);
  public event NewConnectionHandler? NewConnection;

  public Guid Id { get; } = Guid.NewGuid();
  public string? Name { get; set; }
  /// <summary>
  /// Key = finaler Target Node wo es am Ende hin soll
  /// Value = Liste aller Nodes über die der Key-Node erreichbar ist. (Key = Node wo es lang geht, Value = anzahl der Sprünge bis zum Target-Node)
  /// </summary>
  public Dictionary<Node_Synch, Dictionary<Node_Synch, int>> ConnectedNodes { get; set; }

  /// <summary>
  /// Funktion zum ermitteln was der beste Weg zum Ziel ist.
  /// Sollte bekommen:
  /// Target-Node
  /// Liste mit Nodes über die Target-Node erreichbar ist
  ///
  /// Liefert den best möglich ermittelten Node
  /// </summary>
  public Func<Dictionary<Node_Synch, int>, Node_Synch> RoutingMethod { get; set; } =
      (Dictionary<Node_Synch, int> possibleNextNodes) =>
      {
          return possibleNextNodes.OrderBy(n => n.Value).First().Key;
      };

  public Node_Synch()
  {
    ConnectedNodes = [];
  }

  /// <summary>
  /// Verbindet <see cref="this"/> mit <paramref name="targetNode"/> und gibt an über welchen <paramref name="nextNode"/> dieser erreichbar ist und zu welchen <paramref name="cost"/> (anzahl an Nodes dazwischen).
  /// </summary>
  /// <param name="targetNode"></param>
  /// <param name="nextNode"></param>
  /// <param name="cost"></param>
  public void Connect(Node_Synch targetNode, Node_Synch nextNode, int cost)
  {
      if (!ConnectedNodes.ContainsKey(targetNode))
      {
          ConnectedNodes[targetNode] = new Dictionary<Node_Synch, int>();
      }

      cost++;
      ConnectedNodes[targetNode].Add(nextNode, cost);

      NewConnection?.Invoke(targetNode, this, cost);
  }

  /// <summary>
  /// Ermittelt anhand <see cref="RoutingMethod"/> den besten möglichen nächsten <see cref="Node_Synch"> der mit diesem verbunden ist um zum eigentlichen Ziel zu gelangen.
  /// Liefert null wenn Ziel nicht erreichbar.
  /// </summary>
  /// <param name="destination"></param>
  /// <returns></returns>
  public Node_Synch? Route(Node_Synch destination)
  {
      return RoutingMethod(ConnectedNodes[destination]);
  }

  /// <summary>
  /// Ermittelt anhand <see cref="RoutingMethod"/> den besten möglichen nächten <see cref="Node_Synch"> der mit diesem verbunden ist um zum eigentlichen Ziel zu gelangen. 
  /// Und ruft auf diesen wiederum rekursiv <see cref="GetBestPathTo(Node_Synch)"/> auf bis das Ziel erreicht ist und liefert eine Auflistung aller Nodes die bis dahin durchschritten wurden.
  /// Liefert eine Leere Auflistung wenn das Ziel nicht erreichbar ist.
  /// </summary>
  /// <param name="destination"></param>
  /// <returns></returns>
  public IEnumerable<Node_Synch> GetBestPathTo(Node_Synch destination)
  {
    if (this == destination)
    {
      return [this];
    }

    var nextNode = Route(destination);
    if (nextNode == null)
    {
      return [];
    }
    else
    {
      return nextNode.GetBestPathTo(destination).Prepend(this);
    }
  }
  public IEnumerable<IEnumerable<Node_Synch>> GetAllPathTo(Node_Synch destination, Node_Synch? callingNode = null)
  {

    if (this == destination)
    {
      return [[this]];
    }

    if (!ConnectedNodes.ContainsKey(destination))
    {
      return []; 
    }

    var result = new List<IEnumerable<Node_Synch>>();
    
    var nextNodes = ConnectedNodes[destination]
      .Where(n => n.Key != callingNode)
      .OrderBy(n => n.Value)
      .Select(n => n.Key);

    foreach (var nextNode in nextNodes)
    {
      var allPathOfNextNode = nextNode.GetAllPathTo(destination,this);
      foreach (var path in allPathOfNextNode) 
      {
        result.Add(path.Prepend(this));
      }    
    }
    return result;
  }
}
public class Node
{
  public delegate Task NewConnectionHandlerAsync(Node callingNode);
  public event NewConnectionHandlerAsync? NewConnectionAsync;

  public Guid Id { get; } = Guid.NewGuid();
  public string? Name { get; set; }
  public Dictionary<Node, Dictionary<Node, int>> ConnectedNodes { get; set; }

  public Func<Dictionary<Node, int>, RoutingResult> RoutingMethod { get; set; } = GetRoutingMethod();
  public Func<Dictionary<Node, int>, Task<RoutingResult>> RoutingMethodAsync { get; set; } = GetRoutingMethodAsync();

  private static Func<Dictionary<Node, int>, RoutingResult> GetRoutingMethod()
  {
    return
    (Dictionary<Node, int> possibleNextNodes) =>
    {
      var result = possibleNextNodes.OrderBy(n => n.Value).First();
      return new RoutingResult(result.Key, result.Value);
    };
  }
  
  private static Func<Dictionary<Node, int>, Task<RoutingResult>> GetRoutingMethodAsync()
  {
    return 
    async (Dictionary<Node, int> possibleNextNodes) =>
    {
      return await Task.Run(() => GetRoutingMethod()(possibleNextNodes));
    };
  }

  public Node()
  {
    ConnectedNodes = [];
  }

  public async Task ConnectAsync(Node targetNode)
  {
    await ConnectIntern(targetNode);
    await targetNode.ConnectAsync(this);
  }
  private async Task ConnectIntern(Node targetNode, Node? nextNode = null, int cost = 0)
  {
    if (nextNode == null)
    {
      nextNode = targetNode;
      nextNode.NewConnectionAsync += TargetNode_NewConnectionAsync;
    }

    if (!ConnectedNodes.ContainsKey(targetNode))
    {
      ConnectedNodes[targetNode] = new Dictionary<Node, int>();
    }
    if (!ConnectedNodes[targetNode].ContainsKey(nextNode))
    {
      ConnectedNodes[targetNode].Add(nextNode, -1);
    }

    if (ConnectedNodes[targetNode][nextNode] != cost)
    {
      ConnectedNodes[targetNode][nextNode] = cost;    

      if (NewConnectionAsync != null)
      {
        await NewConnectionAsync.Invoke(this);
      }
    }
  }

  private async Task TargetNode_NewConnectionAsync(Node callingNode)
  {
    foreach (var connectedNode in callingNode.ConnectedNodes)
    {
      if (connectedNode.Key == this)
      {
        continue;
      }
      
      var result = await RoutingMethodAsync(connectedNode.Value);
      await ConnectIntern(connectedNode.Key, callingNode, result.Cost + 1);      
    }
  }

  public Node? Route(Node destination)
  {
    return RoutingMethod(ConnectedNodes[destination]).Node;
  }
  public async Task<Node?> RouteAsync(Node destination)
  {
    return (await RoutingMethodAsync(ConnectedNodes[destination])).Node;
  }

  public async Task<IEnumerable<Node>> GetBestPathToAsync(Node destination)
  {
    if (this == destination)
    {
      return new List<Node> { this };
    }

    var nextNode = await RouteAsync(destination);
    if (nextNode == null)
    {
      return new List<Node>();
    }
    else
    {
      var path = await nextNode.GetBestPathToAsync(destination);
      return new List<Node> { this }.Concat(path);
    }
  }

  public async Task<IEnumerable<IEnumerable<Node>>> GetAllPathToAsync(Node destination, Node? callingNode = null)
  {
    if (this == destination)
    {
      return new List<IEnumerable<Node>> { new List<Node> { this } };
    }

    if (!ConnectedNodes.ContainsKey(destination))
    {
      return new List<IEnumerable<Node>>();
    }

    var result = new List<IEnumerable<Node>>();

    var nextNodes = ConnectedNodes[destination]
        .Where(n => n.Key != callingNode)
        .OrderBy(n => n.Value)
        .Select(n => n.Key);

    foreach (var nextNode in nextNodes)
    {
      var allPathOfNextNode = await nextNode.GetAllPathToAsync(destination, this);
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
