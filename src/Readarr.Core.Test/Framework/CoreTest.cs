using System;
using System.IO;
using NUnit.Framework;
using Readarr.Common.Cache;
using Readarr.Common.Cloud;
using Readarr.Common.EnvironmentInfo;
using Readarr.Common.Http;
using Readarr.Common.Http.Dispatchers;
using Readarr.Common.TPL;
using Readarr.Core.Configuration;
using Readarr.Core.Http;
using Readarr.Core.MetadataSource;
using Readarr.Core.Organizer;
using Readarr.Test.Common;

namespace Readarr.Core.Test.Framework
{
    public abstract class CoreTest : TestBase
    {
        protected void UseRealHttp()
        {
            Mocker.SetConstant<IHttpProxySettingsProvider>(new HttpProxySettingsProvider(Mocker.Resolve<IConfigService>()));
            Mocker.SetConstant<ICreateManagedWebProxy>(new ManagedWebProxyFactory(Mocker.Resolve<ICacheManager>()));
            Mocker.SetConstant<IHttpDispatcher>(new ManagedHttpDispatcher(Mocker.Resolve<IHttpProxySettingsProvider>(), Mocker.Resolve<ICreateManagedWebProxy>(), Mocker.Resolve<UserAgentBuilder>(), Mocker.Resolve<ICacheManager>(), TestLogger));
            Mocker.SetConstant<IHttpClient>(new HttpClient(new IHttpRequestInterceptor[0], Mocker.Resolve<ICacheManager>(), Mocker.Resolve<IRateLimitService>(), Mocker.Resolve<IHttpDispatcher>(), TestLogger));
            Mocker.SetConstant<IReadarrCloudRequestBuilder>(new ReadarrCloudRequestBuilder());
        }
    }

    public abstract class CoreTest<TSubject> : CoreTest
        where TSubject : class
    {
        private TSubject _subject;

        [SetUp]
        public void CoreTestSetup()
        {
            _subject = null;
        }

        protected TSubject Subject
        {
            get
            {
                if (_subject == null)
                {
                    _subject = Mocker.Resolve<TSubject>();
                }

                return _subject;
            }
        }
    }
}
