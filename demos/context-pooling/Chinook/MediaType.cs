﻿namespace context_pooling.Chinook;

public partial class MediaType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Track> Tracks { get; } = new List<Track>();
}
