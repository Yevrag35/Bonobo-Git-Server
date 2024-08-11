using System.ComponentModel.DataAnnotations;

namespace Yev.Bonobo.Database.Entities
{
    public sealed class UserModel
    {
        [Key]
        public int Id { get; set; }
        public required Guid EntraIdObjectId { get; set; }
        public required string UserName { get; set; }
    }
}