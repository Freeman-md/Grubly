using Grubly.Models;
using Grubly.Tests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Grubly.Tests.Identity
{
    public class RegisterPageTests : IClassFixture<RegisterPageFixture>
    {
        private readonly RegisterPageFixture _fixture;
        public RegisterPageTests(RegisterPageFixture fixture) {
            _fixture = fixture;
        }

        [Fact]
        public async Task OnPostAsync_ValidInput_RedirectsToRegisterConfirmationPage()
        {
            #region Arrange
            _fixture.SetRegisterInput("test@example.com", "StrongPassword123!", "StrongPassword123!");

            _fixture.MockUserManager.Setup(
                um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())
                )
                .ReturnsAsync(IdentityResult.Success);
            _fixture.MockUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                           .ReturnsAsync("dummy-token");

            
            _fixture.MockUserManager.Setup(um => um.GetUserIdAsync(It.IsAny<ApplicationUser>()))
                           .ReturnsAsync("dummy-user-id");
            #endregion

            #region Act
            var result = await _fixture.RegisterModel.OnPostAsync();
            #endregion

            #region Assert
            var redirectResult = result.Should().BeOfType<RedirectToPageResult>().Subject;
            redirectResult.PageName.Should().Be("/Account/RegisterConfirmation");

            _fixture.MockUserManager.Verify(
                um => um.CreateAsync(
                    It.Is<ApplicationUser>(
                    user => user.UserName == "test@example.com" && user.Email == "test@example.com"
                    ), 
                    "StrongPassword123!"                   
                ),
                Times.Once
                );
            #endregion 
        }
    }
}
