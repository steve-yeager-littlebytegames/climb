using System;
using System.Collections;
using System.Collections.Generic;
using Climb.Models;
using Climb.Services;
using Climb.Test.Extensions;
using NUnit.Framework;

namespace Climb.Test.Services
{
    [TestFixture]
    public class BracketGeneratorTest
    {
        private BracketGenerator testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new BracketGenerator();
        }

        [Test]
        public void Generate_NotEnoughUsers_ThrowException()
        {
            var userCount = testObj.MinCompetitors - 1;
            var users = new List<TournamentUser>().Init(userCount);

            Assert.Throws<ArgumentException>(() => testObj.Generate(users));
        }
    }
}