namespace Tests_and_Interviews.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AdviceChoice
    {
        public string Advice { get; private set; }

        public string Feedback { get; private set; }

        public AdviceChoice(string advice, string feedback)
        {
            Advice = advice;
            Feedback = feedback;
        }

        public string IsChosen()
        {
            return Feedback;
        }
    }
}
