using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models.Domain;
using CodeHelper.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace CodeHelper.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly QuestionsRepository _questionsRepository;
        private readonly TagRepository _tagRepository;

        public QuestionsController(QuestionsRepository questionsRepository, TagRepository tagRepository)
        {
            _questionsRepository = questionsRepository;
            _tagRepository = tagRepository;
        }

        public IActionResult All()
        {
            var questions = _questionsRepository.GetAll(g => g.Author, g => g.Tags).ToList();
            var tags = _tagRepository.GetAll().ToList();

            var questionsViewModel = new QuestionsViewModel
            {
                Questions = questions,
                AllTags = _tagRepository.GetAll().ToList(),
                NoAcceptedAnswer = false,
                NoAnswers = false
            };

            return View(questionsViewModel);
        }

        [HttpPost]
        public IActionResult All(QuestionsViewModel model)
        {
            var tags = model.SelectedTags;
            Expression<Func<Question, bool>>? expression = null;
            Func<Question, bool>? resultFunc = null;

            if (tags != null)
                resultFunc = resultFunc.AndAlso(e => e.Tags.Any(a => tags.Contains(a.Name)));

            if (model.NoAnswers)
                resultFunc = resultFunc.AndAlso(e => e.HasAnswers.Equals(model.NoAnswers));

            if (model.NoAcceptedAnswer)
                resultFunc = resultFunc.AndAlso(e => e.HasAcceptedAnswer.Equals(model.NoAcceptedAnswer));

            expression = PredicateExtemtions.FuncToExpression(resultFunc);

            model.Questions = _questionsRepository.Get(expression, g => g.Author, g => g.Tags).ToList();
            model.AllTags = _tagRepository.GetAll().ToList();

            return View(model);
        }

        public IActionResult AskQuestion()
        {
            var model = new QuestionViewModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult AskQuestion(QuestionViewModel model)
        {
            return View(model);
        }

        public IActionResult Question()
        {
            return View();
        }
    }
}
