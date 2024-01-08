using System.ComponentModel.DataAnnotations;

namespace AnketProje.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<Question> Questions { get; set; }

    }

    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int SurveyId { get; set; }
        public Survey Survey { get; set; }

    }

}
