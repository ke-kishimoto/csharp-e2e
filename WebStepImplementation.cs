using Gauge.CSharp.Lib;
using Gauge.CSharp.Lib.Attribute;
using Microsoft.Playwright;
using Shouldly;
using System;
using System.Threading.Tasks;

namespace DotNet.Template
{
    public class WebStepImplementation
    {
        private const string KeyPage = "pw:page";
        private const string KeyBaseUrl = "pw:base_url"; 

        private static IPage Page =>
            ScenarioDataStore.Get<IPage>(KeyPage) ?? throw new System.InvalidOperationException("Pageが初期化されていません");

        [Step("URL <url> を開く")]
        public async Task OpenUrl(string url)
        {
            await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        }

        [Step("URL <url> に遷移している")]
        public async Task UrlIs(string url)
        {
            var baseUrl = ScenarioDataStore.Get<string>(KeyBaseUrl);
            var actual = Page.Url;
            if (baseUrl != null && Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) && Uri.TryCreate(baseUri, url, out var expectedUri))
            {
                url = expectedUri.ToString();
            }
            actual.ShouldBe(url);
        }

        [Step("要素 <selector> に <text> が表示されている")]
        public async Task ElementTextIsVisible(string selector, string text)
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        }

        [Step("要素 <selector> が <count> 件表示されている")]
        public async Task ElementCountIsVisible(string selector, int count)
        {
            var elements = Page.Locator(selector);
            await elements.First.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            var actualCount = await elements.CountAsync();
            actualCount.ShouldBe(count);
        }

        [Step("リンク <text> をクリックする")]
        public async Task ClickLink(string text)
        {
            var link = Page.Locator($"a:has-text(\"{text}\")");
            await link.ClickAsync();
        }

        [Step("見出し <text> が表示されている")]
        public async Task HeadingIsVisible(string text)
        {
            var h1 = Page.Locator("h1");
            await h1.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var actual = await h1.InnerTextAsync();
            actual.ShouldContain(text);
        }
    }

}