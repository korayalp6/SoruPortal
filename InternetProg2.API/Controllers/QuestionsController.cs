using InternetProg2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetProg2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public DataResult<Question> GetById(int id)
        {
            DataResult<Question> dataResult = new DataResult<Question>();

            Question? question = _context.Questions.Where(x => x.Id == id).FirstOrDefault();
            if (question == null)
            {
                dataResult.ErrorMessage = "Soru Bulunamadı.";
                dataResult.IsSuccess = false;
                return dataResult;
            }
            else
            {
                dataResult.IsSuccess = true;
                dataResult.Data = question;
                return dataResult;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public bool Add(Question question)
        {
            _context.Questions.Add(question);
            _context.SaveChanges();
            return true;
        }

        [HttpDelete("{id}")]
        public DataResult<Question> Delete(int id)
        {
            DataResult<Question> dataResult = new DataResult<Question>();

            Question? question = _context.Questions.Where(x => x.Id == id).FirstOrDefault();
            if (question == null)
            {
                dataResult.ErrorMessage = "Silinecek Soru Bulunamadı.";
                dataResult.IsSuccess = false;
                return dataResult;
            }
            else
            {
                _context.Questions.Remove(question);
                _context.SaveChanges();
                dataResult.IsSuccess = true;
                dataResult.Data = question;
                return dataResult;
            }
        }

        [HttpPut]
        public DataResult<Question> Update(Question questionDto)
        {
            DataResult<Question> dataResult = new DataResult<Question>();

            Question? question = _context.Questions.Where(x => x.Id == questionDto.Id).SingleOrDefault();
            if (question == null)
            {
                dataResult.ErrorMessage = "Güncellenecek Soru Bulunamadı.";
                dataResult.IsSuccess = false;
                return dataResult;
            }
            else
            {
                question.Content = questionDto.Content;
                question.SurveyId = questionDto.SurveyId;
                _context.Questions.Update(question);
                _context.SaveChanges();
                dataResult.IsSuccess = true;
                dataResult.Data = question;
                return dataResult;
            }
        }
    }
}
