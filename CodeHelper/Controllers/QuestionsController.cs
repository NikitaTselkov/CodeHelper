using CodeHelper.Data;
using CodeHelper.Models.Domain;
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

        public IActionResult All()
        {
            var questions = _questionsRepository.GetAll("Tags").ToList();

            questions.Add(new Question() { ViewsCount = 5345, Author = new User() {UserName = "Nikita" }, Content = "Content WWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWContent WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { ViewsCount = 1235, Tags = new List<Tag>() { new Tag() { Name = "", DisplayName = "google-chrome-extension" }, new Tag() { Name = "", DisplayName = "google-chrome-extension" }, new Tag() {Name = "", DisplayName = "google-chrome-extension" }, new Tag() { Name = "C_Sharp", DisplayName = "C#" }, new Tag() { Name = "asp_net_core", DisplayName = "ASP-Net Core" } }, Author = new User() { UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Metal is premultiplying texture alpha between fragment attachment & fragment shader arg— why?" });
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});
            questions.Add(new Question() { Author = new User() {UserName = "Nikita" }, Content = "Content", PublisedDate = DateTime.Now, Title = "Title"});

            return View(questions);
        }

        public IActionResult Question()
        {
            return View();
        }
    }
}
