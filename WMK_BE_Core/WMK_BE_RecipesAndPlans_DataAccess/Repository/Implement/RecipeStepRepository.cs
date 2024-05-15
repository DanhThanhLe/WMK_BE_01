using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
    public class RecipeStepRepository : BaseRepository<RecipeStep>, IRecipeStepRepository 
    {
        public RecipeStepRepository(RecipesAndPlansContext context) : base(context) { }

        public override Task<List<RecipeStep>> GetAllAsync()
        {
            return _dbSet.Include(r => r.Recipe)
                .ThenInclude(rs => rs.RecipeSteps)
                .ToListAsync();
        }

        //public override async Task<RecipeStep?> GetByIdAsync(string id)
        //{
        //    try
        //    {
        //        Guid guid;
        //        if(!Guid.TryParse(id, out guid))
        //        {
        //            return null;
        //        }
        //        var result = await _dbSet.Include(r => r.Recipe)
        //            .ThenInclude(rs => rs.RecipeSteps)
        //            .FirstOrDefaultAsync(rs => rs.Id == guid);
        //        return result;
        //    }
        //}
    }
}
