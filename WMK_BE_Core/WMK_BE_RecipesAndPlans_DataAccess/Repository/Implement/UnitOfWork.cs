﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class UnitOfWork : IUnitOfWork
	{
		public IIngredientRepository IngredientRepository { get; private set;}
		public ICategoryRepository CategoryRepository { get; private set;}

        private readonly RecipesAndPlansContext _context;
        public UnitOfWork(RecipesAndPlansContext context)
        {
            this._context = context;
            IngredientRepository = new IngredientRepository(context);
            CategoryRepository = new CategoryRepository(context);
        }
        public async Task CompleteAsync()
        {
            await this._context.SaveChangesAsync();
        }
    }
}
