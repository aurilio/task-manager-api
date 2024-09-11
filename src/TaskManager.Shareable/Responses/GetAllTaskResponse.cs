﻿using TaskManager.Shareable.DTOs;

namespace TaskManager.Shareable.Responses;

public class GetAllTaskResponse
{
    public List<TaskEntityGetDTO> Tasks { get; set; } = new List<TaskEntityGetDTO>();
    
    public int TotalPages { get; set; }
    
    public int CurrentPage { get; set; }
}