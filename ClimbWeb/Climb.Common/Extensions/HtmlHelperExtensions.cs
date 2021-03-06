﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Climb.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string FilterKeys(this IHtmlHelper helper, params string[] keys) => string.Join("|", keys);
    }
}