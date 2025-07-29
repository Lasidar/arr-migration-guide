using Moq;
using NUnit.Framework;
using Readarr.Common.EnvironmentInfo;
using Readarr.Core.HealthCheck.Checks;
using Readarr.Core.Localization;
using Readarr.Core.Test.Framework;
using Readarr.Test.Common;

namespace Readarr.Core.Test.HealthCheck.Checks
{
    [TestFixture]
    public class AppDataLocationFixture : CoreTest<AppDataLocationCheck>
    {
        [SetUp]
        public void Setup()
        {
            Mocker.GetMock<ILocalizationService>()
                  .Setup(s => s.GetLocalizedString(It.IsAny<string>()))
                  .Returns("Some Warning Message");
        }

        [Test]
        public void should_return_warning_when_app_data_is_child_of_startup_folder()
        {
            Mocker.GetMock<IAppFolderInfo>()
                  .Setup(s => s.StartUpFolder)
                  .Returns(@"C:\NzbDrone".AsOsAgnostic());

            Mocker.GetMock<IAppFolderInfo>()
                  .Setup(s => s.AppDataFolder)
                  .Returns(@"C:\NzbDrone\AppData".AsOsAgnostic());

            Subject.Check().ShouldBeWarning();
        }

        [Test]
        public void should_return_warning_when_app_data_is_same_as_startup_folder()
        {
            Mocker.GetMock<IAppFolderInfo>()
                  .Setup(s => s.StartUpFolder)
                  .Returns(@"C:\NzbDrone".AsOsAgnostic());

            Mocker.GetMock<IAppFolderInfo>()
                  .Setup(s => s.AppDataFolder)
                  .Returns(@"C:\NzbDrone".AsOsAgnostic());

            Subject.Check().ShouldBeWarning();
        }

        [Test]
        public void should_return_ok_when_no_conflict()
        {
            Mocker.GetMock<IAppFolderInfo>()
                  .Setup(s => s.StartUpFolder)
                  .Returns(@"C:\NzbDrone".AsOsAgnostic());

            Mocker.GetMock<IAppFolderInfo>()
                  .Setup(s => s.AppDataFolder)
                  .Returns(@"C:\ProgramData\NzbDrone".AsOsAgnostic());

            Subject.Check().ShouldBeOk();
        }
    }
}
