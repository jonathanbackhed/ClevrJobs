using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Extensions
{
    internal static class PageExtensions
    {
        public static async Task<string> GetOptionalTextAsync(this IPage page, string selector, string defaultValue = "N/A")
        {
            var locator = page.Locator(selector);

            return await locator.CountAsync() > 0 ? await locator.First.InnerTextAsync() : defaultValue;
        }

        public static async Task<string> GetRequiredTextAsync(this IPage page, string selector)
        {
            var locator = page.Locator(selector);
            if (await locator.CountAsync() == 0)
            {
                throw new InvalidOperationException($"Required element not found: {selector}");
            }

            return await locator.First.InnerTextAsync();
        }
    }
}
