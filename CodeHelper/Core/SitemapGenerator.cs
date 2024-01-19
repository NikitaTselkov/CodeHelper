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

        public IReadOnlyCollection<string> GetSitemapNodes(int offset, int length)
        {
            List<string> nodes = new List<string>();

            if (offset == 0)
            {
                nodes.Add($"{_configuration["Domen"]}Questions/All");
            }

            foreach (var question in _questionsRepository.GetAll(offset, length))
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

        public string GetSitemapIndexDocument(int offset)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "sitemapindex");

            var questionsCount = _questionsRepository.GetAll().Count();
            int i = 0;

            do
            {
                XElement urlElement = new XElement(
                  xmlns + "sitemap",
                  new XElement(xmlns + "loc", Uri.EscapeUriString($"{_configuration["Domen"]}{i}/Sitemap.xml")));
                root.Add(urlElement);

                i += offset;
            } while (i < questionsCount);

            XDocument document = new XDocument(root);
            return document.ToString();
        }
    }
}
