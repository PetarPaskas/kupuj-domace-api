namespace KupujDomace.Database.Models
{
    public class Category
    {
        
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string IconLink { get; set; }
        public string ParentCategoryId { get; set; }

        public bool IsSubCategory => !String.IsNullOrWhiteSpace(CategoryName);
    }
}
