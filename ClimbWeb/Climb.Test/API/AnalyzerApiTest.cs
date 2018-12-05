<<<<<<< master
<<<<<<< master
﻿using System;
using System.Net;
=======
﻿using System.Net;
>>>>>>> Setup class and start API tests
=======
﻿using System;
using System.Net;
>>>>>>> Successful test
using System.Threading.Tasks;
using Climb.API;
using Climb.Exceptions;
using Climb.Services;
using Climb.Test.Utilities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Climb.Test.API
{
    [TestFixture]
    public class AnalyzerApiTest
    {
        private AnalyzerApi testObj;
        private IAnalyzerService analyzerService;

        [SetUp]
        public void SetUp()
        {
            var logger = Substitute.For<ILogger<AnalyzerApi>>();
            analyzerService = Substitute.For<IAnalyzerService>();

            testObj = new AnalyzerApi(logger, analyzerService);
        }

        [Test]
        public async Task Get_NoPlayer_NotFoundResult()
        {
            analyzerService.Calculate(0, 0).ThrowsForAnyArgs<NotFoundException>();

            var result = await testObj.Analyze(-1, -2);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.NotFound);
        }

        
        [Test]
        public async Task Get_SamePlayer_BadRequestResult()
        {
            analyzerService.Calculate(0, 0).ThrowsForAnyArgs<NotFoundException>();

            var result = await testObj.Analyze(-1, -1);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.BadRequest);
        }
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Successful test

        [Test]
        public async Task Get_Success_OkResult()
        {
            const int p1ID = 1;
            const int p2ID = 2;
            analyzerService.Calculate(p1ID, p2ID).Returns(new AnalyzerDataCollection(p1ID, p2ID, DateTime.MinValue));

            var result = await testObj.Analyze(p1ID, p2ID);

            ControllerUtility.AssertStatusCode(result, HttpStatusCode.OK);
        }
<<<<<<< master
=======
>>>>>>> Setup class and start API tests
=======
>>>>>>> Successful test
    }
}