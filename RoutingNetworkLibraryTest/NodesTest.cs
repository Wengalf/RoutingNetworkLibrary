using RoutingNetworkLibrary;
using System.Linq;

namespace RoutingNetworkLibraryTest
{
  [TestClass]
  public class NodesTest
  {
    [TestMethod]
    public void TestNodeConnect()
    {
      var n1 = new Node_Synch();
      var n2 = new Node_Synch();
      var n3 = new Node_Synch();
      var n4 = new Node_Synch();
      var n5 = new Node_Synch();
      var n6 = new Node_Synch();

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
    var n1 = new Node_Synch(){Name="n1"};
    var n2 = new Node_Synch(){Name="n2"};
    var n3 = new Node_Synch(){Name="n3"};
    var n4 = new Node_Synch(){Name="n4"};
    var n5 = new Node_Synch(){Name="n5"};

    n1.Connect(n2, n2, 0);
    n1.Connect(n3, n2, 1);
    n1.Connect(n4, n2, 2);
    n1.Connect(n2, n4, 2);
    n1.Connect(n3, n4, 1);
    n1.Connect(n4, n4, 0);
    n1.Connect(n5, n2, 3);

    n2.Connect(n1, n1, 0);
    n2.Connect(n3, n3, 0);
    n2.Connect(n4, n3, 1);
    n2.Connect(n4, n1, 1);
    n2.Connect(n1, n3, 2);
    n2.Connect(n3, n1, 2);
    n2.Connect(n5, n3, 2);
    n2.Connect(n5, n5, 0);

    n3.Connect(n1, n2, 1);
    n3.Connect(n2, n2, 0);
    n3.Connect(n4, n4, 0);
    n3.Connect(n4, n2, 2);
    n3.Connect(n1, n4, 1);
    n3.Connect(n2, n4, 2);
    n3.Connect(n5, n4, 1);

    n4.Connect(n1, n1, 0);
    n4.Connect(n2, n1, 1);
    n4.Connect(n2, n3, 1);
    n4.Connect(n3, n3, 0);
    n4.Connect(n1, n3, 2);
    n4.Connect(n3, n1, 2);
    n4.Connect(n5, n5, 0);

    Assert.AreEqual(n2, n1.Route(n2));
    Assert.AreEqual(n4, n1.Route(n4));
    Assert.IsTrue(new Node_Synch[] { n2, n4 }.Contains(n1.Route(n3)));

    Assert.AreEqual(n1, n2.Route(n1));
    Assert.AreEqual(n3, n2.Route(n3));
    Assert.IsTrue(new Node_Synch[] { n1, n3 }.Contains(n2.Route(n4)));

    Assert.AreEqual(n2, n3.Route(n2));
    Assert.AreEqual(n4, n3.Route(n4));
    Assert.IsTrue(new Node_Synch[] { n2, n4 }.Contains(n3.Route(n1)));

    Assert.AreEqual(n1, n4.Route(n1));
    Assert.AreEqual(n3, n4.Route(n3));
    Assert.IsTrue(new Node_Synch[] { n1, n3 }.Contains(n4.Route(n2)));

    //GetBestPathTo
    var path = n1.GetBestPathTo(n4);
    Assert.AreEqual(2, path.Count());
    Assert.AreEqual(n1, path.ElementAt(0) );
    Assert.AreEqual(n4, path.ElementAt(1) );
    
    path = n1.GetBestPathTo(n5);
    Assert.AreEqual(3, path.Count());
    Assert.AreEqual(n1, path.ElementAt(0) );
    Assert.AreEqual(n2, path.ElementAt(1) );
    Assert.AreEqual(n5, path.ElementAt(2) );
    //Assert.AreEqual(5, path.Count());
    //Assert.AreEqual(n1, path.ElementAt(0) );
    //Assert.AreEqual(n2, path.ElementAt(1) );
    //Assert.AreEqual(n3, path.ElementAt(2) );
    //Assert.AreEqual(n4, path.ElementAt(3) );
    //Assert.AreEqual(n5, path.ElementAt(4) );

    //GetAllPathTo
    var paths = n1.GetAllPathTo(n4);
    Assert.AreEqual(2, paths.Count());
    Assert.AreEqual(2, paths.ElementAt(0).Count());
    Assert.AreEqual(4, paths.ElementAt(1).Count());

    Assert.AreEqual(n1, paths.ElementAt(0).ElementAt(0));
    Assert.AreEqual(n4, paths.ElementAt(0).ElementAt(1));

    Assert.AreEqual(n1, paths.ElementAt(1).ElementAt(0));
    Assert.AreEqual(n2, paths.ElementAt(1).ElementAt(1));
    Assert.AreEqual(n3, paths.ElementAt(1).ElementAt(2));
    Assert.AreEqual(n4, paths.ElementAt(1).ElementAt(3));

    paths = n1.GetAllPathTo(n5);
    Assert.AreEqual(2, paths.Count());
    Assert.AreEqual(3, paths.ElementAt(0).Count());
    Assert.AreEqual(5, paths.ElementAt(1).Count());

    Assert.AreEqual(n1, paths.ElementAt(0).ElementAt(0));
    Assert.AreEqual(n2, paths.ElementAt(0).ElementAt(1));
    Assert.AreEqual(n5, paths.ElementAt(0).ElementAt(2));

    Assert.AreEqual(n1, paths.ElementAt(1).ElementAt(0));
    Assert.AreEqual(n2, paths.ElementAt(1).ElementAt(1));
    Assert.AreEqual(n3, paths.ElementAt(1).ElementAt(2));
    Assert.AreEqual(n4, paths.ElementAt(1).ElementAt(3));
    Assert.AreEqual(n5, paths.ElementAt(1).ElementAt(4));

    //Ovveride RoutingMethod
    var newRoutingMethod =
      (Dictionary<Node_Synch, int> possibleNextNodes) =>
      {
        return possibleNextNodes.OrderByDescending(n => n.Value).First().Key;
      };

    n1.RoutingMethod = newRoutingMethod;
    n2.RoutingMethod = newRoutingMethod;
    n3.RoutingMethod = newRoutingMethod;
    n4.RoutingMethod = newRoutingMethod;

    Assert.AreEqual(n4, n1.Route(n2));
    Assert.AreEqual(n2, n1.Route(n4));
    Assert.IsTrue(new Node_Synch[] { n2, n4 }.Contains(n1.Route(n3)));

    Assert.AreEqual(n3, n2.Route(n1));
    Assert.AreEqual(n1, n2.Route(n3));
    Assert.IsTrue(new Node_Synch[] { n1, n3 }.Contains(n2.Route(n4)));

    Assert.AreEqual(n4, n3.Route(n2));
    Assert.AreEqual(n2, n3.Route(n4));
    Assert.IsTrue(new Node_Synch[] { n2, n4 }.Contains(n3.Route(n1)));

    Assert.AreEqual(n3, n4.Route(n1));
    Assert.AreEqual(n1, n4.Route(n3));
    Assert.IsTrue(new Node_Synch[] { n1, n3 }.Contains(n4.Route(n2)));
  }

  [TestMethod]
  public async Task TestNodeRoutingMethod_Async()
  {
    var n1 = new Node(){Name="n1"};
    var n2 = new Node(){Name="n2"};
    var n3 = new Node(){Name="n3"};
    var n4 = new Node(){Name="n4"};
    var n5 = new Node(){Name="n5"};


    await n1.ConnectAsync(n2);
    await n1.ConnectAsync(n4);

    await n2.ConnectAsync(n3);
    await n2.ConnectAsync(n5);

    await n3.ConnectAsync(n2);
    await n3.ConnectAsync(n4);

    await n4.ConnectAsync(n1);
    await n4.ConnectAsync(n3);
    await n4.ConnectAsync(n5);

    //await n1.ConnectAsync(n2, n2);
    //await n1.ConnectAsync(n3, n2);
    //await n1.ConnectAsync(n4, n2);
    //await n1.ConnectAsync(n2, n4);
    //await n1.ConnectAsync(n3, n4);
    //await n1.ConnectAsync(n4, n4);
    //await n1.ConnectAsync(n5, n2);

    //await n2.ConnectAsync(n1, n1);
    //await n2.ConnectAsync(n3, n3);
    //await n2.ConnectAsync(n4, n3);
    //await n2.ConnectAsync(n4, n1);
    //await n2.ConnectAsync(n1, n3);
    //await n2.ConnectAsync(n3, n1);
    //await n2.ConnectAsync(n5, n3);
    //await n2.ConnectAsync(n5, n5);

    //await n3.ConnectAsync(n1, n2);
    //await n3.ConnectAsync(n2, n2);
    //await n3.ConnectAsync(n4, n4);
    //await n3.ConnectAsync(n4, n2);
    //await n3.ConnectAsync(n1, n4);
    //await n3.ConnectAsync(n2, n4);
    //await n3.ConnectAsync(n5, n4);

    //await n4.ConnectAsync(n1, n1);
    //await n4.ConnectAsync(n2, n1);
    //await n4.ConnectAsync(n2, n3);
    //await n4.ConnectAsync(n3, n3);
    //await n4.ConnectAsync(n1, n3);
    //await n4.ConnectAsync(n3, n1);
    //await n4.ConnectAsync(n5, n5);

    Assert.AreEqual(n2, await n1.RouteAsync(n2));
    Assert.AreEqual(n4, await n1.RouteAsync(n4));
    Assert.IsTrue(new Node[] { n2, n4 }.Contains(await n1.RouteAsync(n3)));

    Assert.AreEqual(n1, await n2.RouteAsync(n1));
    Assert.AreEqual(n3, await n2.RouteAsync(n3));
    Assert.IsTrue(new Node[] { n1, n3 }.Contains(await n2.RouteAsync(n4)));

    Assert.AreEqual(n2, await n3.RouteAsync(n2));
    Assert.AreEqual(n4, await n3.RouteAsync(n4));
    Assert.IsTrue(new Node[] { n2, n4 }.Contains(await n3.RouteAsync(n1)));

    Assert.AreEqual(n1, await n4.RouteAsync(n1));
    Assert.AreEqual(n3, await n4.RouteAsync(n3));
    Assert.IsTrue(new Node[] { n1, n3 }.Contains(await n4.RouteAsync(n2)));

    //GetBestPathTo
    var path = await n1.GetBestPathToAsync(n4);
    Assert.AreEqual(2, path.Count());
    Assert.AreEqual(n1, path.ElementAt(0) );
    Assert.AreEqual(n4, path.ElementAt(1) );
    
    path = await n1.GetBestPathToAsync(n5);
    Assert.AreEqual(3, path.Count());
    Assert.AreEqual(n1, path.ElementAt(0) );
    Assert.AreEqual(n2, path.ElementAt(1) );
    Assert.AreEqual(n5, path.ElementAt(2) );
    //Assert.AreEqual(5, path.Count());
    //Assert.AreEqual(n1, path.ElementAt(0) );
    //Assert.AreEqual(n2, path.ElementAt(1) );
    //Assert.AreEqual(n3, path.ElementAt(2) );
    //Assert.AreEqual(n4, path.ElementAt(3) );
    //Assert.AreEqual(n5, path.ElementAt(4) );

    //GetAllPathTo
    var paths = await n1.GetAllPathToAsync(n4);
    Assert.AreEqual(2, paths.Count());
    Assert.AreEqual(2, paths.ElementAt(0).Count());
    Assert.AreEqual(4, paths.ElementAt(1).Count());

    Assert.AreEqual(n1, paths.ElementAt(0).ElementAt(0));
    Assert.AreEqual(n4, paths.ElementAt(0).ElementAt(1));

    Assert.AreEqual(n1, paths.ElementAt(1).ElementAt(0));
    Assert.AreEqual(n2, paths.ElementAt(1).ElementAt(1));
    Assert.AreEqual(n3, paths.ElementAt(1).ElementAt(2));
    Assert.AreEqual(n4, paths.ElementAt(1).ElementAt(3));

    paths = await n1.GetAllPathToAsync(n5);
    Assert.AreEqual(2, paths.Count());
    Assert.AreEqual(3, paths.ElementAt(0).Count());
    Assert.AreEqual(5, paths.ElementAt(1).Count());

    Assert.AreEqual(n1, paths.ElementAt(0).ElementAt(0));
    Assert.AreEqual(n2, paths.ElementAt(0).ElementAt(1));
    Assert.AreEqual(n5, paths.ElementAt(0).ElementAt(2));

    Assert.AreEqual(n1, paths.ElementAt(1).ElementAt(0));
    Assert.AreEqual(n2, paths.ElementAt(1).ElementAt(1));
    Assert.AreEqual(n3, paths.ElementAt(1).ElementAt(2));
    Assert.AreEqual(n4, paths.ElementAt(1).ElementAt(3));
    Assert.AreEqual(n5, paths.ElementAt(1).ElementAt(4));

    //Ovveride RoutingMethod
    var newRoutingMethod =
      async (Dictionary<Node, int> possibleNextNodes) =>
      {
        return
        await Task.Run(() =>
        {
          var result = possibleNextNodes.OrderByDescending(n => n.Value).First();
          return new RoutingResult(result.Key, result.Value);
        });
      };

    n1.RoutingMethodAsync = newRoutingMethod;
    n2.RoutingMethodAsync = newRoutingMethod;
    n3.RoutingMethodAsync = newRoutingMethod;
    n4.RoutingMethodAsync = newRoutingMethod;

    Assert.AreEqual(n4, await n1.RouteAsync(n2));
    Assert.AreEqual(n2, await n1.RouteAsync(n4));
    Assert.IsTrue(new Node[] { n2, n4 }.Contains(await n1.RouteAsync(n3)));

    Assert.AreEqual(n3, await n2.RouteAsync(n1));
    Assert.AreEqual(n1, await n2.RouteAsync(n3));
    Assert.IsTrue(new Node[] { n1, n3 }.Contains(await n2.RouteAsync(n4)));

    Assert.AreEqual(n4, await n3.RouteAsync(n2));
    Assert.AreEqual(n2, await n3.RouteAsync(n4));
    Assert.IsTrue(new Node[] { n2, n4 }.Contains(await n3.RouteAsync(n1)));

    Assert.AreEqual(n3, await n4.RouteAsync(n1));
    Assert.AreEqual(n1, await n4.RouteAsync(n3));
    Assert.IsTrue(new Node[] { n1, n3 }.Contains(await n4.RouteAsync(n2)));
  }

}