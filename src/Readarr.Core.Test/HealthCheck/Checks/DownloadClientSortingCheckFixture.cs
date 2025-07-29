using System;
using Moq;
using NUnit.Framework;
using Readarr.Core.Download;
using Readarr.Core.Download.Clients;
using Readarr.Core.HealthCheck.Checks;
using Readarr.Core.Localization;
using Readarr.Core.Test.Framework;
using Readarr.Test.Common;

namespace Readarr.Core.Test.HealthCheck.Checks
{
    [TestFixture]
    public class DownloadClientFolderCheckFixture : CoreTest<DownloadClientSortingCheck>
    {
        private DownloadClientInfo _clientStatus;
        private Mock<IDownloadClient> _downloadClient;

        private static Exception[] DownloadClientExceptions =
        {
            new DownloadClientUnavailableException("error"),
            new DownloadClientAuthenticationException("error"),
            new DownloadClientException("error")
        };

        [SetUp]
        public void Setup()
        {
            _clientStatus = new DownloadClientInfo
            {
                IsLocalhost = true,
                SortingMode = null
            };

            _downloadClient = Mocker.GetMock<IDownloadClient>();
            _downloadClient.Setup(s => s.Definition)
                .Returns(new DownloadClientDefinition { Name = "Test" });

            _downloadClient.Setup(s => s.GetStatus())
                .Returns(_clientStatus);

            Mocker.GetMock<IProvideDownloadClient>()
                .Setup(s => s.GetDownloadClients(It.IsAny<bool>()))
                .Returns(new IDownloadClient[] { _downloadClient.Object });

            Mocker.GetMock<ILocalizationService>()
                .Setup(s => s.GetLocalizedString(It.IsAny<string>()))
                .Returns("Some Warning Message");
        }

        [Test]
        public void should_return_ok_if_sorting_is_not_enabled()
        {
            Subject.Check().ShouldBeOk();
        }

        [Test]
        public void should_return_warning_if_sorting_is_enabled()
        {
            _clientStatus.SortingMode = "TV";

            Subject.Check().ShouldBeWarning();
        }

        [Test]
        [TestCaseSource("DownloadClientExceptions")]
        public void should_return_ok_if_client_throws_downloadclientexception(Exception ex)
        {
            _downloadClient.Setup(s => s.GetStatus())
                .Throws(ex);

            Subject.Check().ShouldBeOk();

            ExceptionVerification.ExpectedErrors(0);
        }
    }
}
