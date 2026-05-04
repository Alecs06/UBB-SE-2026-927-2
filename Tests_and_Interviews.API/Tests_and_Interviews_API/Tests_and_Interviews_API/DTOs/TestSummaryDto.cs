namespace Tests_and_Interviews_API.DTOs
{
    /// <summary>
    /// Returned when listing all tests (no questions included).
    /// </summary>
    public class TestSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int QuestionCount { get; set; }
    }
}
