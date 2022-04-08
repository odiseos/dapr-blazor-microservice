using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Game_API.Context
{
    [Table("PlayedGame")]
    public class PlayedGame
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Points { get; set; }
        public DateTime DatePlayed { get; set; }
    }
}
