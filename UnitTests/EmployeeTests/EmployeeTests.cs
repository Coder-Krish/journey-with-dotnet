using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace UnitTests.EmployeeTests;

[TestFixture("Chrome")]
public class EmployeeTests
{
    private string _browser;
    private string _version;
    private string _os;

    public EmployeeTests(string browser, string version, string os)
    {
        _browser = browser;
        _version = version;
        _os = os;
    }

    [SetUp]
    public void Setup()
    {
        Console.WriteLine("Initialization is done here");
        Console.WriteLine($"Printing Parameters {_browser}:{_version}{_os}");
    }

    [TearDown]
    public void ClearUp()
    {
    }

    [Test]
    public void Test1()
    {
        Console.WriteLine("Test can run here");
        Assert.Pass();
    }


}
