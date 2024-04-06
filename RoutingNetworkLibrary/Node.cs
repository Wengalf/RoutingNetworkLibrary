namespace RoutingNetworkLibrary
{
    public class Node
    {
        public delegate void NewConnectionHandler(Node targetNode, Node callingNode, int cost);
        public event NewConnectionHandler NewConnection;

        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Key = finaler Target Node wo es am Ende hin soll
        /// Value = Liste aller Nodes über die der Key-Node erreichbar ist. (Key = Node wo es lang geht, Value = anzahl der Sprünge bis zum Target-Node)
        /// </summary>
        public Dictionary<Node, Dictionary<Node, int>> ConnectedNodes { get; set; }

        /// <summary>
        /// Funktion zum ermitteln was der beste Weg zum Ziel ist.
        /// Sollte bekommen:
        /// Target-Node
        /// Liste mit Nodes über die Target-Node erreichbar ist
        ///
        /// Liefert den best möglich ermittelten Node
        /// </summary>
        public Func<Node, Dictionary<Node, int>, Node> RoutingMethod { get; set; } =
            (Node targetNode, Dictionary<Node, int> possibleNextNodes) =>
            {
                return possibleNextNodes.OrderBy(n => n.Value).First().Key;
            };

        public Node()
        {
            ConnectedNodes = new Dictionary<Node, Dictionary<Node, int>>();
        }

        public void Connect(Node targetNode, Node nextNode, int cost)
        {
            if (!ConnectedNodes.ContainsKey(targetNode))
            {
                ConnectedNodes[targetNode] = new Dictionary<Node, int>();
            }

            cost++;
            ConnectedNodes[targetNode].Add(nextNode, cost);

            NewConnection?.Invoke(targetNode, this, cost);
        }

        public Node Route(Node destination)
        {
            return RoutingMethod(this, ConnectedNodes[destination]);
        }
    }
}