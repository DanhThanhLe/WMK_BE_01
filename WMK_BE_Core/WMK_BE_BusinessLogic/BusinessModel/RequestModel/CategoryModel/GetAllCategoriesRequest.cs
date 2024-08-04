using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMK_BE_BusinessLogic.BusinessModel.RequestModel.CategoryModel
{
	public class GetAllCategoriesRequest
	{
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
