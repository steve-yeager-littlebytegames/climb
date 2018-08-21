using System;
using System.Threading.Tasks;
using Climb.Services;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services
{
    [TestFixture]
    public class CdnServiceTest
    {
        private class FakeCdnService : CdnService
        {
            public int deleteCalls;

            public FakeCdnService()
            {
                root = "https://www.climb.com/";
            }

            protected override Task UploadImageInternalAsync(IFormFile image, string folder, string fileKey)
            {
                return Task.CompletedTask;
            }

            protected override void EnsureFolder(string rulesFolder)
            {
            }

            public override Task DeleteImageAsync(string fileKey, ImageRules rules)
            {
                ++deleteCalls;
                return Task.CompletedTask;
            }
        }

        private FakeCdnService testObj;

        [SetUp]
        public void SetUp()
        {
            testObj = new FakeCdnService();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GetImageUrl_MissingKey_ReturnMissingUrl(string key)
        {
            const string missingUrl = "missingURL";
            var rules = new ImageRules(1, 1, 1, "folder", missingUrl);

            var url = testObj.GetImageUrl(key, rules);

            Assert.AreEqual(missingUrl, url);
        }

        [Test]
        public void GetImageUrl_HasKey_ReturnUrl()
        {
            const string missingUrl = "missingURL";
            var rules = new ImageRules(1, 1, 1, "folder", missingUrl);

            var url = testObj.GetImageUrl("Key", rules);

            Assert.AreNotEqual(missingUrl, url);
        }

        [Test]
        public void UploadImage_NotValid_ArgumentException()
        {
            var image = Substitute.For<IFormFile>();
            image.Length.Returns(2);
            var rules = new ImageRules(1, 0, 0, "", "");

            Assert.ThrowsAsync<ArgumentException>(() => testObj.UploadImageAsync(image, rules));
        }

        [Test]
        public async Task ReplaceImage_HasOldKey_DeleteCalled()
        {
            var image = Substitute.For<IFormFile>();
            var rules = new ImageRules(0, 0, 0, "", "");

            await testObj.ReplaceImageAsync("asdf", image, rules);

            Assert.AreEqual(1, testObj.deleteCalls);
        }

        [Test]
        public async Task ReplaceImage_NoOldKey_DeleteNotCalled()
        {
            var image = Substitute.For<IFormFile>();
            var rules = new ImageRules(0, 0, 0, "", "");

            await testObj.ReplaceImageAsync("", image, rules);

            Assert.AreEqual(0, testObj.deleteCalls);
        }

        [Test]
        public void GetUserProfilePicUrl_HasKey_ReturnUrlToImage()
        {
            const string imageKey = "key.png";
            
            var url = testObj.GetUserProfilePicUrl("123", imageKey, ClimbImageRules.ProfilePic);

            Assert.IsTrue(url.EndsWith(imageKey));
            Assert.IsFalse(url.StartsWith("/"));
        }

        [Test]
        public void GetUserProfilePicUrl_NoKey_ReturnTempImage()
        {
            var url = testObj.GetUserProfilePicUrl("123", "", ClimbImageRules.ProfilePic);

            Assert.IsTrue(url.StartsWith("/"));
        }
    }
}