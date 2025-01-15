public class Like {
    public int Id {get; set;}
    public DateTime LikedAt {get; set;} = DateTime.UtcNow;
    //Related post
    public int PostId {get; set;}
    public Post? Post {get; set;}
    //Related user
    public int UserId {get; set;}
    public User? User {get; set;}

}