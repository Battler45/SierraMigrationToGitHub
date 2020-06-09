using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SierraMigrationToGitHub
{
    class Selenium
    {
        static public Selenium seleniumClient = null;
        static public Selenium GetSeleniumClient => seleniumClient ??= new Selenium();
        private ChromeDriver webDriver = null;
        private Selenium()
        {
            var chromePath = @$"{Directory.GetCurrentDirectory()}/chromedriver_win32";
            var chromeDriverOptions = new ChromeOptions();
            //chromeDriverOptions.AddArgument("headless");
            webDriver = new ChromeDriver(chromePath, chromeDriverOptions);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        public Selenium SignIn(string login, string password)
        {
            webDriver.Url = "https://github.com/login";
            var loginInput = webDriver.FindElementByXPath("//input[@name='login']");
            var passwordInput = webDriver.FindElementByXPath("//input[@name='password']");
            var signInButton = webDriver.FindElementByXPath("//input[@value='Sign in']");

            loginInput.SendKeys(login);
            passwordInput.SendKeys(password);
            signInButton.Click();

            return GetSeleniumClient;
        }

        public Selenium AttachFileToComment(string issueUrl, List<string> filePathes, string commentId, bool issueComment)
        {
            webDriver.Url = issueUrl;
            string commentKey = issueComment ? "issuecomment" : "issue";

            var comment = GetForm(webDriver, commentKey, commentId);
            ShowFilesInput(comment);
            var filesInput = GetFilesInput(comment, commentKey, commentId);
            AttachFiles(filesInput, filePathes);
            UploadFiles(webDriver, comment);



            return GetSeleniumClient;

            static IWebElement GetForm(ChromeDriver webDriver, string commentKey, string commentId) 
                => webDriver.FindElementByXPath($".//*[@id='{commentKey}-{commentId}']"); 
            static void ShowFilesInput(IWebElement form)
            {
                var commentMenuButton = form.FindElement(By.XPath(".//summary[not(@aria-label='Add your reaction')]"));
                commentMenuButton.Click();
                var commentEditButton = form.FindElement(By.XPath(".//button[@aria-label='Edit comment']"));
                commentEditButton.Click();
            }
            static IWebElement GetFilesInput(IWebElement form, string commentKey, string commentId) 
                => form.FindElement(By.XPath($".//input[@id='fc-{commentKey}-{commentId}-body']"));
            static void AttachFiles(IWebElement filesInput, List<string> filePathes) 
                => filePathes.ForEach(filesInput.SendKeys);
            static void UploadFiles(ChromeDriver webDriver, IWebElement form)
            {
                //var updateButton = form.FindElement(By.XPath(".//button[@type='submit' and text() = 'Update comment']"));
                //var commentFileAttacment = form.FindElement(By.XPath(".//file-attachment"));
                //Thread.Sleep(500);
                //bool canUploadFiles() => commentFileAttacment.DoesElementHaveClass("is-default");//not is-uploading or errors 


                //var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
                //var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("content-section")));


                //var wait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 30));
                //wait.Until(clickableCondition); //.until(elementToBeClickable(myElem));

                var wait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 2));

                var updateButtonXPath = By.XPath(".//button[@type='submit' and text() = 'Update comment']");
                //var updateButton = form.FindElement(updateButtonXPath);
                var updateButtonWaitingCondition = SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(updateButtonXPath);
                


                var updateButton = wait.Until(updateButtonWaitingCondition);

                updateButton.Click();


                /*
                while (!canUploadFiles())
                    Thread.Sleep(500);//lol spin lock in one thread // lol "semi-spin lock", lool
                */
                //updateButton.Click();





            }
        }


    }

    static public class IWebElementExtensions
    {
        static public bool DoesElementHaveClass(this IWebElement element, string activeClass)
            => element.GetAttribute("class").Contains(activeClass);

    }
}
