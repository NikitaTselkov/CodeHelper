using System.ComponentModel.DataAnnotations;

namespace CodeHelper.Models
{
    public enum SortFilters
    {
        [Display(Name = "Новизне")]
        Newest,
        [Display(Name = "Популярности")]
        MostFrequent
    }
}
