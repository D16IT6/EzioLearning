﻿namespace EzioLearning.Share.Models.Token;

public class JwtConfiguration
{
    public string? Issuer { get; set; }
    public required string PrivateKey { get; init; }
    public string? Audience { get; set; }

    public int ExpiredAfterMinutes { get; set; }

    public int ExpiredRefreshTokenAfterDays { get; set; }
}