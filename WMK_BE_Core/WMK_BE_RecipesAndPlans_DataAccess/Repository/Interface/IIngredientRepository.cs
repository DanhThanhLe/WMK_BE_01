﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface
{
	public interface IIngredientRepository : IBaseRepository<Ingredient>
	{
		public Task<bool> ChangeStatusAsync(Guid id, BaseStatus status);
	}
}
