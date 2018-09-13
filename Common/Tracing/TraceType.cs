
namespace Common.Tracing
{
    using NLog;

    public sealed class TraceType
    {
        public TraceType(string name, string traceId = null)
        {
            this.Name = name;

            if (string.IsNullOrWhiteSpace(traceId))
            {
                this.Logger = LogManager.GetLogger(string.Format("{0}", name));
            }
            else
            {
                this.Logger = LogManager.GetLogger(string.Format("{0}.{1}", name, traceId));
            }
        }

        public Logger Logger { get; private set; }

        public string Name { get; private set; }
    }
}
