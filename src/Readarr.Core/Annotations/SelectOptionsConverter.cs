using System.Collections.Generic;

namespace Readarr.Core.Annotations
{
    public interface ISelectOptionsConverter
    {
        List<SelectOption> GetSelectOptions();
    }
}
