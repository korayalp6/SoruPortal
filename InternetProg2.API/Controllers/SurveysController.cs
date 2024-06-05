using InternetProg2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternetProg2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurveysController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public DataResult<Survey> GetById(int id)
        {
            DataResult<Survey> dataResult = new DataResult<Survey>();

            Survey? survey = _context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            if (survey == null)
            {
                dataResult.ErrorMessage = "Anket Bulunamadı.";
                dataResult.IsSuccess = false;
                return dataResult;
            }
            else
            {
                dataResult.IsSuccess = true;
                dataResult.Data = survey;
                return dataResult;
            }
        }

        [HttpPost]
        public bool Add(Survey survey)
        {
            _context.Surveys.Add(survey);
            _context.SaveChanges();
            return true;
        }

        [HttpDelete]
        public DataResult<Survey> Delete(int id)
        {
            DataResult<Survey> dataResult = new DataResult<Survey>();

            Survey? survey = _context.Surveys.Where(x => x.Id == id).FirstOrDefault();
            if (survey == null)
            {
                dataResult.ErrorMessage = "Silinecek Anket Bulunamadı.";
                dataResult.IsSuccess = false;
                return dataResult;
            }
            else
            {
                _context.Surveys.Remove(survey);
                _context.SaveChanges();
                dataResult.IsSuccess = true;
                dataResult.Data = survey;
                return dataResult;
            }
        }

        [HttpPut]
        public DataResult<Survey> Update(Survey surveyDto)
        {
            DataResult<Survey> dataResult = new DataResult<Survey>();

            Survey? survey = _context.Surveys.Where(x => x.Id == surveyDto.Id).SingleOrDefault();
            if (survey == null)
            {
                dataResult.ErrorMessage = "Güncellenecek Anket Bulunamadı.";
                dataResult.IsSuccess = false;
                return dataResult;
            }
            else
            {
                survey.Title = surveyDto.Title;
                survey.Description = surveyDto.Description;
                _context.Surveys.Update(survey);
                _context.SaveChanges();
                dataResult.IsSuccess = true;
                dataResult.Data = survey;
                return dataResult;
            }
        }
    }
}
