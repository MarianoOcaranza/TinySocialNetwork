using Microsoft.Net.Http.Headers;

public class Post 
{
    public int Id {get; set;}
    public string Text {get; set;} = string.Empty;
    public string? ImageUrl {get; set;}
    public int LikesCount {get; set;} = 0;
    public int UserId{get; set;}
    public User? User {get; set;}
}