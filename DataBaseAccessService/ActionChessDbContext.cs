using System.Data.Entity;

namespace DataBaseAccessService
{
    internal class CActionChessDbContext : DbContext
    {
        public CActionChessDbContext() : base("ActionChessDb")
        {
        }

        public DbSet<CAuthentication> Authentications { get; set; }

        public DbSet<CUser> Users { get; set; }

        public DbSet<CGame> Games { get; set; }

        public DbSet<CBoard> Boards { get; set; }

        public DbSet<CRecord> Records { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;

            modelBuilder.Entity<CAuthentication>()
                .HasRequired(authentication => authentication.User)
                .WithRequiredDependent(user => user.Authentication)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CGame>()
                .HasRequired(game => game.WhitePlayer)
                .WithMany(user => user.WhiteGames)
                .HasForeignKey(game => game.WhitePlayerId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CGame>()
                .HasRequired(game => game.BlackPlayer)
                .WithMany(user => user.BlackGames)
                .HasForeignKey(game => game.BlackPlayerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CBoard>()
                .HasRequired(board => board.Game)
                .WithMany(game => game.Boards)
                .HasForeignKey(board => board.GameId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CRecord>()
                .HasRequired(record => record.Board)
                .WithOptional(board => board.Record)
                .WillCascadeOnDelete(true);
        }
    }
}
