using System;

namespace SimpleFeedReader.ViewModels
{
    // <SnippetFinishedViewModel>
#nullable enable
    public class NewsStoryViewModel
    {
        public NewsStoryViewModel(DateTimeOffset published, string title, string uri) =>
            (Published, Title, Uri) = (published, title, uri);

        public DateTimeOffset Published { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
    }
#nullable restore
    // </SnippetFinishedViewModel>
}
