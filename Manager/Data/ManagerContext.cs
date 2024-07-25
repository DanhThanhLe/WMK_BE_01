using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace Manager.Data
{
    public class ManagerContext : DbContext
    {
        public ManagerContext (DbContextOptions<ManagerContext> options)
            : base(options)
        {
        }

        public DbSet<WMK_BE_RecipesAndPlans_DataAccess.Models.Category> Category { get; set; } = default!;
    }
}
