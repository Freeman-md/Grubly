using Grubly.Areas.Identity.Pages.Account;
using Grubly.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace Grubly.Tests.Fixtures
{
    public class RegisterPageFixture
    {
        public Mock<IUserEmailStore<ApplicationUser>> MockUserStore { get; }
        public Mock<UserManager<ApplicationUser>> MockUserManager { get; }
        public Mock<SignInManager<ApplicationUser>> MockSignInManager { get; }
        public Mock<ILogger<RegisterModel>> MockLogger { get; }
        public Mock<IEmailSender> MockEmailSender { get; }
        public RegisterModel RegisterModel { get; }

        public RegisterPageFixture()
        {
            // Initialize MockUserStore as IUserEmailStore<ApplicationUser>
            MockUserStore = new Mock<IUserEmailStore<ApplicationUser>>();

            // Initialize MockUserManager with the MockUserStore
            MockUserManager = new Mock<UserManager<ApplicationUser>>(
                MockUserStore.Object, null, null, null, null, null, null, null, null);

            // Setup SupportsUserEmail to return true
            MockUserManager.Setup(m => m.SupportsUserEmail).Returns(true);

            // Mock SignInManager dependencies
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockUserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            MockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                MockUserManager.Object,
                mockHttpContextAccessor.Object,
                mockUserPrincipalFactory.Object,
                null, null, null, null);

            // Initialize other mocks
            MockLogger = new Mock<ILogger<RegisterModel>>();
            MockEmailSender = new Mock<IEmailSender>();

            // Instantiate RegisterModel with mocked dependencies
            RegisterModel = new RegisterModel(
                MockUserManager.Object,
                MockUserStore.Object,
                MockSignInManager.Object,
                MockLogger.Object,
                MockEmailSender.Object)
            {
                Input = new RegisterModel.InputModel()
            };

            // Mock the Url.Page method
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(u => u.Page(
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
                .Returns("http://localhost/Account/ConfirmEmail?area=Identity&userId=dummy-user-id&code=dummy-token&returnUrl=/");

            RegisterModel.Url = mockUrlHelper.Object;

            // Mock the Request.Scheme property
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(r => r.Scheme).Returns("https");

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

            // Assign the mocked HttpContext to PageContext
            RegisterModel.PageContext = new PageContext
            {
                HttpContext = mockHttpContext.Object,
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<PageModel>(
                    new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                    new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
            };

        }

        // Helper method to set input data dynamically
        public void SetRegisterInput(string email, string password, string confirmPassword)
        {
            RegisterModel.Input.Email = email;
            RegisterModel.Input.Password = password;
            RegisterModel.Input.ConfirmPassword = confirmPassword;
        }
    }
}
