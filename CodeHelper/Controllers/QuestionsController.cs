﻿using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models;
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

        [HttpGet]
        public IActionResult All(int page = 1)
        {
            var tags = _tagRepository.GetAll().ToList();
            var pagesCount = 0;
            var pageOffset = (int)((page - 1) * GlobalConstants.QuestionsCountIntPage);

            if (TempData["QuestionsViewModel"] is string value)
            {
                var model = value.FromJson<QuestionsViewModel>();
                var filterPredicate = PredicateBuilder.True<Question>();
                var sortPredicate = PredicateBuilder.True<Question>();

                #region Filters

                var selectedTags = model.SelectedTags;

                if (selectedTags != null)
                    filterPredicate = filterPredicate.And(e => e.Tags.Any(a => selectedTags.Contains(a.Name)));

                if (model.NoAnswers)
                    filterPredicate = filterPredicate.And(e => e.HasAnswers == false);

                if (model.NoAcceptedAnswer)
                    filterPredicate = filterPredicate.And(e => e.HasAcceptedAnswer == false);

                #endregion

                var questions = _questionsRepository.Get(filterPredicate, g => g.Author, g => g.Tags);

                #region Sorts

                switch (model.Sort)
                {
                    case SortFilters.Newest:
                        questions = questions.OrderBy(o => o.PublisedDate);
                        break;
                    case SortFilters.MostFrequent:
                        questions = questions.OrderByDescending(o => o.ViewsCount);
                        break;
                }

                #endregion

                pagesCount = (int)Math.Ceiling(questions.Count() / GlobalConstants.QuestionsCountIntPage);
                questions = questions.Skip(pageOffset).Take((int)GlobalConstants.QuestionsCountIntPage);

                TempData["QuestionsViewModel"] = model.ToJson();

                model.AllTags = tags;
                model.Questions = questions.ToList();
                model.Pagination = new Pagination(page, pagesCount);

                return View(model);
            }

            pagesCount = (int)Math.Ceiling(_questionsRepository.GetAll().Count() / GlobalConstants.QuestionsCountIntPage);

            var questionsViewModel = new QuestionsViewModel
            {
                Questions = _questionsRepository.GetAll(pageOffset, (int)GlobalConstants.QuestionsCountIntPage, g => g.Author, g => g.Tags).ToList(),
                AllTags = tags,
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
        public IActionResult EditAnswer(int answerId)
        {
            var answer = _answerRepository.Get(g => g.Id == answerId, g => g.User).FirstOrDefault();

            TempData["EditAnswer"] = answer.ToJson();

            return NoContent();
        }

        [HttpPost]
        public IActionResult EditQuestion(int questionId)
        {
            var question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags).FirstOrDefault();

            TempData["EditQuestion"] = question.ToJson();

            return RedirectToAction(nameof(AskQuestion));
        }

        [HttpGet]
        public IActionResult AskQuestion()
        {
            var model = new AskQuestionViewModel();

            if (TempData["EditQuestion"] is string value)
            {
                model.Question = value.FromJson<Question>();
                model.SelectedTags = model.Question.Tags.Select(s => s.Name).ToList();

                TempData["EditQuestionId"] = model.Question.Id.ToJson();
            }

            model.AllTags = _tagRepository.GetAll().ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion(AskQuestionViewModel model)
        {
            var tags = _tagRepository.GetAll().ToList();

            if (TempData["EditQuestionId"] is string value)
            {
                var editQuestionId = value.FromJson<int>();
                var question = _questionsRepository.Get(g => g.Id == editQuestionId, g => g.Tags, g => g.Answers, g => g.Author).FirstOrDefault();

                if (question != null)
                {
                    question.Title = model.Question.Title;
                    question.Content = model.Question.Content;
                    question.PublisedDate = DateTime.UtcNow;

                    if (model.SelectedTags != null)
                    {
                        question.Tags = new List<Tag>();

                        foreach (var tag in tags)
                        {
                            if (model.SelectedTags.Any(a => a == tag.Name))
                                question.Tags.Add(tag);
                        }
                    }

                    _questionsRepository.Update(question);

                    TempData.Remove("EditQuestionId");

                    return RedirectToAction("Question", "Questions", new { questionId = question.Id });
                }
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            model.AllTags = tags;

            if (user == null || model.Question.Content == null) return View(model);

            model.Question.PublisedDate = DateTime.UtcNow;
            model.Question.Tags = new List<Tag>();
            model.Question.Author = user;

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
        public IActionResult Question(int questionId, int page = 1)
        {
            var model = new QuestionViewModel();
            var userName = HttpContext.User.Identity?.Name;
            var user = _usersRepository.Get(g => g.UserName == userName, g => g.LikedAnswers).FirstOrDefault();
            var question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags).FirstOrDefault();

            if (question != null)
            {
                var pageOffset = (int)((page - 1) * GlobalConstants.AnswersCountIntPage);
                var answers = _answerRepository.Get(g => g.Question.Id == questionId, pageOffset, (int)GlobalConstants.AnswersCountIntPage, g => g.Question).ToList();
                var answersCount = _answerRepository.Get(g => g.Question.Id == questionId, g => g.Question).Count();
                var pagesCount = (int)Math.Ceiling(answersCount / GlobalConstants.AnswersCountIntPage);

                question.Answers = answers;
                model.Question = question;
                model.AnswersCount = answersCount;
                model.AnswersContent = new List<Item>();
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
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveAnswer(int answerId, List<Item> answersContent)
        {
            var answer = _answerRepository.Get(g => g.Id == answerId, g => g.User, g => g.Question).FirstOrDefault();

            if (answer != null && answersContent != null)
            {
                answer.Content = answersContent.FirstOrDefault(f => f.Id == answerId).Value;
                answer.PublisedDate = DateTime.UtcNow;
                _answerRepository.Update(answer);

                return RedirectToAction("Question", "Questions", new { questionId = answer.Question.Id });
            }

            return NoContent();
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
            var question = _questionsRepository.Get(g => g.Id == questionId, g => g.Author, g => g.Tags, g => g.Answers).FirstOrDefault();
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

            return RedirectToAction("Question", "Questions", new { questionId = model.Question.Id });
        }

        [HttpPost]
        public ContentResult SetAcceptedAnswer(int answerId, int questionId)
        {
            var answer = _answerRepository.Get(g => g.Id == answerId).FirstOrDefault();

            if (answer != null)
            {
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
        public async Task<ContentResult> SetLikeAnswer(int answerId, bool isLikedAnswer)
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
    }
}
