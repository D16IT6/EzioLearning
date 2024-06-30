﻿namespace EzioLearning.Share.Models.Pages;

public class PageResult<T> : PageResultBase where T : class
{
    public IEnumerable<T> PageData { get; set; } = [];
}