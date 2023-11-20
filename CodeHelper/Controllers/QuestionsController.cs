using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models.Domain;
using CodeHelper.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeHelper.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly QuestionsRepository _questionsRepository;
        private readonly TagRepository _tagRepository;
        private readonly AnswerRepository _answerRepository;
        private readonly LikesRepository _likesRepository;
        private readonly UsersRepository _usersRepository;
        private readonly UserManager<User> _userManager;

        public QuestionsController(QuestionsRepository questionsRepository,
            TagRepository tagRepository,
            AnswerRepository answerRepository,
            UsersRepository usersRepository,
            LikesRepository likesRepository,
            UserManager<User> userManager)
        {
            _questionsRepository = questionsRepository;
            _tagRepository = tagRepository;
            _answerRepository = answerRepository;
            _usersRepository = usersRepository;
            _likesRepository = likesRepository;
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
            var model = new AskQuestionViewModel();

            model.AllTags = tags;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion(AskQuestionViewModel model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var tags = _tagRepository.GetAll().ToList();

            model.AllTags = tags;

            if (user == null || model.Question.Content == null) return View(model);

            model.Question.PublisedDate = DateTime.UtcNow;
            model.Question.Author = user;
            model.Question.Tags = new List<Tag>();

            if (model.SelectedTags != null)
                foreach (var tag in tags)
                {
                    if (model.SelectedTags.Any(a => a == tag.Name))
                        model.Question.Tags.Add(tag);
                }

            _questionsRepository.Add(model.Question);
            _questionsRepository.Save();

            var questionId = _questionsRepository.Get(g => g == model.Question).FirstOrDefault()?.Id;

            return RedirectToAction("Question", "Questions", new { questionId = questionId });
        }

        [HttpGet("{questionId}")]
        public IActionResult Question(int questionId)
        {
            var model = new QuestionViewModel();
            var user = _usersRepository.Get(g => g.UserName == HttpContext.User.Identity.Name, g => g.LikedAnswers).FirstOrDefault();

            model.Question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags, g => g.Answers).FirstOrDefault();

            foreach (var answer in model.Question.Answers)
            {
                answer.IsLikedAnswer = false;

                if (user != null)
                    foreach (var likedAnswers in user.LikedAnswers)
                    {
                        answer.IsLikedAnswer = likedAnswers.AnswerId == answer.Id;
                    }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PushAnswer(int questionId, string answerContent)
        {
            if (string.IsNullOrEmpty(answerContent))
            {
                ModelState.AddModelError("", "Answer is empty");
                return RedirectToAction("Question", "Questions", new { questionId = questionId });
            }

            var model = new QuestionViewModel();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var answer = new Answer()
            {
                Content = answerContent,
                PublisedDate = DateTime.UtcNow,
                IsLikedAnswer = false,
                IsAcceptedAnswer = false,
                LikesCount = 0,
                User = user
            };

            model.Question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags, g => g.Answers).FirstOrDefault();

            if (model.Question == null) return View(model);
            if (model.Question.Answers == null)
                model.Question.Answers = new List<Answer>();

            model.Question.Answers.Add(answer);
            model.Question.HasAnswers = true;

            _questionsRepository.Update(model.Question);

            return RedirectToAction("Question", "Questions", new { questionId = model.Question.Id });
        }

        [HttpPost]
        public async Task<ContentResult> SetLikeAnswer(int answerId, bool isLikedAnswer)
        {
            try
            {
                var user = _usersRepository.Get(g => g.UserName == HttpContext.User.Identity.Name, g => g.LikedAnswers).FirstOrDefault();
                var answer = _answerRepository.Get(g => g.Id == answerId).SingleOrDefault();

                if (user == null) return Content(answer.LikesCount.ToString());
                if (user.LikedAnswers == null)
                    user.LikedAnswers = new List<Like>();

                if (isLikedAnswer)
                {
                    answer.LikesCount += 1;
                    answer.IsLikedAnswer = true;
                    user.LikedAnswers.Add(new Like(answer.Id));
                }
                else
                {
                    answer.LikesCount -= answer.LikesCount > 0 ? 1 : 0;
                    answer.IsLikedAnswer = false;

                    var like = user.LikedAnswers.First(f => f.AnswerId == answer.Id);

                    user.LikedAnswers.Remove(like);

                    _likesRepository.Remove(like);
                    _likesRepository.Save();
                }

                _answerRepository.Update(answer);
                await _userManager.UpdateAsync(user);

                return Content(answer.LikesCount.ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
