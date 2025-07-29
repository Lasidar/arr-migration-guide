using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Datastore.Events;
using Readarr.Core.HealthCheck;
using Readarr.Core.Messaging.Events;
using Readarr.SignalR;
using Readarr.Http;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Health
{
    [V3ApiController]
    public class HealthController : RestControllerWithSignalR<HealthResource, HealthCheck>,
                                IHandle<HealthCheckCompleteEvent>
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthController(IBroadcastSignalRMessage signalRBroadcaster, IHealthCheckService healthCheckService)
            : base(signalRBroadcaster)
        {
            _healthCheckService = healthCheckService;
        }

        [NonAction]
        public override ActionResult<HealthResource> GetResourceByIdWithErrorHandler(int id)
        {
            return base.GetResourceByIdWithErrorHandler(id);
        }

        protected override HealthResource GetResourceById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Produces("application/json")]
        public List<HealthResource> GetHealth()
        {
            return _healthCheckService.Results().ToResource();
        }

        [NonAction]
        public void Handle(HealthCheckCompleteEvent message)
        {
            BroadcastResourceChange(ModelAction.Sync);
        }
    }
}
