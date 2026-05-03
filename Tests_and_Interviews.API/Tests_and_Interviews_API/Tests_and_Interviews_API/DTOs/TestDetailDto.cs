namespace Tests_and_Interviews_API.DTOs
{
    /// <summary>
    /// Returned when fetching a single test (includes full question list).
    /// </summary>
    public class TestDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<QuestionDto> Questions { get; set; } = [];
    }
}
