namespace Tests_and_Interviews_API.DTOs
{
    /// <summary>
    /// Sent by the client when updating an existing test.
    /// Only title and category can be changed here; questions are managed separately.
    /// </summary>
    public class UpdateTestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
