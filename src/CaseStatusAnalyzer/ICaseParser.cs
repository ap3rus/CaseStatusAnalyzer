﻿namespace CaseStatusAnalyzer
{
    public interface ICaseParser
    {
        bool TryParseOne(string inputHtml, out Case result);
    }
}