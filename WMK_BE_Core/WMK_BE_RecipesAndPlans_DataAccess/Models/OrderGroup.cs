using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;

namespace WMK_BE_RecipesAndPlans_DataAccess.Models
{
	public class OrderGroup
	{
		public Guid Id { get; set; }
		[ForeignKey(nameof(User))]
		public Guid ShipperId { get; set; }

		public string Location { get; set; } = string.Empty;
		public double Longitude { get; set; }//kinh dộ
		public double Latitude { get; set; }//vĩ độ
		//[NotMapped]
		//public double[] Coordinates
		//{
		//	get => string.IsNullOrEmpty(CoordinatesJson) ? Array.Empty<double>() : JsonSerializer.Deserialize<double[]>(CoordinatesJson) ?? Array.Empty<double>();
		//	set => CoordinatesJson = JsonSerializer.Serialize(value);
		//}
		public DateTime AsignAt { get; set; }
		public Guid AsignBy { get; set; }
		public BaseStatus Status { get; set; }


		public virtual User User { get; set; }

		public virtual List<Order> Orders { get; set; }

		public OrderGroup()
		{

		}
	}
}
