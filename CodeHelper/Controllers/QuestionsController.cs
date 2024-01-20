using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models;
using CodeHelper.Models.Domain;
using CodeHelper.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Web;

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
        private readonly ImageManager _imageManager;
        private readonly IConfiguration _configuration;

        public QuestionsController(QuestionsRepository questionsRepository,
            TagRepository tagRepository,
            AnswerRepository answerRepository,
            UsersRepository usersRepository,
            LikesRepository likesRepository,
            UserManager<User> userManager,
            ImageManager imageManager,
            IConfiguration configuration)
        {
            _questionsRepository = questionsRepository;
            _tagRepository = tagRepository;
            _answerRepository = answerRepository;
            _usersRepository = usersRepository;
            _likesRepository = likesRepository;
            _userManager = userManager;
            _imageManager = imageManager;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult All(int page = 1)
        {
            var questionsViewModel = new QuestionsViewModel();
            var pagesCount = 0;
            var pageOffset = (int)((page - 1) * GlobalConstants.QuestionsCountIntPage);

            ViewData["Description"] = "Rewrite Code is a community for developers where people share their knowledge and experience";

            if (TempData["QuestionsViewModel"] is string value)
            {
                var model = value.FromJson<QuestionsViewModel>();
                var filterPredicate = PredicateBuilder.True<Question>();

                #region Filters

                var selectedTags = model.SelectedTags;

                if (selectedTags != null)
                    filterPredicate = filterPredicate.And(e => e.Tags.Any(a => selectedTags.Contains(a.Id)));

                if (model.NoAnswers)
                    filterPredicate = filterPredicate.And(e => e.HasAnswers == false);

                if (model.NoAcceptedAnswer)
                    filterPredicate = filterPredicate.And(e => e.HasAcceptedAnswer == false);

                #endregion

                #region Search

                var searchQuery = string.Empty;

                if (TempData["SearchQuery"] is string sQuery)
                {
                    searchQuery = sQuery.FromJson<string>();
                    TempData["SearchQuery"] = sQuery;
                }

                #endregion

                var questions = _questionsRepository.SearchByText(searchQuery, filterPredicate, 0, 0, g => g.Author, g => g.Tags);
                pagesCount = (int)Math.Ceiling(_questionsRepository.SearchByText(searchQuery, filterPredicate, 0, 0).Count() / GlobalConstants.QuestionsCountIntPage);

                #region Sorts

                switch (model.Sort)
                {
                    case SortFilters.Newest:
                        questions = questions.OrderByDescending(o => o.PublisedDate);
                        break;
                    case SortFilters.MostFrequent:
                        questions = questions.OrderByDescending(o => o.ViewsCount);
                        break;
                }

                #endregion

                TempData["QuestionsViewModel"] = model.ToJson();

                model.Questions = questions.Skip(pageOffset).Take((int)GlobalConstants.QuestionsCountIntPage).ToArray();
                model.Pagination = new Pagination(page, pagesCount);

                var tags = new List<Tag>();

                if (model.SelectedTags != null)
                    foreach (var tag in model.SelectedTags)
                    {
                        var t = _tagRepository.Get(g => g.Id == tag).FirstOrDefault();

                        if (t != null)
                            tags.Add(t);
                    }

                model.Tags = tags;

                return View(model);
            }

            if (TempData["SearchQuery"] is string query)
            {
                var questions = _questionsRepository.SearchByText(query.FromJson<string>(), null, 0, 0, g => g.Author, g => g.Tags);

                pagesCount = (int)Math.Ceiling(_questionsRepository.SearchByText(query.FromJson<string>(), null, 0, 0).Count() / GlobalConstants.QuestionsCountIntPage);
                questions = questions.Skip(pageOffset).Take((int)GlobalConstants.QuestionsCountIntPage);

                var model = new QuestionsViewModel
                {
                    Questions = questions.ToArray(),
                    NoAcceptedAnswer = false,
                    NoAnswers = false,
                    Sort = SortFilters.Newest,
                    Pagination = new Pagination(page, pagesCount)
                };

                TempData["SearchQuery"] = query;

                return View(model);
            }

            pagesCount = (int)Math.Ceiling(_questionsRepository.GetAll().Count() / GlobalConstants.QuestionsCountIntPage);

            questionsViewModel = new QuestionsViewModel
            {
                Questions = _questionsRepository.GetAll(pageOffset, (int)GlobalConstants.QuestionsCountIntPage, g => g.Author, g => g.Tags).ToArray(),
                NoAcceptedAnswer = false,
                NoAnswers = false,
                Sort = SortFilters.Newest,
                Pagination = new Pagination(page, pagesCount)
            };

            return View(questionsViewModel);
        }

        [HttpPost]
        public IActionResult All(QuestionsViewModel model)
        {
            TempData["QuestionsViewModel"] = model.ToJson();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public IActionResult Search(string query)
        {
            TempData["SearchQuery"] = query.ToJson();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public IActionResult Tags([FromQuery] string query)
        {
            var tags = _tagRepository.SearchByText(query, null, 0, 5).ToList();

            var result = tags.Select(s => new { value = s.Id, label = s.Name });

            return Json(result);
        }

        [HttpPost]
        public IActionResult UploadImage(List<IFormFile> files)
        {
            var filePath = string.Empty;

            foreach (var image in files)
            {
                filePath = _imageManager.SaveImage(image);
            }

            return Json(new { url = filePath });
        }

        [HttpPost]
        public IActionResult EditAnswer(int answerId)
        {
            return NoContent();
        }

        [HttpPost]
        public IActionResult EditQuestion(int questionId)
        {
            TempData["EditQuestionId"] = questionId.ToJson();

            return RedirectToAction(nameof(AskQuestion));
        }

        [HttpGet]
        public IActionResult AskQuestion()
        {
            ViewData["CurrentPage"] = "Autorization";

            var model = new AskQuestionViewModel();

            if (TempData["EditQuestionId"] is string value)
            {
                var editQuestionId = value.FromJson<int>();
                var question = _questionsRepository.Get(g => g.Id == editQuestionId, g => g.Tags).FirstOrDefault();

                if (question != null)
                {
                    model.Question = question;
                    model.SelectedTags = question.Tags.Select(s => s.Id).ToList();

                    TempData["EditQuestionId"] = model.Question.Id.ToJson();
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult AskQuestion(AskQuestionViewModel model)
        {
            if (TempData["EditQuestionId"] is string value)
            {
                var editQuestionId = value.FromJson<int>();
                var question = _questionsRepository.Get(g => g.Id == editQuestionId, g => g.Tags, g => g.Answers, g => g.Author).FirstOrDefault();

                if (question != null)
                {
                    if (question.Author.Id != _userManager.GetUserId(HttpContext.User))
                        return RedirectToAction("Question", "Questions", new { title = Extensions.TitleToUrl(question.Title), questionId = question.Id });

                    _imageManager.RemoveImages(question.Content, model.Question.Content);

                    question.Title = model.Question.Title;
                    question.Content = model.Question.Content;
                    question.PublisedDate = DateTime.UtcNow;

                    if (model.SelectedTags == null)
                        question.Tags = new List<Tag>();
                    else
                        question.Tags = _tagRepository.Get(t => model.SelectedTags.Any(a => a == t.Id)).ToList();

                    _questionsRepository.Update(question);

                    TempData.Remove("EditQuestionId");

                    return RedirectToAction("Question", "Questions", new { title = Extensions.TitleToUrl(question.Title), questionId = question.Id });
                }
            }

            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            if (user == null || model.Question.Content == null) return View(model);

            model.Question.PublisedDate = DateTime.UtcNow;
            model.Question.Tags = new List<Tag>();
            model.Question.Author = user;

            if (model.SelectedTags != null && model.SelectedTags.Count > 0)
                model.Question.Tags = _tagRepository.Get(t => model.SelectedTags.Any(a => a == t.Id)).ToList();

            _questionsRepository.Add(model.Question);
            _questionsRepository.Save();

            var quest = _questionsRepository.Get(g => g == model.Question).FirstOrDefault();

            return RedirectToAction("Question", "Questions", new { title = Extensions.TitleToUrl(quest.Title), questionId = quest.Id });
        }

        [HttpGet("questions/{title}/{questionId}")]
        public IActionResult Question(string title, int questionId, int page = 1)
        {
            var model = new QuestionViewModel();
            var userName = HttpContext.User.Identity?.Name;
            var user = _usersRepository.Get(g => g.UserName == userName, g => g.LikedAnswers).FirstOrDefault();
            var question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags).FirstOrDefault();

            if (question != null)
            {
                var description = question.Content.Length >= 120 ? string.Join("", question.Content.Take(120)) : question.Content;
                description = Regex.Replace(description, @"<[^>]*>", "");
                description += " ...";

                ViewData["Description"] = HttpUtility.HtmlDecode(description);

                var pageOffset = (int)((page - 1) * GlobalConstants.AnswersCountIntPage);
                var answers = _answerRepository.Get(g => g.Question.Id == questionId, 0, 0, g => g.Question, g => g.User)
                    .OrderByDescending(o => o.LikesCount)
                    .ThenByDescending(o => o.IsAcceptedAnswer)
                    .Skip(pageOffset)
                    .Take((int)GlobalConstants.AnswersCountIntPage)
                    .ToList();

                var answersCount = _answerRepository.Get(g => g.Question.Id == questionId, g => g.Question).Count();
                var pagesCount = (int)Math.Ceiling(answersCount / GlobalConstants.AnswersCountIntPage);

                question.Answers = answers;
                model.Question = question;
                model.AnswersCount = answersCount;
                model.AnswersContent = new List<Item>();
                model.Links = new List<Link>();
                model.Pagination = new Pagination(page, pagesCount);

                foreach (var answer in question.Answers)
                {
                    model.AnswersContent.Add(new Item { Id = answer.Id, Value = answer.Content });
                }

                if (user != model.Question.Author)
                {
                    model.Question.ViewsCount += 1;
                    _questionsRepository.Update(model.Question);
                    _questionsRepository.Save();
                }

                if (user != null)
                {
                    foreach (var answer in model.Question.Answers)
                    {
                        answer.IsLikedAnswer = false;
                        answer.IsLikedAnswer = user.LikedAnswers.Any(a => a.AnswerId == answer.Id);
                    }
                }

                if (question.Tags.Count > 0)
                {
                    var links = _questionsRepository.Get(g => g.Tags.Contains(question.Tags.First()) && g.Title != question.Title)
                        ?.Take(5)
                        ?.Select(s =>
                            new Link()
                            {
                                Title = s.Title,
                                URl = $"{_configuration["Domen"]}questions/{Extensions.TitleToUrl(s.Title)}/{s.Id}",
                                ViewsCount = s.ViewsCount
                            })?.ToList();

                    model.Links = links;
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveAnswer(int answerId, List<Item> answersContent)
        {
            var answer = _answerRepository.Get(g => g.Id == answerId, g => g.User, g => g.Question).FirstOrDefault();

            if (answer != null)
            {
                var answerContent = answersContent.FirstOrDefault(f => f.Id == answerId)?.Value;

                if (answerContent != null)
                    _imageManager.RemoveImages(answer.Content, answerContent);

                answer.Content = answerContent;
                answer.PublisedDate = DateTime.UtcNow;
                _answerRepository.Update(answer);

                return RedirectToAction("Question", "Questions", new { title = Extensions.TitleToUrl(answer.Question.Title), questionId = answer.Question.Id });
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PushAnswer(int questionId, string answerContent)
        {
            var model = new QuestionViewModel();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags, g => g.Answers).FirstOrDefault();

            if (string.IsNullOrEmpty(answerContent))
            {
                ModelState.AddModelError("", "Answer is empty");
                return RedirectToAction("Question", "Questions", new { title = Extensions.TitleToUrl(question.Title), questionId = questionId });
            }

            var answer = new Answer()
            {
                Content = answerContent,
                PublisedDate = DateTime.UtcNow,
                IsLikedAnswer = false,
                IsAcceptedAnswer = false,
                LikesCount = 0,
                User = user
            };

            if (question == null) return View(model);
            if (question.Answers == null)
                question.Answers = new List<Answer>();

            model.Question = question;
            model.Question.Answers.Add(answer);
            model.Question.HasAnswers = true;

            _questionsRepository.Update(model.Question);

            return RedirectToAction("Question", "Questions", new { title = Extensions.TitleToUrl(model.Question.Title), questionId = model.Question.Id });
        }

        [HttpPost]
        public ContentResult SetAcceptedAnswer(int answerId, int questionId)
        {
            var answer = _answerRepository.Get(g => g.Id == answerId, g => g.User).FirstOrDefault();

            if (answer != null)
            {
                if (answer.User.Id != _userManager.GetUserId(HttpContext.User)) return Content("");

                answer.IsAcceptedAnswer = !answer.IsAcceptedAnswer;
                _answerRepository.Update(answer);

                var question = _questionsRepository.Get(g => g.Id == questionId, g => g.Answers).FirstOrDefault();

                if (question != null)
                {
                    question.HasAcceptedAnswer = question.Answers.Any(a => a.IsAcceptedAnswer);
                    _questionsRepository.Update(question);
                }
            }

            return Content("");
        }

        [HttpPost]
        public ContentResult SetLikeAnswer(int answerId, bool isLikedAnswer)
        {
            var userName = HttpContext.User.Identity?.Name;
            var user = _usersRepository.Get(g => g.UserName == userName, g => g.LikedAnswers).FirstOrDefault();
            var answer = _answerRepository.Get(g => g.Id == answerId).SingleOrDefault();

            if (answer == null) return Content(string.Empty);
            if (user == null) return Content(answer.LikesCount.ToString());
            if (user.LikedAnswers == null)
                user.LikedAnswers = new List<Like>();

            if (isLikedAnswer)
            {
                answer.LikesCount += 1;
                answer.IsLikedAnswer = true;
                user.LikedAnswers.Add(new Like(answer.Id));

                _usersRepository.Update(user);
            }
            else
            {
                answer.LikesCount -= answer.LikesCount > 0 ? 1 : 0;
                answer.IsLikedAnswer = false;

                var like = user.LikedAnswers.First(f => f.AnswerId == answer.Id);

                user.LikedAnswers.Remove(like);

                _usersRepository.Update(user);

                _likesRepository.Remove(like);
                _likesRepository.Save();
            }

            _answerRepository.Update(answer);

            return Content(answer.LikesCount.ToString());
        }
    }
}
