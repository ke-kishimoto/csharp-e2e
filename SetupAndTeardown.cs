using Gauge.CSharp.Lib.Attribute;
using Gauge.CSharp.Lib;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace DotNet.Template
{
    public class SetupAndTeardown 
    {
        private const string KeyPage = "pw:page";
        private const string KeyBrowser = "pw:browser";
        private const string KeyPlaywright = "pw:playwright";
        private const string KeyContext = "pw:context";

        [BeforeScenario]
        public async Task Setup()
        {
           var pw = await Playwright.CreateAsync();
            var browser = await pw.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var ctx = await browser.NewContextAsync();
            var page = await ctx.NewPageAsync();

            ScenarioDataStore.Add(KeyPlaywright, pw);
            ScenarioDataStore.Add(KeyBrowser, browser);
            ScenarioDataStore.Add(KeyContext, ctx);
            ScenarioDataStore.Add(KeyPage, page);
        }

        [AfterScenario]
        public async Task Teardown()
        {
            var page = ScenarioDataStore.Get<IPage>(KeyPage);
            var ctx = ScenarioDataStore.Get<IBrowserContext>(KeyContext);
            var browser = ScenarioDataStore.Get<IBrowser>(KeyBrowser);
            var pw = ScenarioDataStore.Get<IPlaywright>(KeyPlaywright);

            if (page != null) await page.CloseAsync();
            if (ctx != null) await ctx.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            pw?.Dispose();
        }

    }
}