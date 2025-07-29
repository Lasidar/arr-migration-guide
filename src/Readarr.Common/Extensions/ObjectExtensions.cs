using Readarr.Common.Serializer;

namespace Readarr.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static T JsonClone<T>(this T source)
            where T : new()
        {
            var json = source.ToJson();
            return Json.Deserialize<T>(json);
        }
    }
}
