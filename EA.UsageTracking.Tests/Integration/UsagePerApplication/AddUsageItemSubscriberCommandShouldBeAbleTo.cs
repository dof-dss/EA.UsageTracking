using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EA.UsageTracking.Core.Entities;
using EA.UsageTracking.Infrastructure.Features.UsagesPerApplication.Commands;
using EA.UsageTracking.SharedKernel.Constants;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EA.UsageTracking.Tests.Integration.UsagePerApplication
{
    [TestFixture]
    class AddUsageItemSubscriberCommandShouldBeAbleTo: BaseIntegration
    {
        private string _identityToken =
            "eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiR3hjeUxVaVk4UkhVSTBPSlNjQXY1ZyIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODc0Nzg4NTksImV4cCI6MTU4NzQ4MjQ1OSwiaWF0IjoxNTg3NDc4ODU5LCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.VneuLxIjOlqI_zD2trw9oymCcNdSbXac8fPWe61EWCzGOIX7qcipCbjcWWzy-MUkOhgugUJnOSRIu_Ugl95j8znhtyeqPubcUafbBGNt-s5y7vIhg1i2GgrFY3srdpAQPahOQL15yBsdF-Mrvp4IubY9VMfs3ABgfguCR0DrdatUUUJQtzUmfsVgsPl3WRc3mMOzQIw8TpvdYWE-iiAVlIHul3wCDlyQu3FK0yIS_q3-P8N5pkIyKyISmdvd6IR1eC6IVxuVw_LkrHgOpndqT89TeHN5goaUPGexoInxv-qZ1fIYUGht3wFHXbSyucmZgIlOc1R0LCmbH2OCBgDknw";
            //"eyJraWQiOiJnRmc1RFFFZmpVanZQZXc5Z1UzYVp6M3RISnZDdnRRSVwvNXZMQTZNK3BUQT0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiI1ZmY2YWZiNy1kOGYyLTQ1MzEtOTc0Yy02YzU3YzYzNjE4ZGMiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIGF1ZGl0LWFwaVwvYXVkaXRfYWRtaW4gcGhvbmUgb3BlbmlkIHByb2ZpbGUgZW1haWwiLCJhdXRoX3RpbWUiOjE1ODczOTY3MDUsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTIuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0yX1F0Z29nSDkxdiIsImV4cCI6MTU4NzQwMDMwNSwiaWF0IjoxNTg3Mzk2NzA1LCJ2ZXJzaW9uIjoyLCJqdGkiOiIxMTMxZmVjZi02N2UxLTQ5MGYtYTQ1Ny1jZDE3Njg2N2NiODYiLCJjbGllbnRfaWQiOiIzajBmZWtvNWFpMGI1OGQ5azhlY2RqdXBnYSIsInVzZXJuYW1lIjoiNWZmNmFmYjctZDhmMi00NTMxLTk3NGMtNmM1N2M2MzYxOGRjIn0.A8rMAmPNmPeeibHD2J59cVmOOcc29k0E3VHM3Kb8vzzvvruVQwZ41hVWJkFBNE8W4hCAEkyBChdI6DS0R2PAnA08RN-dW81MbKfgPrXC2nsfZdnwqsi4bBczbSY4RRwV6wuI7yvus6vqBvLZct9BD99wpyZisdd0Km9w2RRbbFaxrlM_tj32NPLQuDHZOCpO_S3TpcbxvpFFqwf1VUPrsiUqT95oY_erfmYVIcNMTT3jtVPlYNsDlNJdB6F9i9jCNh9re7_UfkVMt2G3FfIvMmeLoVmjSP3zFg2ie8ztewjqP8yabOQSOIK2uG5Nj7wdK3qPz81mrfKXWgNWUG2e-g";
            //"eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiaVZ6UzRCTDN1ak1zZFVuYnpNZTZWdyIsInN1YiI6IjVmZjZhZmI3LWQ4ZjItNDUzMS05NzRjLTZjNTdjNjM2MThkYyIsImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInRva2VuX3VzZSI6ImlkIiwiYXV0aF90aW1lIjoxNTg3Mzk2NzA1LCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNWZmNmFmYjctZDhmMi00NTMxLTk3NGMtNmM1N2M2MzYxOGRjIiwiZXhwIjoxNTg3NDAwMzA1LCJpYXQiOjE1ODczOTY3MDUsImVtYWlsIjoiZ3JhaGFtLm1jdmVhQGhvdG1haWwuY28udWsifQ.NjgJkJZvveY5YqgKLF2GKpxCWIlNFokF2A71oCqxkfIdqatWWdyJ8u4Pb23oI82yo4Ynz4x68aTQruF0ymsuNnHOWWpXdm2pk-QSrM8jE7OU09azNT2R58KezM_qIklIqoG3-DyMe-C1nWfXOZb681KyE_w-aw8tlRpkL92yr1Fo7Fy1i3yeU2VHpnYgq_WGeqOoJgHX_wSzJpgxl1lMvv1bI6b6hFB3RWOLvO7DwTR6nd7PiFSHPcN3r2FVS6dyjJCl1jYQX8gcPoGwoxLtqf-uhyvP4OIMXlK-ljgpt3kDjcR8GyM4y8Zlre-z0d7WTdHMavob9xYFZMvCSOOtPQ";
            //"eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoic1Zwbk10RjBWSU5tQnRVcW5UaEVpUSIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODczOTUxMTAsImV4cCI6MTU4NzM5ODcxMCwiaWF0IjoxNTg3Mzk1MTEwLCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.R5l4uaAJmT8TE_Act1Q1cvKToD124aqOHFomHuqHULJ-42hM_sO1fgkTwns7B45ONORuwnnmbw4SB2Njuy8MvvLRqomXIEz56r_YQLx7RQtOr8O2n4hjvngp7Mk_zyB4t20nAePBOeWC3ouCqnTLyRiWG9eYbZKQbdpu30mjKPY63jFXU_PMxFwoekGS406OWW0rbc0hmnLVOJCP7y_BGqoV7kObDEUO-A-eDviEjBEVDISn9QkiKlDkYsxa769FO1SX69RDY_HBu5-C_LUHN6Qlz6vfWUUyj_zfToRpsc7hLVSWwrpXNCfChgKDGcaakNBZq49bk1P4ewtdo1X0Ww";
            //"eyJraWQiOiJnRmc1RFFFZmpVanZQZXc5Z1UzYVp6M3RISnZDdnRRSVwvNXZMQTZNK3BUQT0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIyMTVlZmU3MC04NDVmLTRkYmYtOTk4Yy1lOGVhODcwMzY0OWIiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIGF1ZGl0LWFwaVwvYXVkaXRfYWRtaW4gcGhvbmUgb3BlbmlkIHByb2ZpbGUgZW1haWwiLCJhdXRoX3RpbWUiOjE1ODczOTE0NTksImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTIuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0yX1F0Z29nSDkxdiIsImV4cCI6MTU4NzM5NTA1OSwiaWF0IjoxNTg3MzkxNDU5LCJ2ZXJzaW9uIjoyLCJqdGkiOiJlYWE2NmVmYS03MzJjLTQxZDEtOGZiYi1iMzA3YzUzYjJhYjkiLCJjbGllbnRfaWQiOiIzajBmZWtvNWFpMGI1OGQ5azhlY2RqdXBnYSIsInVzZXJuYW1lIjoiMjE1ZWZlNzAtODQ1Zi00ZGJmLTk5OGMtZThlYTg3MDM2NDliIn0.TwHURk_SLkfDlF7U4i_tuUViWHMqJfkObdc22NZgNf94JYM6x1vHG65W35mFIgutxkvIhzUi0kFMHOk8-8WfZ-RysIt7cMn4AvgUIWdMBinpqposqhCo9nLg1wtSJC8zn1KdM63CB3uzU_ghgbxd_cu900DzWRSpea0AtnlKv8h9vVsLQl_R0oRdBAJFND5G87Y7zhW0AJDSU1yX7V8eCl0kqWr9_29J8-U2dvQh2iJnxoi5cV3q0PD7LyXy1HkW595ciQAv61qDBOQPdxUyc7l7icoyanVkbo9NfH-1nxnMur6Zp2aJsi9rPI88xtbJVu-uSqkDfv-4YYhkszZRzg";
            //"eyJraWQiOiJnRmc1RFFFZmpVanZQZXc5Z1UzYVp6M3RISnZDdnRRSVwvNXZMQTZNK3BUQT0iLCJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIyMTVlZmU3MC04NDVmLTRkYmYtOTk4Yy1lOGVhODcwMzY0OWIiLCJ0b2tlbl91c2UiOiJhY2Nlc3MiLCJzY29wZSI6ImF3cy5jb2duaXRvLnNpZ25pbi51c2VyLmFkbWluIGF1ZGl0LWFwaVwvYXVkaXRfYWRtaW4gcGhvbmUgb3BlbmlkIHByb2ZpbGUgZW1haWwiLCJhdXRoX3RpbWUiOjE1ODczOTA4ODUsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5ldS13ZXN0LTIuYW1hem9uYXdzLmNvbVwvZXUtd2VzdC0yX1F0Z29nSDkxdiIsImV4cCI6MTU4NzM5NDQ4NSwiaWF0IjoxNTg3MzkwODg1LCJ2ZXJzaW9uIjoyLCJqdGkiOiJjM2RmMWIwZS04MjI1LTRmNDktODU2My1iNTUxNTk4OWFiNDAiLCJjbGllbnRfaWQiOiIzajBmZWtvNWFpMGI1OGQ5azhlY2RqdXBnYSIsInVzZXJuYW1lIjoiMjE1ZWZlNzAtODQ1Zi00ZGJmLTk5OGMtZThlYTg3MDM2NDliIn0.ilw9C9mm_FO-R9AVn1DDwMPS6xEI94vXkyUaxpkLGEfatV2nu991IAa8alWvfpbXBj01XNjGpjiU8YsOKThGUs7XuB1f0AVYO45RJNc1uKa3906Y2NaKaWk6kD42cfZuUcSS8PSbHL3hzd-e5C14Ec3k3GNKT0--swlCnVy5sxSnPeCOzG-K43boXyZyHCaOeaPNjOthbgGQ0cyDOVUmo6BR3_EjdYi2UklJvwAY1nouTN6q27twOf4QlrAN5MJm_neJq1Jd3nS_scDG-GETk622NCktBQp_dM89j7WLlNmN5kDr2odVN9VlRjNXGBmPWjIK9p6Y1v002M-IrMlq5A";
            //"eyJraWQiOiJoUk1YMXZUQU8zUmpXeHloVG4zZUpSblFZejNpMWZTT044WjV3d0VCVGJrPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiVUotcEl0VEZmTElpazJLdFF4VVdtQSIsInN1YiI6IjUzZWZiM2M0LWY4MzctNGUxMS1hNjIyLTYwYTQzYjBlMTJhNCIsImNvZ25pdG86Z3JvdXBzIjpbIkFkbWluIl0sImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuZXUtd2VzdC0yLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMl9RdGdvZ0g5MXYiLCJjb2duaXRvOnVzZXJuYW1lIjoiNTNlZmIzYzQtZjgzNy00ZTExLWE2MjItNjBhNDNiMGUxMmE0IiwiY29nbml0bzpyb2xlcyI6WyJhcm46YXdzOmlhbTo6MTExMDk0ODE4MjE3OnJvbGVcL0FQSUdhdGV3YXlJbnZva2VSb2xlIl0sImF1ZCI6IjNqMGZla281YWkwYjU4ZDlrOGVjZGp1cGdhIiwidG9rZW5fdXNlIjoiaWQiLCJhdXRoX3RpbWUiOjE1ODY3ODk1NjcsImV4cCI6MTU4Njc5MzE2NywiaWF0IjoxNTg2Nzg5NTY3LCJlbWFpbCI6ImdyYWhhbS5tY3ZlYUBnbWFpbC5jb20ifQ.BSXjwtmJ4yJf0wcIIr1_JYMng3Yhw6IHrHy0507xO21zzDhWf9wsq2kkvhn0nSMQDTw5sYj6pMAJFNkGMKchk8poT5vgspwhCT-Io1xKVN8a9xVdoxxXJkZrHxv8lw-JoxnC_giLOrvq0XRDE6JsaoLD6EQnxXQKaJ5IzG0784wqMUNOLA7QiHAhFOd5IbvE6zMzbc5jsPIMFRXKF_t71y08CtjYw40wV4Lk8FAvANBwYkPobKzYEZEGNwYQFn3rl5dx4ohJfiennUzVfntDuwh4FELqPSR9dicStUFwBaHvWMdMYSKHRv8GeTpYPY9c2J3JuXrB3z8sicqdt_UhjA";

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
