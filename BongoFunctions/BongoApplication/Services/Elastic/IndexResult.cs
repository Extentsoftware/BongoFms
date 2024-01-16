namespace BongoApplication.Services.Elastic
{
    public class IndexResult
    {
        public bool IsValid { get; set; }

        public Exception Exception { get; set; } = default!;

        public string DebugInfo { get; set; } = default!;
    }

}
