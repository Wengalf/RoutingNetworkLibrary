// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoutingNetworkLibrary;

var summary = BenchmarkRunner.Run<MyBenchmark>();

public class MyBenchmark
{
    //[GlobalSetup]
    //public void Setup()
    //{
    //  node = new Node();
    //  destination = new Node();
    //  // Hier können Sie Ihren Node und Destination initialisieren
    //}

    [Benchmark]
    public void Test()
    {
    }
}