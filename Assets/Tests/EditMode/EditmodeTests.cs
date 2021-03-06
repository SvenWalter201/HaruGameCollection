using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Mathf;
using System;

public class EditmodeTests
{

    #region EXTENSION_TESTS
    [Test]
    public void PerpendicularVector_001()
    {
        Vector3 v3 = new Vector3(0, 0, 1).normalized;
        Assert.Less(Abs(Vector3.Dot(v3, v3.GetPerpendicular().normalized)), 0.001f);
    }

    [Test]
    public void PerpendicularVector_002()
    {
        Vector3 v3 = new Vector3(0, 0, 1).normalized;
        Assert.Less(Abs(Vector3.Dot(v3, v3.GetPerpendicular().normalized)), 0.001f);
    }

    [Test]
    public void PerpendicularVector_003()
    {
        Vector3 v3 = new Vector3(0, 1, 0).normalized;
        Assert.Less(Abs(Vector3.Dot(v3, v3.GetPerpendicular().normalized)), 0.001f);
    }

    [Test]
    public void PerpendicularVector_004()
    {
        Vector3 v3 = new Vector3(-5, 0, -9).normalized;
        Assert.Less(Abs(Vector3.Dot(v3, v3.GetPerpendicular().normalized)), 0.001f);
    }
    [Test]
    public void PerpendicularVector_005()
    {
        Vector3 v3 = new Vector3(-5, 5, 2).normalized;

        Assert.Less(Abs(Vector3.Dot(v3, v3.GetPerpendicular().normalized)), 0.001f);
    }

    [Test]
    public void TestBoundingBox_001()
    {
        List<Vector3> v3s = new List<Vector3>() {
            new Vector3(1,1,1),
            new Vector3(3,3,3)
        };

        Bounds v3s_Bounds = v3s.GetBoundingBox();

        Assert.AreEqual(v3s_Bounds.center, new Vector3(2, 2, 2), "bounding box center test failed");
        Assert.AreEqual(v3s_Bounds.min, new Vector3(1, 1, 1), "bounding box min test failed");
        Assert.AreEqual(v3s_Bounds.max, new Vector3(3, 3, 3), "bounding box max test failed");
    }

    [Test]
    public void TestBoundingBox_002()
    {
        List<Vector3> v3s = new List<Vector3>() {
            new Vector3(-2,-4,-6),
            new Vector3(3,-6,-4)
        };

        Bounds v3s_Bounds = v3s.GetBoundingBox();

        Assert.AreEqual(v3s_Bounds.center, new Vector3(0.5f, -5, -5), "bounding box center test failed");
        Assert.AreEqual(v3s_Bounds.min, new Vector3(-2, -6, -6), "bounding box min test failed");
        Assert.AreEqual(v3s_Bounds.max, new Vector3(3, -4, -4), "bounding box max test failed");
    }

    [Test]
    public void TestBoundingBox_003()
    {
        List<Vector3> v3s = new List<Vector3>() {
            new Vector3(1,3,7),
            new Vector3(-6,5,3),
            new Vector3(0,9,-4),
        };

        Bounds v3s_Bounds = v3s.GetBoundingBox();

        Assert.AreEqual(v3s_Bounds.center, new Vector3(-2.5f, 6, 1.5f), "bounding box center test failed");
        Assert.AreEqual(v3s_Bounds.min, new Vector3(-6, 3, -4), "bounding box min test failed");
        Assert.AreEqual(v3s_Bounds.max, new Vector3(1, 9, 7), "bounding box max test failed");
    }

    [Test]
    public void TestSphereIntersection_001()
    {
        Bounds b = new Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        Vector3 sphereCenter = new Vector3(0, 0, 0);
        float sphereRadius = 1f;
        Assert.IsTrue(b.IntersectSphere(sphereCenter, sphereRadius));
        Assert.IsTrue(b.IntersectXZCircle(sphereCenter, sphereRadius));
    }

    [Test]
    public void TestSphereIntersection_002()
    {
        Bounds b = new Bounds(new Vector3(0, 2, 0), new Vector3(1, 1, 1));
        Vector3 sphereCenter = new Vector3(0, 0, 0);
        float sphereRadius = 1f;
        Assert.IsFalse(b.IntersectSphere(sphereCenter, sphereRadius));
        Assert.IsTrue(b.IntersectXZCircle(sphereCenter, sphereRadius));
    }

    [Test]
    public void TestSphereIntersection_003()
    {
        Bounds b = new Bounds(new Vector3(0, -2, 0), new Vector3(1, 1, 1));
        Vector3 sphereCenter = new Vector3(0, 0, 0);
        float sphereRadius = 1f;
        Assert.IsFalse(b.IntersectSphere(sphereCenter, sphereRadius));
        Assert.IsTrue(b.IntersectXZCircle(sphereCenter, sphereRadius));
    }

    [Test]
    public void TestSphereIntersection_004()
    {
        Bounds b = new Bounds(new Vector3(-2, -2, -2), new Vector3(2.2f, 2.2f, 2.2f));
        Vector3 sphereCenter = new Vector3(0, 0, 0);
        float sphereRadius = 1f;
        Assert.IsFalse(b.IntersectSphere(sphereCenter, sphereRadius));
        Assert.IsFalse(b.IntersectXZCircle(sphereCenter, sphereRadius));
    }

    [Test]
    public void TestSphereIntersection_005()
    {
        Bounds b = new Bounds(new Vector3(-2, -2, -2), new Vector3(3f, 3f, 3f));
        Vector3 sphereCenter = new Vector3(0, 0, 0);
        float sphereRadius = 1f;
        Assert.IsTrue(b.IntersectSphere(sphereCenter, sphereRadius));
        Assert.IsTrue(b.IntersectXZCircle(sphereCenter, sphereRadius));
    }
    #endregion

    #region GRAMMAR_UTILITY_TEST

    [Test]
    public void TestLookForPriority_001()//testing if equals ture for correct cases
    {
        string[] priorityKeywords = new string[] { "thing", "animal", "person" };

        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @animal@ @position@ @action@", priorityKeywords), "animal");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @person@ @position@ @action@", priorityKeywords), "person");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @thing@ @position@ @action@", priorityKeywords), "thing");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @position@ @action@ @animal@", priorityKeywords), "animal");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @position@ @action@ @person@", priorityKeywords), "person");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @position@ @action@ @thing@", priorityKeywords), "thing");
    }

    [Test]
    public void TestLookForPriority_002()//testing if not equal for incorrect cases
    {
        string[] priorityKeywords = new string[] { "thing", "animal", "person" };

        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @animal@ @position@ @action@", priorityKeywords), "setting");
        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @animal@ @position@ @action@", priorityKeywords), "position");
        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @animal@ @position@ @action@", priorityKeywords), "action");
        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @person@ @position@ @action@", priorityKeywords), "setting");
        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @thing@ @position@ @action@", priorityKeywords), "setting");
    }

    [Test]
    public void TestLookForPriority_003()//testing if equal when no prio words
    {
        string[] priorityKeywords = new string[] { "thing", "animal", "person" };

        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @description@ @position@ @action@", priorityKeywords), "setting");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @action@, wobei {ein} @description@ @position@ @action@", priorityKeywords), "action");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @word@, wobei {ein} @description@ @position@ @action@", priorityKeywords), "word");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @description@, wobei {ein} @description@ @position@ @action@", priorityKeywords), "description");
        Assert.AreEqual(Helper.LookForPriority("Das ganze spielt @linkingword@, wobei {ein} @description@ @position@ @action@", priorityKeywords), "linkingword");
    }

    [Test]
    public void TestLookForPriority_004()//
    {
        string[] priorityKeywords = new string[] { "thing", "animal", "person" };

        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @description@ Haru @position@ @action@", priorityKeywords), "animal");
        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @description@ Lehrer @position@ @action@", priorityKeywords), "thing");
        Assert.AreNotEqual(Helper.LookForPriority("Das ganze spielt @setting@, wobei {ein} @description@ Haus @position@ @action@", priorityKeywords), "person");
    }

    [Test]
    public void TestFirstToUppercase_001()//
    {

        Assert.AreEqual("Das ganze spielt",Helper.UpperCase("Das ganze spielt"));
        Assert.AreEqual("Das ganze spielt", Helper.UpperCase("das ganze spielt"));
        Assert.AreEqual("Das ganze spielt",Helper.UpperCase(" Das ganze spielt    "));
        
        Assert.AreEqual("Das ganze spielt",Helper.UpperCase("  das ganze spielt"));
    }

    [Test]
    public void TestFirstToUppercase_002()//
    {

        Assert.AreNotEqual(Helper.UpperCase("Das ganze spielt"), " Daas ganze spielt");
        Assert.AreNotEqual(Helper.UpperCase("  Das ganze spielt"), "  Das ganze spielt");
        Assert.AreNotEqual(Helper.UpperCase("das ganze spielt"), "das ganze spielt");
        Assert.AreNotEqual(Helper.UpperCase("  das ganze spielt"), "  das ganze spielt");
    }



    #endregion

}