using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Yev.Bonobo.Database.Entities
{
    public sealed class UserModel : IDbEntity<UserModel>
    {
        [Key]
        public int Id { get; set; }
        public required Guid EntraIdObjectId { get; set; }
        public required string UserName { get; set; }

        static void IDbEntity<UserModel>.AddToModel(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<UserModel>().ToTable("Users");

            user.HasKey(x => x.Id);

            user.HasData(new UserModel
            {
                UserName = "admin",
                EntraIdObjectId = Guid.Parse("da049019-bbfc-480f-9ab6-a16c4829db20"),
                Id = 1,
            });
        }
    }
}