﻿namespace NSDGenerator.Client.Models;

public interface IBlockModel
{
    public IBlockModel Parent { get; set; }
    public Guid Id { get; set; }
}