using AnketProje.Models;
using AnketProje.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AnketProje.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz Kullanıcı Adı veya Parola!");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Kullanıcı Girişi " + user.LockoutEnd + " kadar kısıtlanmıştır!");
                return View();
            }
            ModelState.AddModelError("", "Geçersiz Kullanıcı Adı veya Parola Başarısız Giriş Sayısı :" + await _userManager.GetAccessFailedCountAsync(user) + "/3");
            return View();
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {


            if (!ModelState.IsValid)
            {
                return View(model);

            }
            var identityResult = await _userManager.CreateAsync(new() { UserName = model.UserName, Email = model.Email, City = model.City, FullName = model.FullName }, model.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View(model);
            }

            
            var user = await _userManager.FindByNameAsync(model.UserName);
            var roleExist = await _roleManager.RoleExistsAsync("Uye");
            if (!roleExist)
            {
                var role = new AppRole { Name = "Uye" };
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Uye");

            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserList()
        {
            var userModels = await _userManager.Users.Select(x => new UserModel()
            {

                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                UserName = x.UserName,
                City = x.City
            }).ToListAsync();
            return View(userModels);
        }
        public async Task<IActionResult> GetRoleList()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult RoleAdd()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleAdd(AppRole model)
        {
            var role = await _roleManager.FindByNameAsync(model.Name);
            if (role == null)
            {

                var newrole = new AppRole();
                newrole.Name = model.Name; ;
                await _roleManager.CreateAsync(newrole);
            }
            return RedirectToAction("GetRoleList");
        }







        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]


        public IActionResult AddSurvey()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddSurvey([FromBody] Survey survey)
        {
            if (survey == null)
            {
                return BadRequest("Anket bilgileri eksik.");
            }

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            var surveyUrl = Url.Action("GetSurveyAdmin", "Home");

            
            return Json(new { redirectUrl = surveyUrl });
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]

        public IActionResult GetSurveyAdmin()
        {
            var surveys = _context.Surveys.ToList();
            return View(surveys);
        }

        [HttpGet("Home/GetSurvey/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetSurvey(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound("Belirtilen ID'ye sahip anket bulunamadı.");
            }

            return View(survey);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);

            if (survey == null)
            {
                return NotFound("Anket bulunamadı.");
            }

            
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetSurveyAdmin", "Home");

        }



        [HttpGet]
        [Authorize(Roles = "Admin")]

        public IActionResult AddQuestion(int surveyId)
        {
            
            ViewBag.SurveyId = surveyId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddQuestion(int surveyId, Question question)
        {
            if (question == null)
            {
                return BadRequest("Soru bilgileri eksik.");
            }

            
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
            {
                return NotFound("Belirtilen ID'ye sahip anket bulunamadı.");
            }

            
            survey.Questions.Add(question);

            
            await _context.SaveChangesAsync();

            
            return RedirectToAction("GetSurvey", "Home", new { id = surveyId });
        }

        [HttpGet]
        public async Task<IActionResult> ListSurveys()
        {
            
            var successMessage = TempData["SuccessMessage"] as string;

            
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }
            var surveys = await _context.Surveys.ToListAsync();
            return View(surveys);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> AnswerSurvey(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound("Belirtilen ID'ye sahip anket bulunamadı.");
            }

            return View(survey);
        }

        [HttpPost]
        [AllowAnonymous]

        public IActionResult SubmitSurveyAnswers()
        {
       
            TempData["SuccessMessage"] = "Anketiniz başarıyla iletilmiştir.";

            return RedirectToAction("ListSurveys");
        }
    }

}
