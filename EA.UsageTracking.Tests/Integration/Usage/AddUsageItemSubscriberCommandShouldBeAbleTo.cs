using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Core.DTOs;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.Usages.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.Usage
{
    [TestFixture]
    class AddUsageItemSubscriberCommandShouldBeAbleTo: BaseIntegration
    {
        private string _identityToken =
            "eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiVUotcEl0VEZmTElpazJLdFF4VVdtQSIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODY3ODk1NjcsImV4cCI6MTU4Njc5MzE2NywiaWF0IjoxNTg2Nzg5NTY3LCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.BSXjwtmJ4yJf0wcIIr1_JYMng3Yhw6IHrHy0507xO21zzDhWf9wsq2kkvhn0nSMQDTw5sYj6pMAJFNkGMKchk8poT5vgspwhCT-Io1xKVN8a9xVdoxxXJkZrHxv8lw-JoxnC_giLOrvq0XRDE6JsaoLD6EQnxXQKaJ5IzG0784wqMUNOLA7QiHAhFOd5IbvE6zMzbc5jsPIMFRXKF_t71y08CtjYw40wV4Lk8FAvANBwYkPobKzYEZEGNwYQFn3rl5dx4ohJfiennUzVfntDuwh4FELqPSR9dicStUFwBaHvWMdMYSKHRv8GeTpYPY9c2J3JuXrB3z8sicqdt_UhjA";
               
        private Guid _identityGuid = Guid.Parse("53efb3c4-f837-4e11-a622-60a43b0e12a4");

        [Test]
        public async Task HandleAddUsageItemWithNoApplication()
        {
            // Arrange
            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = 1, IdentityToken = _identityToken};

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.True(result.Error.Contains(Constants.ErrorMessages.NoTenantExists));
        }

        [Test]
        public async Task HandleAddUsageItemWithNoEvent()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var user = new ApplicationUser() { Id = Guid.NewGuid() };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var command = new AddUsageItemSubscriberCommand { ApplicationEventId = 1, IdentityToken = _identityToken, TenantId = DbContext.TenantId };

            // Act
            var result = await Mediator.Send(command);

            //Assert
            Assert.True(result.IsFailure);
            Assert.True(result.Error.Contains(Constants.ErrorMessages.NoEventExists));
        }

        [Test]
        public async Task AddUsageItemWithNewUser()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            DbContext.SaveChanges();

            var command = new AddUsageItemSubscriberCommand
            {
                ApplicationEventId = ev.Id, TenantId = DbContext.TenantId,
                IdentityToken = _identityToken
            };

            // Act
            var result = await Mediator.Send(command);
            var newUser =
                DbContext.ApplicationUsers
                    .Include(x => x.UserToApplications)
                    .ThenInclude(y => y.Application)
                    .SingleOrDefault(x =>
                    x.Id == _identityGuid);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsTrue(newUser != null);
            Assert.IsTrue(newUser.UserToApplications.Count == 1);
            Assert.AreEqual(DbContext.TenantId, newUser.UserToApplications.First().Application.TenantId);
        }

        [Test]
        public async Task AddUsageItemWithExistingUserNoAssociation()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            var user = new ApplicationUser
            {
                Id = Guid.Parse("53efb3c4-f837-4e11-a622-60a43b0e12a4"),
                Name = "Test"
            };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();

            var command = new AddUsageItemSubscriberCommand
            {
                ApplicationEventId = ev.Id,
                TenantId = DbContext.TenantId,
                IdentityToken = "eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiVUotcEl0VEZmTElpazJLdFF4VVdtQSIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODY3ODk1NjcsImV4cCI6MTU4Njc5MzE2NywiaWF0IjoxNTg2Nzg5NTY3LCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.BSXjwtmJ4yJf0wcIIr1_JYMng3Yhw6IHrHy0507xO21zzDhWf9wsq2kkvhn0nSMQDTw5sYj6pMAJFNkGMKchk8poT5vgspwhCT-Io1xKVN8a9xVdoxxXJkZrHxv8lw-JoxnC_giLOrvq0XRDE6JsaoLD6EQnxXQKaJ5IzG0784wqMUNOLA7QiHAhFOd5IbvE6zMzbc5jsPIMFRXKF_t71y08CtjYw40wV4Lk8FAvANBwYkPobKzYEZEGNwYQFn3rl5dx4ohJfiennUzVfntDuwh4FELqPSR9dicStUFwBaHvWMdMYSKHRv8GeTpYPY9c2J3JuXrB3z8sicqdt_UhjA"
            };

            // Act
            var result = await Mediator.Send(command);
            var newUser =
                DbContext.ApplicationUsers
                    .Include(x => x.UserToApplications)
                    .ThenInclude(y => y.Application)
                    .SingleOrDefault(x =>
                    x.Id == Guid.Parse("53efb3c4-f837-4e11-a622-60a43b0e12a4"));

            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsTrue(newUser != null);
            Assert.IsTrue(newUser.UserToApplications.Count == 1);
            Assert.AreEqual(DbContext.TenantId, newUser.UserToApplications.First().Application.TenantId);
        }

        [Test]
        public async Task AddUsageItemWithExistingUserAndAssociation()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            var user = new ApplicationUser
            {
                Id = Guid.Parse("53efb3c4-f837-4e11-a622-60a43b0e12a4"),
                Name = "Test"
            };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();
            user.UserToApplications = new List<UserToApplication> { new UserToApplication { ApplicationId = app.Id, UserId = user.Id} };
            DbContext.SaveChanges();

            var command = new AddUsageItemSubscriberCommand
            {
                ApplicationEventId = ev.Id,
                TenantId = DbContext.TenantId,
                IdentityToken = "eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiVUotcEl0VEZmTElpazJLdFF4VVdtQSIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODY3ODk1NjcsImV4cCI6MTU4Njc5MzE2NywiaWF0IjoxNTg2Nzg5NTY3LCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.BSXjwtmJ4yJf0wcIIr1_JYMng3Yhw6IHrHy0507xO21zzDhWf9wsq2kkvhn0nSMQDTw5sYj6pMAJFNkGMKchk8poT5vgspwhCT-Io1xKVN8a9xVdoxxXJkZrHxv8lw-JoxnC_giLOrvq0XRDE6JsaoLD6EQnxXQKaJ5IzG0784wqMUNOLA7QiHAhFOd5IbvE6zMzbc5jsPIMFRXKF_t71y08CtjYw40wV4Lk8FAvANBwYkPobKzYEZEGNwYQFn3rl5dx4ohJfiennUzVfntDuwh4FELqPSR9dicStUFwBaHvWMdMYSKHRv8GeTpYPY9c2J3JuXrB3z8sicqdt_UhjA"
            };

            // Act
            var result = await Mediator.Send(command);
            var newUser =
                DbContext.ApplicationUsers
                    .Include(x => x.UserToApplications)
                    .ThenInclude(y => y.Application)
                    .SingleOrDefault(x =>
                    x.Id == Guid.Parse("53efb3c4-f837-4e11-a622-60a43b0e12a4"));

            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsTrue(newUser != null);
            Assert.IsTrue(newUser.UserToApplications.Count == 1);
            Assert.AreEqual(DbContext.TenantId, newUser.UserToApplications.First().Application.TenantId);
        }

        [Test]
        public async Task AddUsageItemWithExistingUserAndAssociationToDifferentApp()
        {
            // Arrange
            var app = new Core.Entities.Application();
            DbContext.Applications.Add(app);
            DbContext.SaveChanges();

            var newTenantId = Guid.NewGuid().ToString();
            var app2 = new Core.Entities.Application();
            var existingTenantId = DbContext.TenantId;
            DbContext.TenantId = newTenantId;
            DbContext.Applications.Add(app2);
            DbContext.SaveChanges();
            DbContext.TenantId = existingTenantId;

            var ev = new ApplicationEvent();
            DbContext.ApplicationEvents.Add(ev);
            var user = new ApplicationUser
            {
                Id = _identityGuid,
                Name = "Test"
            };
            DbContext.ApplicationUsers.Add(user);
            DbContext.SaveChanges();
            user.UserToApplications = new List<UserToApplication> { new UserToApplication { ApplicationId = app2.Id, UserId = user.Id } };
            DbContext.SaveChanges();

            var command = new AddUsageItemSubscriberCommand
            {
                ApplicationEventId = ev.Id,
                TenantId = DbContext.TenantId,
                IdentityToken = _identityToken };

            // Act
            var result = await Mediator.Send(command);
            var newUser =
                DbContext.ApplicationUsers
                    .Include(x => x.UserToApplications)
                    .ThenInclude(y => y.Application)
                    .SingleOrDefault(x =>
                    x.Id == _identityGuid);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.IsTrue(newUser != null);
            Assert.IsTrue(newUser.UserToApplications.Count == 2);
            Assert.AreEqual(DbContext.TenantId, newUser.UserToApplications.First(x => x.ApplicationId == app.Id).Application.TenantId);
        }
    }
}
