﻿namespace IntegrationTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity.DocumentDB;
    using NUnit.Framework;
    using System;

    // todo low - validate all tests work
    [TestFixture]
    public class UserStoreTests : UserIntegrationTestsBase
    {
        [Test]
        public async Task Create_NewUser_Saves()
        {
            var userName = "name";
            var user = new IdentityUser { UserName = userName };
            var manager = GetUserManager();

            await manager.CreateAsync(user);

            var savedUser = Client.CreateDocumentQuery<IdentityUser>(Users.DocumentsLink).AsEnumerable().FirstOrDefault();
            Expect(savedUser.UserName, Is.EqualTo(user.UserName));
        }

        [Test]
        public async Task FindByName_SavedUser_ReturnsUser()
        {
            var userName = "name";
            var user = new IdentityUser { UserName = userName };
            var manager = GetUserManager();
            await manager.CreateAsync(user);

            var foundUser = await manager.FindByNameAsync(userName);

            Expect(foundUser, Is.Not.Null);
            Expect(foundUser.UserName, Is.EqualTo(userName));
        }

        [Test]
        public async Task FindByName_NoUser_ReturnsNull()
        {
            var manager = GetUserManager();

            var foundUser = await manager.FindByNameAsync("nouserbyname");

            Expect(foundUser, Is.Null);
        }

        [Test]
        public async Task FindById_SavedUser_ReturnsUser()
        {
            var userId = Guid.NewGuid().ToString();
            var user = new IdentityUser { UserName = "name" };
            user.Id = userId;
            var manager = GetUserManager();
            await manager.CreateAsync(user);

            var foundUser = await manager.FindByIdAsync(userId);

            Expect(foundUser, Is.Not.Null);
            Expect(foundUser.Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task FindById_NoUser_ReturnsNull()
        {
            var manager = GetUserManager();

            var foundUser = await manager.FindByIdAsync(Guid.NewGuid().ToString());

            Expect(foundUser, Is.Null);
        }

        [Test]
        public async Task FindById_IdIsNotAnObjectId_ReturnsNull()
        {
            var manager = GetUserManager();

            var foundUser = await manager.FindByIdAsync("notanobjectid");

            Expect(foundUser, Is.Null);
        }

        [Test]
        public async Task Delete_ExistingUser_Removes()
        {
            var user = new IdentityUser { UserName = "name" };
            var manager = GetUserManager();
            await manager.CreateAsync(user);
            Expect(Client.CreateDocumentQuery<IdentityUser>(Users.DocumentsLink).AsEnumerable(), Is.Not.Empty);

            await manager.DeleteAsync(user);

            Expect(Client.CreateDocumentQuery<IdentityUser>(Users.DocumentsLink).AsEnumerable(), Is.Empty);
        }

        [Test]
        public async Task Update_ExistingUser_Updates()
        {
            var user = new IdentityUser { UserName = "name" };
            var manager = GetUserManager();
            await manager.CreateAsync(user);
            var savedUser = await manager.FindByIdAsync(user.Id);
            savedUser.UserName = "newname";

            await manager.UpdateAsync(savedUser);

            var changedUser = Client.CreateDocumentQuery<IdentityUser>(Users.DocumentsLink).AsEnumerable().FirstOrDefault();
            Expect(changedUser, Is.Not.Null);
            Expect(changedUser.UserName, Is.EqualTo("newname"));
        }

        [Test]
        public async Task SimpleAccessorsAndGetters()
        {
            var user = new IdentityUser
            {
                UserName = "username"
            };
            var manager = GetUserManager();
            await manager.CreateAsync(user);

            Expect(await manager.GetUserIdAsync(user), Is.EqualTo(user.Id));
            Expect(await manager.GetUserNameAsync(user), Is.EqualTo("username"));

            await manager.SetUserNameAsync(user, "newUserName");
            Expect(await manager.GetUserNameAsync(user), Is.EqualTo("newUserName"));
        }
    }
}