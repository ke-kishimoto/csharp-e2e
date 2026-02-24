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

        private static IPage Page =>
            ScenarioDataStore.Get<IPage>(KeyPage) ?? throw new System.InvalidOperationException("Pageが初期化されていません");

        [Step("URL <url> を開く")]
        public async Task OpenUrl(string url)
        {
            await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        }

        [Step("要素 <selector> に <text> が表示されている")]
        public async Task ElementTextIsVisible(string selector, string text)
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

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