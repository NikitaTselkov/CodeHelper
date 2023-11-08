using CodeHelper.Data;
using Microsoft.AspNetCore.Mvc;

namespace CodeHelper.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly QuestionsRepository _questionsRepository;

        public QuestionsController(QuestionsRepository questionsRepository)
        {
            _questionsRepository = questionsRepository;
        }

        public ActionResult All()
        {
            var questions = _questionsRepository.GetAll("Tags").ToList();
            return View(questions);
        }
    }
}
