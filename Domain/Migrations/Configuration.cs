namespace Domain.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Domain.Entities;
    using Domain.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<Domain.Contexts.InoDriveContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Domain.Contexts.InoDriveContext context)
        {
           
            //add new client for Api
            
            Client newClient = new Client
            {
                Id = "testQueries",
                Secret = CryptHelper.GetHash("123@abc"),
                Name = "Front-end dev",
                Active = true,
                RefreshTokenLifeTime = 302400,
                ApplicationType = Domain.Models.ApplicationTypes.NativeConfidential,
                AllowedOrigin = "*",
            };
            User user = new User
            {
                Id = "b3afa534-8554-476d-96be-4303877ec621",
                PasswordHash = "AGkxnSNdgjV4/pjHeInyOpouESe+3NAPOAEg+6EJY0oF+g2x/tIgABrh0LYbOyBF8w==",
                SecurityStamp = "06a5b81e-5ab1-4669-9809-b9d72024e379",
                Email = "qq@qq.qq",
                UserName = "qq@qq.qq",
                UserProfile = new UserProfile { 
                    FirstName= "Пользователь",
                    LastName = "Тестовый"
                }
            };
            if (context.Clients.FirstOrDefault(x => x.Id == newClient.Id) == null)
            {
                context.Clients.Add(newClient);
                context.Users.Add(user);
            }
        }
    }
}
