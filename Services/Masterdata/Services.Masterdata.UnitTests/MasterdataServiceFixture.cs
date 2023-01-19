using Lens.Core.Lib.Services;
using Lens.Services.Masterdata.Repositories;
using Lens.Services.Masterdata.Services;
using Moq;

namespace Lens.Services.Masterdata.UnitTests;

public class MasterdataServiceFixture : IDisposable
{
    public MasterdataService MasterdataService { get; private set; }
    public Mock<IMasterdataRepository> MasterdataRepositoryMock { get; private set; }

    public MasterdataServiceFixture()
    {
        var applicationService = Mock.Of<IApplicationService<MasterdataService>>();
        MasterdataRepositoryMock = new Mock<IMasterdataRepository>();
        MasterdataService = new MasterdataService(applicationService, MasterdataRepositoryMock.Object, new Ganss.Xss.HtmlSanitizer());
    }

    public void Dispose()
    {
    }
}

