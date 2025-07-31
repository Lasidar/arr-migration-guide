using System.Collections.Generic;
using FluentValidation.Results;

namespace Readarr.Core.ImportLists.Custom
{
    public interface ICustomBookImportProxy
    {
        List<CustomBookAPIResource> GetBooks(CustomBookSettings settings);
        ValidationFailure Test(CustomBookSettings settings);
    }
}