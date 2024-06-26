﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
    [Table("RecipesPlans")]
    public class RecipePLan
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Recipe))]
        public Guid RecipeId { get; set; }
        [ForeignKey(nameof(WeeklyPlan))]
        public Guid StandardWeeklyPlanId { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }

        //reference
        public virtual Recipe Recipe { get; set; }
        public virtual WeeklyPlan WeeklyPlan { get; set; }

    }
}
