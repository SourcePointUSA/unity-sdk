using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace AltTester.AltTesterUnitySDK.Tests
{
    public class AltObjectIdTests
    {
        [Test]
        public void GetReturnsStableUniqueProtocolIdsForDifferentUnityObjects()
        {
            var first = new GameObject("AltObjectIdTests.First");
            var second = new GameObject("AltObjectIdTests.Second");

            try
            {
                var get = typeof(AltRunner).Assembly
                    .GetType("AltTester.AltTesterUnitySDK.AltObjectId")
                    .GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                var firstId = (int)get.Invoke(null, new object[] { first });
                var secondId = (int)get.Invoke(null, new object[] { second });

                Assert.That(firstId, Is.Not.EqualTo(0));
                Assert.That(secondId, Is.Not.EqualTo(0));
                Assert.That(get.Invoke(null, new object[] { first }), Is.EqualTo(firstId));
                Assert.That(secondId, Is.Not.EqualTo(firstId));
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
            }
        }
    }
}
