﻿namespace PalpiteFC.Worker.Integrations.Providers.Responses.Entities;

public class Score
{
    public Halftime? Halftime { get; set; }
    public Fulltime? Fulltime { get; set; }
    public Extratime? Extratime { get; set; }
    public Penalty? Penalty { get; set; }
}

