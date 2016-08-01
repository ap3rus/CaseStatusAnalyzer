namespace CaseStatusAnalyzer
{
    public interface ICrawlingStateProvider
    {
        CrawlingState Get();
        void Set(CrawlingState state);
    }
}