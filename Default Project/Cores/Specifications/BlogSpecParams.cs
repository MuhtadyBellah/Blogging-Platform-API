namespace Default_Project.Cores.Specifications
{
    public class BlogSpecParams
    {
        public SortOptions? Sort { get; set; }

        private string? search { get; set; }
        public string? term
        {
            get => search;
            set => search = value?.ToLower();
        }
    }

}
