namespace RoutingNetworkLibrary
{
    public class Node
    {
        public delegate void NewConnectionHandler(Node callingNode, Node targetNode);

        public event NewConnectionHandler? NewConnection;

        public Guid Id { get; } = Guid.NewGuid();
        public string? Name { get; set; }

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

        public void Connect(Node targetNode)
        {
            if (!ConnectedNodes.ContainsKey(targetNode))
            {
                Connect(targetNode, targetNode);
            }
        }

        private void Connect(Node targetNode, Node nextNode)
        {
            if (targetNode.Equals(this))
            {
                return;
            }

            if (!ConnectedNodes.ContainsKey(targetNode))
            {
                ConnectedNodes[targetNode] = new Dictionary<Node, int>();
            }

            if (targetNode.Equals(nextNode))
            {
                ConnectedNodes[targetNode].Add(nextNode, 0);
                targetNode.Connect(this);
            }
            else if (ConnectedNodes.ContainsKey(nextNode))
            {
                foreach (var nodeCost in ConnectedNodes[nextNode])
                {
                    //Bug: n2 denkt das n1 nun mit n3 verbunden ist was nicht stimmt, sollte man abbrechen wenn targetnode bekannt ist oder wnen target und nextnode bekannt sind oder ganze anders?
                    ConnectedNodes[targetNode].Add(nodeCost.Key, nodeCost.Value + 1);
                }
            }

            targetNode.NewConnection += OnTargetNodeNewConnection;
            NewConnection?.Invoke(this, targetNode);
        }

        private void OnTargetNodeNewConnection(Node callingNode, Node targetNode)
        {
            Connect(targetNode, callingNode);
        }

        public Node Route(Node destination)
        {
            return RoutingMethod(this, ConnectedNodes[destination]);
        }
    }
}