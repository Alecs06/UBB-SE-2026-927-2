namespace Tests_and_Interviews_API.DTOs
{
    /// <summary>
    /// Returned when reading a question (e.g. inside a test response).
    /// </summary>
    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public float QuestionScore { get; set; }
        public string? QuestionAnswer { get; set; }
        public string? OptionsJson { get; set; }
    }
}
