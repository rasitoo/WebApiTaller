namespace WebApiTaller.Models.DTO.DTOUser;

public class DTOUserReadAll
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

public class DTOUserRead
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string dni { get; set; }
}