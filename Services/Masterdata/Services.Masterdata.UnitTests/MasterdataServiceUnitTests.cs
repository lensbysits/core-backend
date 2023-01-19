using Lens.Services.Masterdata.Models;
using System.Text.Json;

namespace Lens.Services.Masterdata.UnitTests
{
    public class MasterdataServiceUnitTests : IClassFixture<MasterdataServiceFixture>
    {
        private readonly MasterdataServiceFixture _fixture;

        public MasterdataServiceUnitTests(MasterdataServiceFixture fixture)
        {
            _fixture = fixture;
        }
        [Theory]
        [InlineData("123", "Name", "Description", "{ \"someKey\": \"some value\" }", "123", "Name", "Description", "{ \"someKey\": \"some value\" }")]
        [InlineData("123", "Name", "This is bad: <script>alert('xss')</script>, should be sanitized", "{ \"someKey\": \"some value\" }", "123", "Name", "This is bad: , should be sanitized", "{ \"someKey\": \"some value\" }")]
        [InlineData("123", "<script>alert('xss')</script>Name", "Description", "{ \"someKey\": \"some value\" }", "123", "Name", "Description", "{ \"someKey\": \"some value\" }")]
        [InlineData("123", "Name", "Description", "{ \"someKey\": \"<script>alert('xss')</script>\" }", "123", "Name", "Description", "{ \"someKey\": \"\" }")]
        public async Task SanitizeMasterdataTypeCreateModelTest(string code, string name, string description, string metadata, 
                                                                string expectedCode, string expectedName, string expectedDescription, string expectedMetadata)
        {
            var metaDataNode = JsonSerializer.Deserialize<dynamic>(metadata);

            var model = new MasterdataTypeCreateModel
            {
                Code = code,
                Name = name,
                Description = description,
                Metadata = metaDataNode
            };

            _fixture.MasterdataRepositoryMock.Setup(repository => repository.AddMasterdataType(It.IsAny<MasterdataTypeCreateModel>()))
                .Callback((MasterdataTypeCreateModel callbackModel) =>
                {
                    callbackModel.Code.Should().Be(expectedCode);
                    callbackModel.Name.Should().Be(expectedName);
                    callbackModel.Description.Should().Be(expectedDescription);
                    callbackModel.Metadata?.ToString().Should().Be(expectedMetadata);

                });

            var result = await _fixture.MasterdataService.AddMasterdataType(model);
            Assert.True(true);
        }
    }
}