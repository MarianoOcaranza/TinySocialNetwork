public class Comment {
    public int Id {get; set;}
    public string Text {get; set;} = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Relationship with Post
    public int PostId {get; set;}
    public Post? Post {get; set;}
    //Relationship with User
    public int UserId{get; set;}
    public User? User{get; set;}

}