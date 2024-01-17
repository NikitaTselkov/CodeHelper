using CodeHelper.Data;
using System.Xml.Linq;

namespace CodeHelper.Core
{
    public class SitemapGenerator
    {
        private readonly QuestionsRepository _questionsRepository;
        private readonly IConfiguration _configuration;

        public SitemapGenerator(QuestionsRepository questionsRepository, IConfiguration configuration)
        {
            _questionsRepository = questionsRepository;
            _configuration = configuration;
        }

        public IReadOnlyCollection<string> GetSitemapNodes()
        {
            List<string> nodes = new List<string>();

            foreach (var question in _questionsRepository.GetAll())
            {
                nodes.Add($"{_configuration["Domen"]}questions/{Extensions.TitleToUrl(question.Title)}/{question.Id}");
            }

            return nodes;
        }

        public string GetSitemapDocument(IEnumerable<string> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (string sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }
    }
}
