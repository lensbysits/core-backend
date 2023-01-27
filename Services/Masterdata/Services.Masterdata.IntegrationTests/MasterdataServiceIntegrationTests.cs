using FluentAssertions;
using Lens.Services.Masterdata.IntegrationTests;


namespace Services.Masterdata.IntegrationTests
{
    public class MasterdataServiceIntegrationTests : IClassFixture<MasterdataServiceFixture>
    {
        private readonly MasterdataServiceFixture _fixture;

        public MasterdataServiceIntegrationTests(MasterdataServiceFixture masterdataServiceFixture)
        {
            _fixture = masterdataServiceFixture;
        }

        [Fact]
        public async Task GetTagsUsingShadowProperty()
        {
            await _fixture.Init();
            var result = await _fixture.MasterdataService.GetMasterdata(_fixture.MasterdataType!.Code!, _fixture.Masterdatas!.Value!.First().Key!);
            result?.Tags.Should().NotBeEmpty();
        }
    }
}