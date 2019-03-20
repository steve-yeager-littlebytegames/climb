using System.Net;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Climb.Test.Utilities
{
    public static class ControllerUtility
    {
        public static void AssertStatusCode(IActionResult result, HttpStatusCode statusCode)
        {
            var reponse = (ObjectResult)result;
            Assert.AreEqual((int)statusCode, reponse.StatusCode);
        }
    }
}