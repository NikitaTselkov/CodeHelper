using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models.Domain;
using CodeHelper.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace CodeHelper.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly QuestionsRepository _questionsRepository;
        private readonly TagRepository _tagRepository;
        private readonly UserManager<User> _userManager;

        public QuestionsController(QuestionsRepository questionsRepository, TagRepository tagRepository, UserManager<User> userManager)
        {
            _questionsRepository = questionsRepository;
            _tagRepository = tagRepository;
            _userManager = userManager;
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
            var filterPredicate = PredicateBuilder.True<Question>();
            var sortPredicate = PredicateBuilder.True<Question>();

            #region Filters

            if (tags != null)
                filterPredicate = filterPredicate.And(e => e.Tags.Any(a => tags.Contains(a.Name)));

            if (model.NoAnswers)
                filterPredicate = filterPredicate.And(e => e.HasAnswers == false);

            if (model.NoAcceptedAnswer)
                filterPredicate = filterPredicate.And(e => e.HasAcceptedAnswer == false);

            #endregion

            #region Sorts

            var questions = _questionsRepository.Get(filterPredicate, g => g.Author, g => g.Tags);

            switch (model.Sort)
            {
                case Models.SortFilters.Newest:
                    questions = questions.OrderBy(o => o.PublisedDate);
                    break;
                case Models.SortFilters.MostFrequent:
                    questions = questions.OrderByDescending(o => o.ViewsCount);
                    break;
            }

            #endregion

            model.Questions = questions.ToList();
            model.AllTags = _tagRepository.GetAll().ToList();

            return View(model);
        }

        public IActionResult AskQuestion()
        {
            var tags = _tagRepository.GetAll().ToList();
            var model = new QuestionViewModel();

            model.AllTags = tags;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion(QuestionViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var tags = _tagRepository.GetAll().ToList();

            if (user == null) return View(model);

            model.Question.PublisedDate = DateTime.UtcNow;
            model.Question.Author = user;
            model.Question.Tags = new List<Tag>();

            foreach (var tag in tags)
            {
                if (model.SelectedTags.Any(a => a == tag.Name))
                    model.Question.Tags.Add(tag);
            }

            _questionsRepository.Add(model.Question);
            _questionsRepository.Save();

            return RedirectToAction("Question", "Questions");
        }

        public IActionResult Question()
        {
            return View();
        }
    }
}
