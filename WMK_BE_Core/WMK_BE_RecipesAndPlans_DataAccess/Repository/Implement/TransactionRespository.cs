using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_RecipesAndPlans_DataAccess.Repository.Implement
{
	public class TransactionRespository : BaseRepository<Transaction>, ITransactionRepository
	{
        public TransactionRespository(WeMealKitContext context) : base(context)
        {
            
        }
    }
}
