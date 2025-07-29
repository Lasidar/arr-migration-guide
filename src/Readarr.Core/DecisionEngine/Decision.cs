using System.Linq;

namespace Readarr.Core.DecisionEngine
{
    public class Decision
    {
        public bool Accepted { get; private set; }
        public Rejection[] Rejections { get; private set; }

        private Decision()
        {
            Rejections = new Rejection[0];
        }

        public static Decision Accept()
        {
            return new Decision
            {
                Accepted = true
            };
        }

        public static Decision Reject(string reason, RejectionType type = RejectionType.Permanent)
        {
            return new Decision
            {
                Accepted = false,
                Rejections = new[] { new Rejection(reason, type) }
            };
        }

        public static Decision Reject(Rejection rejection)
        {
            return new Decision
            {
                Accepted = false,
                Rejections = new[] { rejection }
            };
        }

        public static Decision Reject(params Rejection[] rejections)
        {
            return new Decision
            {
                Accepted = false,
                Rejections = rejections
            };
        }

        public static Decision Reject(string reason, params Rejection[] rejections)
        {
            var allRejections = rejections.ToList();
            allRejections.Insert(0, new Rejection(reason));

            return new Decision
            {
                Accepted = false,
                Rejections = allRejections.ToArray()
            };
        }
    }
}