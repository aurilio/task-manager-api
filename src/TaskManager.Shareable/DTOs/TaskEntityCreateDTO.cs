﻿namespace TaskManager.Shareable.DTOs;

public class TaskEntityCreateDTO
{
    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}