using RoutingNetworkLibrary;

namespace RoutingNetworkLibraryTest
{
  [TestClass]
  public class NodesTest
  {
    [TestMethod]
    public void TestNodeConnect()
    {
      var n1 = new Node();
      var n2 = new Node();
      var n3 = new Node();
      var n4 = new Node();
      var n5 = new Node();
      var n6 = new Node();

      n1.Connect(n2, n2, 0);
      n1.Connect(n3, n2, 1);
      n1.Connect(n4, n2, 2);
      n1.Connect(n5, n5, 0);
      n1.Connect(n6, n5, 1);

      n2.Connect(n1, n1, 0);
      n2.Connect(n3, n3, 0);
      n2.Connect(n4, n3, 1);
      n2.Connect(n5, n1, 1);
      n2.Connect(n6, n1, 2);

      n3.Connect(n1, n2, 1);
      n3.Connect(n2, n2, 0);
      n3.Connect(n4, n4, 0);
      n3.Connect(n5, n2, 2);
      n3.Connect(n6, n2, 3);

      n4.Connect(n1, n3, 2);
      n4.Connect(n2, n3, 1);
      n4.Connect(n3, n3, 0);
      n4.Connect(n5, n3, 3);
      n4.Connect(n6, n3, 4);

      n5.Connect(n1, n1, 0);
      n5.Connect(n2, n1, 1);
      n5.Connect(n3, n1, 2);
      n5.Connect(n4, n1, 4);
      n5.Connect(n6, n6, 0);

      n6.Connect(n1, n5, 1);
      n6.Connect(n2, n5, 2);
      n6.Connect(n3, n5, 3);
      n6.Connect(n4, n5, 4);
      n6.Connect(n5, n5, 0);

      Assert.AreEqual(n2, n1.Route(n2));
      Assert.AreEqual(n2, n1.Route(n3));
      Assert.AreEqual(n2, n1.Route(n4));
      Assert.AreEqual(n5, n1.Route(n5));
      Assert.AreEqual(n5, n1.Route(n6));

      Assert.AreEqual(n1, n2.Route(n1));
      Assert.AreEqual(n3, n2.Route(n3));
      Assert.AreEqual(n3, n2.Route(n4));
      Assert.AreEqual(n1, n2.Route(n5));
      Assert.AreEqual(n1, n2.Route(n6));

      Assert.AreEqual(n2, n3.Route(n1));
      Assert.AreEqual(n2, n3.Route(n2));
      Assert.AreEqual(n4, n3.Route(n4));
      Assert.AreEqual(n2, n3.Route(n5));
      Assert.AreEqual(n2, n3.Route(n6));

      Assert.AreEqual(n3, n4.Route(n1));
      Assert.AreEqual(n3, n4.Route(n2));
      Assert.AreEqual(n3, n4.Route(n3));
      Assert.AreEqual(n3, n4.Route(n5));
      Assert.AreEqual(n3, n4.Route(n6));

      Assert.AreEqual(n1, n5.Route(n1));
      Assert.AreEqual(n1, n5.Route(n2));
      Assert.AreEqual(n1, n5.Route(n3));
      Assert.AreEqual(n1, n5.Route(n4));
      Assert.AreEqual(n6, n5.Route(n6));

      Assert.AreEqual(n5, n6.Route(n1));
      Assert.AreEqual(n5, n6.Route(n2));
      Assert.AreEqual(n5, n6.Route(n3));
      Assert.AreEqual(n5, n6.Route(n4));
      Assert.AreEqual(n5, n6.Route(n5));
    }


    [TestMethod]
    public void TestNodeRoutingMethod()
    {
      var n1 = new Node();
      var n2 = new Node();
      var n3 = new Node();
      var n4 = new Node();

      n1.Connect(n2, n2, 0);
      n1.Connect(n3, n2, 1);
      n1.Connect(n4, n2, 2);
      n1.Connect(n2, n4, 2);
      n1.Connect(n3, n4, 1);
      n1.Connect(n4, n4, 0);

      n2.Connect(n1, n1, 0);
      n2.Connect(n3, n3, 0);
      n2.Connect(n4, n3, 1);
      n2.Connect(n4, n1, 1);
      n2.Connect(n1, n3, 2);
      n2.Connect(n3, n1, 2);

      n3.Connect(n1, n2, 1);
      n3.Connect(n2, n2, 0);
      n3.Connect(n4, n4, 0);
      n3.Connect(n4, n2, 2);
      n3.Connect(n1, n4, 1);
      n3.Connect(n2, n4, 2);

      n4.Connect(n1, n1, 0);
      n4.Connect(n2, n1, 1);
      n4.Connect(n2, n3, 1);
      n4.Connect(n3, n3, 0);
      n4.Connect(n1, n3, 2);
      n4.Connect(n3, n1, 2);

      Assert.AreEqual(n2, n1.Route(n2));
      Assert.AreEqual(n4, n1.Route(n4));
      Assert.IsTrue(new Node[] { n2, n4 }.Contains(n1.Route(n3)));

      Assert.AreEqual(n1, n2.Route(n1));
      Assert.AreEqual(n3, n2.Route(n3));
      Assert.IsTrue(new Node[] { n1, n3 }.Contains(n2.Route(n4)));

      Assert.AreEqual(n2, n3.Route(n2));
      Assert.AreEqual(n4, n3.Route(n4));
      Assert.IsTrue(new Node[] { n2, n4 }.Contains(n3.Route(n1)));

      Assert.AreEqual(n1, n4.Route(n1));
      Assert.AreEqual(n3, n4.Route(n3));
      Assert.IsTrue(new Node[] { n1, n3 }.Contains(n4.Route(n2)));
      
      var newRoutingMethod =
        (Node targetNode, Dictionary<Node, int> possibleNextNodes) =>
        {
          return possibleNextNodes.OrderByDescending(n => n.Value).First().Key;
        };

      n1.RoutingMethod = newRoutingMethod;
      n2.RoutingMethod = newRoutingMethod;
      n3.RoutingMethod = newRoutingMethod;
      n4.RoutingMethod = newRoutingMethod;

      Assert.AreEqual(n4, n1.Route(n2));
      Assert.AreEqual(n2, n1.Route(n4));
      Assert.IsTrue(new Node[] { n2, n4 }.Contains(n1.Route(n3)));

      Assert.AreEqual(n3, n2.Route(n1));
      Assert.AreEqual(n1, n2.Route(n3));
      Assert.IsTrue(new Node[] { n1, n3 }.Contains(n2.Route(n4)));

      Assert.AreEqual(n4, n3.Route(n2));
      Assert.AreEqual(n2, n3.Route(n4));
      Assert.IsTrue(new Node[] { n2, n4 }.Contains(n3.Route(n1)));

      Assert.AreEqual(n3, n4.Route(n1));
      Assert.AreEqual(n1, n4.Route(n3));
      Assert.IsTrue(new Node[] { n1, n3 }.Contains(n4.Route(n2)));
    }
  }
}