﻿namespace PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

public class League
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Country { get; set; }
    public string? Logo { get; set; }
    public string? Flag { get; set; }
    public int? Season { get; set; }
    public string? Round { get; set; }
}