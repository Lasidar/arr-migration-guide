using System;
using Readarr.Common.Exceptions;

namespace Readarr.Core.Datastore
{
    public class ModelNotFoundException : ReadarrException
    {
        public ModelNotFoundException(Type modelType, int modelId)
            : base("{0} with ID {1} does not exist", modelType.Name, modelId)
        {
        }
    }
}
