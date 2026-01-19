namespace Notes_Backend.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string username { get; set; } = null!;
        public string email { get; set; } = null!;
        public string password { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}