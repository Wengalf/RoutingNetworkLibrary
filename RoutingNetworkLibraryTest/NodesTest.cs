using RoutingNetworkLibrary;
using System.Linq;

namespace RoutingNetworkLibraryTest;

[TestClass]
public class NodesTest
{
    private List<Node> GetNodes()
    {
        var nodes = new List<Node>()
        {
            new() { Name = "n1" },
            new() { Name = "n2" },
            new() { Name = "n3" },
            new() { Name = "n4" },
            new() { Name = "n5" },
        };

        nodes[0].Connect(nodes[1]);
        nodes[0].Connect(nodes[3]);

        nodes[1].Connect(nodes[2]);
        nodes[1].Connect(nodes[4]);

        nodes[2].Connect(nodes[3]);

        nodes[3].Connect(nodes[4]);

        return nodes;
    }

    [TestMethod]
    public void TestNodeRoutingUnknownNode()
    {
        var nodes = GetNodes();

        Assert.IsNull(new Node() { Name = "nTemp" }.Route(nodes[2]));
    }

    [TestMethod]
    public void TestNodeRoutingMethod()
    {
        var nodes = GetNodes();

        Assert.AreEqual(nodes[1], nodes[0].Route(nodes[1]));
        Assert.AreEqual(nodes[3], nodes[0].Route(nodes[3]));
        Assert.IsTrue(new Node[] { nodes[1], nodes[3] }.Contains(nodes[0].Route(nodes[2])));

        Assert.AreEqual(nodes[0], nodes[1].Route(nodes[0]));
        Assert.AreEqual(nodes[2], nodes[1].Route(nodes[2]));
        Assert.IsTrue(new Node[] { nodes[0], nodes[2] }.Contains(nodes[1].Route(nodes[3])));

        Assert.AreEqual(nodes[1], nodes[2].Route(nodes[1]));
        Assert.AreEqual(nodes[3], nodes[2].Route(nodes[3]));
        Assert.IsTrue(new Node[] { nodes[1], nodes[3] }.Contains(nodes[2].Route(nodes[0])));

        Assert.AreEqual(nodes[0], nodes[3].Route(nodes[0]));
        Assert.AreEqual(nodes[2], nodes[3].Route(nodes[2]));
        Assert.IsTrue(new Node[] { nodes[0], nodes[2] }.Contains(nodes[3].Route(nodes[1])));
    }

    [TestMethod]
    public void TestNodeRoutingMethodOverride()
    {
        var nodes = GetNodes();

        var newRoutingMethod =
          (Dictionary<Node, int> possibleNextNodes) =>
          {
              var result = possibleNextNodes.OrderByDescending(n => n.Value).First();
              return new RoutingResult(result.Key, result.Value);
          };

        nodes[0].RoutingMethod = newRoutingMethod;
        nodes[1].RoutingMethod = newRoutingMethod;
        nodes[2].RoutingMethod = newRoutingMethod;
        nodes[3].RoutingMethod = newRoutingMethod;

        Assert.AreEqual(nodes[3], nodes[0].Route(nodes[1]));
        Assert.AreEqual(nodes[1], nodes[0].Route(nodes[3]));
        Assert.IsTrue(new Node[] { nodes[1], nodes[3] }.Contains(nodes[0].Route(nodes[2])));

        Assert.AreEqual(nodes[2], nodes[1].Route(nodes[0]));
        Assert.AreEqual(nodes[0], nodes[1].Route(nodes[2]));
        Assert.IsTrue(new Node[] { nodes[0], nodes[2] }.Contains(nodes[1].Route(nodes[3])));

        Assert.AreEqual(nodes[3], nodes[2].Route(nodes[1]));
        Assert.AreEqual(nodes[1], nodes[2].Route(nodes[3]));
        Assert.IsTrue(new Node[] { nodes[1], nodes[3] }.Contains(nodes[2].Route(nodes[0])));

        Assert.AreEqual(nodes[2], nodes[3].Route(nodes[0]));
        Assert.AreEqual(nodes[0], nodes[3].Route(nodes[2]));
        Assert.IsTrue(new Node[] { nodes[0], nodes[2] }.Contains(nodes[3].Route(nodes[1])));
    }

    [TestMethod]
    public void TestNodeGetBestPathTo()
    {
        var nodes = GetNodes();

        var path = nodes[0].GetBestPathTo(nodes[3]);
        Assert.AreEqual(2, path.Count());
        Assert.AreEqual(nodes[0], path.ElementAt(0));
        Assert.AreEqual(nodes[3], path.ElementAt(1));

        path = nodes[0].GetBestPathTo(nodes[4]);
        Assert.AreEqual(3, path.Count());
        Assert.AreEqual(nodes[0], path.ElementAt(0));
        Assert.AreEqual(nodes[1], path.ElementAt(1));
        Assert.AreEqual(nodes[4], path.ElementAt(2));
    }

    [TestMethod]
    public void TestNodeGetAllPathTo()
    {
        var nodes = GetNodes();

        var paths = nodes[0].GetAllPathTo(nodes[3]);
        Assert.AreEqual(3, paths.Count());
        Assert.AreEqual(2, paths.ElementAt(0).Count());
        Assert.AreEqual(4, paths.ElementAt(1).Count());
        Assert.AreEqual(4, paths.ElementAt(2).Count());

        Assert.AreEqual(nodes[0], paths.ElementAt(0).ElementAt(0));
        Assert.AreEqual(nodes[3], paths.ElementAt(0).ElementAt(1));

        Assert.AreEqual(nodes[0], paths.ElementAt(1).ElementAt(0));
        Assert.AreEqual(nodes[1], paths.ElementAt(1).ElementAt(1));
        Assert.AreEqual(nodes[2], paths.ElementAt(1).ElementAt(2));
        Assert.AreEqual(nodes[3], paths.ElementAt(1).ElementAt(3));

        Assert.AreEqual(nodes[0], paths.ElementAt(2).ElementAt(0));
        Assert.AreEqual(nodes[1], paths.ElementAt(2).ElementAt(1));
        Assert.AreEqual(nodes[4], paths.ElementAt(2).ElementAt(2));
        Assert.AreEqual(nodes[3], paths.ElementAt(2).ElementAt(3));

        paths = nodes[0].GetAllPathTo(nodes[4]);
        Assert.AreEqual(4, paths.Count());
        Assert.AreEqual(3, paths.ElementAt(0).Count());
        Assert.AreEqual(5, paths.ElementAt(1).Count());
        Assert.AreEqual(3, paths.ElementAt(2).Count());
        Assert.AreEqual(5, paths.ElementAt(3).Count());

        Assert.AreEqual(nodes[0], paths.ElementAt(0).ElementAt(0));
        Assert.AreEqual(nodes[1], paths.ElementAt(0).ElementAt(1));
        Assert.AreEqual(nodes[4], paths.ElementAt(0).ElementAt(2));

        Assert.AreEqual(nodes[0], paths.ElementAt(1).ElementAt(0));
        Assert.AreEqual(nodes[1], paths.ElementAt(1).ElementAt(1));
        Assert.AreEqual(nodes[2], paths.ElementAt(1).ElementAt(2));
        Assert.AreEqual(nodes[3], paths.ElementAt(1).ElementAt(3));
        Assert.AreEqual(nodes[4], paths.ElementAt(1).ElementAt(4));

        Assert.AreEqual(nodes[0], paths.ElementAt(2).ElementAt(0));
        Assert.AreEqual(nodes[3], paths.ElementAt(2).ElementAt(1));
        Assert.AreEqual(nodes[4], paths.ElementAt(2).ElementAt(2));

        Assert.AreEqual(nodes[0], paths.ElementAt(3).ElementAt(0));
        Assert.AreEqual(nodes[3], paths.ElementAt(3).ElementAt(1));
        Assert.AreEqual(nodes[2], paths.ElementAt(3).ElementAt(2));
        Assert.AreEqual(nodes[1], paths.ElementAt(3).ElementAt(3));
        Assert.AreEqual(nodes[4], paths.ElementAt(3).ElementAt(4));
    }
}