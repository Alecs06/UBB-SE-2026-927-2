using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests_and_Interviews.Dtos
{
    /// <summary>
    /// DTO representing an answer to a question in a test attempt. It contains the ID of the question being answered and the value of
    /// the answer provided by the user. This DTO is used to transfer answer data between the client and server when submitting test
    /// attempts or retrieving test results.
    /// </summary>
    public class AnswerDto
    {
        public int QuestionId { get; set; }

        public string Value { get; set; } = string.Empty;
    }
}
