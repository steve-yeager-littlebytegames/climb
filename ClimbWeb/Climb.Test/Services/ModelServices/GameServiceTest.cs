using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Exceptions;
using Climb.Requests.Games;
using Climb.Services;
using Climb.Services.ModelServices;
using Climb.Test.Utilities;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Climb.Test.Services.ModelServices
{
    [TestFixture]
    public class GameServiceTest
    {
        private GameService testObj;
        private ApplicationDbContext dbContext;
        private ICdnService cdnService;

        [SetUp]
        public void SetUp()
        {
            dbContext = DbContextUtility.CreateMockDb();
            cdnService = Substitute.For<ICdnService>();

            testObj = new GameService(dbContext, cdnService);
        }

        [Test]
        public async Task Update_Valid_ReturnGame()
        {
            var logoFile = Substitute.For<IFormFile>();
            var request = new UpdateRequest("GameName", 1, 2, true, logoFile);

            var game = await testObj.Update(request);

            Assert.IsNotNull(game);
        }

        [Test]
        public void Update_NameTaken_Conflict()
        {
            var logoFile = Substitute.For<IFormFile>();
            var gameOld = GameUtility.Create(dbContext, 1, 1);
            var createRequest = new UpdateRequest(gameOld.Name, 1, 2, true, logoFile);

            Assert.ThrowsAsync<ConflictException>(() => testObj.Update(createRequest));

            var game = GameUtility.Create(dbContext, 0, 0, name:"GameOther");
            var updateRequest = new UpdateRequest(gameOld.Name, 1, 2, true, null)
            {
                GameID = game.ID,
            };

            Assert.ThrowsAsync<ConflictException>(() => testObj.Update(updateRequest));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Update_InvalidMaxCharacters_BadRequestException(int maxCharacters)
        {
            var logoFile = Substitute.For<IFormFile>();
            var createRequest = new UpdateRequest("GameName", maxCharacters, 2, true, logoFile);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Update(createRequest));

            var game = GameUtility.Create(dbContext, 0, 0);
            var updateRequest = new UpdateRequest("GameName", maxCharacters, 2, true, logoFile)
            {
                GameID = game.ID,
            };

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Update(updateRequest));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Update_InvalidMaxMatchPoints_BadRequestException(int maxPoints)
        {
            var logoFile = Substitute.For<IFormFile>();
            var createRequest = new UpdateRequest("GameName", 1, maxPoints, true, logoFile);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Update(createRequest));

            var game = GameUtility.Create(dbContext, 0, 0);
            var updateRequest = new UpdateRequest("GameName", 1, maxPoints, true, logoFile)
            {
                GameID = game.ID,
            };

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Update(updateRequest));
        }

        [Test]
        public void Update_NewGameNoLogo_BadRequest()
        {
            var request = new UpdateRequest("Name", 1, 2, false, null);

            Assert.ThrowsAsync<BadRequestException>(() => testObj.Update(request));
        }

        [Test]
        public async Task AddCharacter_Valid_Character()
        {
            var game = GameUtility.Create(dbContext, 0, 0);
            var imageFile = Substitute.For<IFormFile>();

            var character = await testObj.AddCharacter(game.ID, null, "Char1", imageFile);

            Assert.IsNotNull(character);
        }

        [Test]
        public void AddCharacter_NoGame_NotFoundException()
        {
            var imageFile = Substitute.For<IFormFile>();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.AddCharacter(0, null, "Char1", imageFile));
        }

        [Test]
        public void AddCharacter_NameTaken_ConflictException()
        {
            var game = GameUtility.Create(dbContext, 1, 0);
            var imageFile = Substitute.For<IFormFile>();

            Assert.ThrowsAsync<ConflictException>(() => testObj.AddCharacter(game.ID, null, game.Characters[0].Name, imageFile));
        }

        [Test]
        public void AddCharacter_NewCharacterNoImageKey_NullArgumentException()
        {
            var game = GameUtility.Create(dbContext, 0, 0);

            Assert.ThrowsAsync<ArgumentNullException>(() => testObj.AddCharacter(game.ID, null, "Char1", null));
        }

        [Test]
        public async Task AddCharacter_NewCharacter_UploadImage()
        {
            var game = GameUtility.Create(dbContext, 1, 0);
            var imageFile = Substitute.For<IFormFile>();

            await testObj.AddCharacter(game.ID, null, "Char1", imageFile);

            await cdnService.Received(1).UploadImageAsync(imageFile, ClimbImageRules.CharacterPic);
        }

        [Test]
        public async Task AddCharacter_OldCharacterNoImage_ImageKeyNotUpdated()
        {
            var game = GameUtility.Create(dbContext, 1, 0);
            var imageKey = game.Characters[0].ImageKey;

            var character = await testObj.AddCharacter(game.ID, game.Characters[0].ID, "Char1", null);

            Assert.AreEqual(imageKey, character.ImageKey);
        }

        [Test]
        public async Task AddCharacter_OldCharacterNewImage_NewImageUploaded()
        {
            var game = GameUtility.Create(dbContext, 1, 0);
            var imageFile = Substitute.For<IFormFile>();

            await testObj.AddCharacter(game.ID, game.Characters[0].ID, "Char1", imageFile);

            await cdnService.Received(1).ReplaceImageAsync(Arg.Any<string>(), imageFile, ClimbImageRules.CharacterPic);
        }

        [Test]
        public async Task AddCharacter_OldCharacterNewImage_ImageKeySaved()
        {
            var game = GameUtility.Create(dbContext, 1, 0);
            var imageFile = Substitute.For<IFormFile>();
            const string imageKey = "key";
            cdnService.ReplaceImageAsync(Arg.Any<string>(), imageFile, ClimbImageRules.CharacterPic).Returns(imageKey);

            var character = await testObj.AddCharacter(game.ID, game.Characters[0].ID, "Char1", imageFile);

            Assert.AreEqual(imageKey, character.ImageKey);
        }

        [Test]
        public async Task AddCharacter_OldCharacterNoImage_ValuesUpdated()
        {
            const string name = "NewName";
            var game = GameUtility.Create(dbContext, 1, 0);

            var character = await testObj.AddCharacter(game.ID, game.Characters[0].ID, name, null);

            Assert.AreEqual(name, character.Name);
        }

        [Test]
        public void AddCharacter_HasCharacterIDButNoCharacter_NotFoundException()
        {
            var game = GameUtility.Create(dbContext, 0, 0);
            var imageFile = Substitute.For<IFormFile>();

            Assert.ThrowsAsync<NotFoundException>(() => testObj.AddCharacter(game.ID, 1, "Char1", imageFile));
        }

        [Test]
        public async Task AddStage_Valid_Stage()
        {
            var game = GameUtility.Create(dbContext, 0, 0);

            var stage = await testObj.AddStage(game.ID, null, "Stage1");

            Assert.IsNotNull(stage);
        }

        [Test]
        public void AddStage_NoGame_NotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => testObj.AddStage(0, null, "Stage1"));
        }

        [Test]
        public void AddStage_NameTaken_ConflictException()
        {
            var game = GameUtility.Create(dbContext, 0, 1);

            Assert.ThrowsAsync<ConflictException>(() => testObj.AddStage(game.ID, null, game.Stages[0].Name));
        }
        
        [Test]
        public void AddStage_HasStageIDButNoStage_NotFoundException()
        {
            var game = GameUtility.Create(dbContext, 0, 0);

            Assert.ThrowsAsync<NotFoundException>(() => testObj.AddStage(game.ID, 1, "Stage1"));
        }

        [Test]
        public async Task AddStage_OldStage_ValuesUpdated()
        {
            const string name = "NewName";
            var game = GameUtility.Create(dbContext, 0, 1);

            var stage = await testObj.AddStage(game.ID, game.Stages[0].ID, name);

            Assert.AreEqual(name, stage.Name);
        }
    }
}