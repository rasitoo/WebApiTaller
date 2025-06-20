﻿namespace WebApiTaller.Models.DTO.DTOWorkshop;

public class DTOWorkshopReadAll
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Location { get; set; }
    public string Speciality { get; set; }
    public string Name { get; set; }
}

public class DTOWorkshopRead
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Nif { get; set; }
    public string Location { get; set; }
    public string Speciality { get; set; }
    public string Name { get; set; }
}