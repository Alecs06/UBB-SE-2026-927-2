namespace Tests_and_Interviews_API.DTOs
{
    /// <summary>
    /// Sent by the client when creating a new question inside a test.
    /// </summary>
    public class CreateQuestionDto
    {
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public float QuestionScore { get; set; }
        public string? QuestionAnswer { get; set; }
        public string? OptionsJson { get; set; }
    }
}
