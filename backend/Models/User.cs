public class User 
{
    public int Id {get; set;}
    public string Username {get; set;} = string.Empty;
    public string? ImageURL{get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty;
    public string PasswordHash{get; set;} = string.Empty;
    public bool IsEmailConfirmed {get; set;} = false; //TO DO! implement an email confirmations system 
}