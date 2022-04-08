using System.ComponentModel.DataAnnotations;

namespace User_API.Context
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
