using Microsoft.EntityFrameworkCore;

public class SocialNetworkContext : DbContext {
    public SocialNetworkContext(DbContextOptions<SocialNetworkContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts {get; set;}
    public DbSet<Comment> Comments {get; set;}
    public DbSet<Like> Likes {get; set;}
}