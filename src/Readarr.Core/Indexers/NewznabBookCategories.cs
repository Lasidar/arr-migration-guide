namespace Readarr.Core.Indexers
{
    public static class NewznabBookCategories
    {
        // Main book categories (7000 range)
        public static readonly int Books = 7000;
        public static readonly int BooksMags = 7010;
        public static readonly int BooksEBook = 7020;
        public static readonly int BooksComics = 7030;
        public static readonly int BooksAudio = 7040;
        public static readonly int BooksTechnical = 7050;
        public static readonly int BooksOther = 7060;
        public static readonly int BooksForeign = 7070;

        // Audio book subcategories
        public static readonly int AudioAudiobook = 3030;

        // Standard book categories array
        public static readonly int[] StandardCategories = new[]
        {
            Books,
            BooksEBook,
            BooksAudio,
            AudioAudiobook
        };

        // All book categories
        public static readonly int[] AllCategories = new[]
        {
            Books,
            BooksMags,
            BooksEBook,
            BooksComics,
            BooksAudio,
            BooksTechnical,
            BooksOther,
            BooksForeign,
            AudioAudiobook
        };
    }
}