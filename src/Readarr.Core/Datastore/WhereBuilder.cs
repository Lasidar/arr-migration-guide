using Dapper;

namespace Readarr.Core.Datastore
{
    public abstract class WhereBuilder : ExpressionVisitor
    {
        public DynamicParameters Parameters { get; protected set; }
    }
}
