using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Annotations;

namespace Readarr.Core.Languages
{
    public class LanguageFieldConverter : ISelectOptionsConverter
    {
        public List<SelectOption> GetSelectOptions()
        {
            return Language.All
                .OrderBy(l => l.Id > 0).ThenBy(l => l.Name)
                .ToList()
                .ConvertAll(v => new SelectOption { Value = v.Id, Name = v.Name });
        }
    }
}
