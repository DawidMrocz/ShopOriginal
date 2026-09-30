namespace UserApp.ApiModels.Favorite
{
    public class FavoriteResponse
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<FavoriteResponseItem> Items { get; set; } = [];
    }
}
