// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using CFS_1507.Infrastructure.Persistence;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.EntityFrameworkCore;

// namespace CFS_1507.Infrastructure.Extension
// {
//     public static class Migration
//     {
//         public static void ApplyMigration(this WebApplication app)
//         {
//             using var scope = app.Services.CreateScope();
//             var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//             dbContext.Database.Migrate();
//         }
//     }
// }