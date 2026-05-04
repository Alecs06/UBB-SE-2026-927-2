namespace Tests_and_Interviews_API.DTOs
{
    /// <summary>
    /// Sent by the client when creating a new test.
    /// </summary>
    public class CreateTestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<CreateQuestionDto> Questions { get; set; } = [];
    }
}
