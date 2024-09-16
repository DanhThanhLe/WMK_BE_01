using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientModel;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.IngredientNutrientModel;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_Test.Service
{
	public class IngredientServiceTests
	{
		private readonly IngredientService _ingredientService;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IIngredientRepository> _repositoryMock;
		private readonly Mock<IIngredientNutrientService> _ingredientNutrientServiceMock;
		private readonly Mock<IRecipeService> _recipeServiceMock;
		private readonly Mock<IValidator<CreateIngredientRequest>> _validatorMock;
		private readonly Mock<IValidator<CreateIngredientNutrientRequest>> _createIngredientNutrientValidatorMock;
		public IngredientServiceTests()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_repositoryMock = new Mock<IIngredientRepository>();
			_mapperMock = new Mock<IMapper>();
			_ingredientNutrientServiceMock = new Mock<IIngredientNutrientService>();
			_recipeServiceMock = new Mock<IRecipeService>();
			_validatorMock = new Mock<IValidator<CreateIngredientRequest>>();
			_createIngredientNutrientValidatorMock = new Mock<IValidator<CreateIngredientNutrientRequest>>();

			// Setup unit of work and repository mocks
			_unitOfWorkMock.Setup(x => x.IngredientRepository).Returns(_repositoryMock.Object);
			_unitOfWorkMock.Setup(x => x.IngredientCategoryRepository).Returns(new Mock<IIngredientCategoryRepository>().Object);

			// Create an instance of IngredientService with the mocked dependencies
			_ingredientService = new IngredientService(
				_mapperMock.Object ,
				_unitOfWorkMock.Object ,
				_ingredientNutrientServiceMock.Object ,
				_recipeServiceMock.Object
			);
		}

		[Fact]
		public async Task CreateIngredient_WithValidData_ReturnsSuccess()
		{
			//Arrange
			var createdBy = "CusTest";
			var ingredientCategoryId = Guid.NewGuid();
			var createIngredientRequest = new CreateIngredientRequest
			{
				IngredientCategoryId = ingredientCategoryId ,
				Name = "Tomato" ,
				Img = "imgtest" ,
				Unit = "kg" ,
				Price = 101 ,
				PackagingMethod = "Vacuum Sealed" ,
				PreservationMethod = "Refrigeration" ,
				NutrientInfo = new CreateIngredientNutrientRequest
				{
					Calories = 18 ,
					Fat = 0.2 ,
					SaturatedFat = 0.0 ,
					Sugar = 2.6 ,
					Carbonhydrate = 3.9 ,
					DietaryFiber = 1.2 ,
					Protein = 0.9 ,
					Sodium = 5
				}
			};
			var ingredientCategory = new IngredientCategory { Id = createIngredientRequest.IngredientCategoryId };
			var ingredient = new Ingredient { Name = "Test" , IngredientCategory = ingredientCategory };

			var validResult = new ValidationResult();
			var ingredientList = new List<Ingredient>();
			_unitOfWorkMock.Setup(x => x.IngredientRepository.GetAllAsync())
			 .ReturnsAsync(ingredientList);

			_validatorMock.Setup(x => x.Validate(createIngredientRequest))
				.Returns(validResult); // Xác thực thành công

			_unitOfWorkMock.Setup(x => x.IngredientCategoryRepository.GetByIdAsync(createIngredientRequest.IngredientCategoryId.ToString()))
				.ReturnsAsync(ingredientCategory);

			_mapperMock.Setup(x => x.Map<Ingredient>(createIngredientRequest))
				.Returns(ingredient);

			_unitOfWorkMock.Setup(x => x.IngredientRepository.CreateAsync(ingredient))
				.ReturnsAsync(true);

			// Setup ingredient nutrient service mock
			_ingredientNutrientServiceMock.Setup(x => x.Create(It.IsAny<Guid>() , It.IsAny<CreateIngredientNutrientRequest>()))
				.ReturnsAsync(new ResponseObject<IngredientNutrient> { StatusCode = 200 , Data = new IngredientNutrient() });
			// Act
			var result = await _ingredientService.CreateIngredient(createdBy , createIngredientRequest);

			// Assert
			Assert.Equal(200 , result.StatusCode);
			Assert.NotNull(result.Message);
			//Assert.Equal("Gia khong de trong va phai lon hon 100 VND" , result.Message);
			//_unitOfWorkMock.Verify(x => x.IngredientRepository.DeleteAsync(ingredient.Id.ToString()) , Times.Once);
		}

		[Fact]
		public async Task CreateIngredient_WithValidData_ReturnsUnSuccessWithCreateIngredientNutrient()
		{
			//Arrange
			var createdBy = "CusTest";
			var ingredientCategoryId = Guid.NewGuid();
			var createIngredientRequest = new CreateIngredientRequest
			{
				IngredientCategoryId = ingredientCategoryId ,
				Name = "Tomato" ,
				Img = "imgtest" ,
				Unit = "kg" ,
				Price = 101 ,
				PackagingMethod = "Vacuum Sealed" ,
				PreservationMethod = "Refrigeration" ,
				NutrientInfo = new CreateIngredientNutrientRequest
				{
					Calories = 18 ,
					Fat = 0.2 ,
					SaturatedFat = 0.0 ,
					Sugar = 2.6 ,
					Carbonhydrate = 3.9 ,
					DietaryFiber = 1.2 ,
					Protein = 0.9 ,
					Sodium = 5
				}
			};
			var ingredientCategory = new IngredientCategory { Id = createIngredientRequest.IngredientCategoryId };
			var ingredient = new Ingredient { Name = "Test" , IngredientCategory = ingredientCategory };

			var validResult = new ValidationResult();
			var ingredientList = new List<Ingredient>();
			_unitOfWorkMock.Setup(x => x.IngredientRepository.GetAllAsync())
			 .ReturnsAsync(ingredientList);

			_validatorMock.Setup(x => x.Validate(createIngredientRequest))
				.Returns(validResult); // Xác thực thành công

			_unitOfWorkMock.Setup(x => x.IngredientCategoryRepository.GetByIdAsync(createIngredientRequest.IngredientCategoryId.ToString()))
				.ReturnsAsync(ingredientCategory);

			_mapperMock.Setup(x => x.Map<Ingredient>(createIngredientRequest))
				.Returns(ingredient);

			_unitOfWorkMock.Setup(x => x.IngredientRepository.CreateAsync(ingredient))
				.ReturnsAsync(true);

			// Setup ingredient nutrient service mock
			_ingredientNutrientServiceMock.Setup(x => x.Create(It.IsAny<Guid>() , It.IsAny<CreateIngredientNutrientRequest>()))
				.ReturnsAsync(new ResponseObject<IngredientNutrient> { StatusCode = 500 , Message = "Create nutrient ingredient faild with id: " + ingredientCategory.Id });
			// Act
			var result = await _ingredientService.CreateIngredient(createdBy , createIngredientRequest);

			// Assert
			Assert.Equal(500 , result.StatusCode);
			Assert.NotNull(result.Message);
			Assert.Equal("Create nutrient ingredient faild with id: " + ingredientCategory.Id , result.Message);
			_unitOfWorkMock.Verify(x => x.IngredientRepository.DeleteAsync(ingredient.Id.ToString()) , Times.Once);
		}

		[Fact]
		public async Task CreateIngredient_WithValidData_ReturnsIngredientCategoryNotExist()
		{
			//Arrange
			var createdBy = "CusTest";
			var ingredientCategoryId = Guid.NewGuid();
			var createIngredientRequest = new CreateIngredientRequest
			{
				IngredientCategoryId = ingredientCategoryId ,
				Name = "Tomato" ,
				Img = "imgtest" ,
				Unit = "kg" ,
				Price = 101 ,
				PackagingMethod = "Vacuum Sealed" ,
				PreservationMethod = "Refrigeration" ,
				NutrientInfo = new CreateIngredientNutrientRequest
				{
					Calories = 18 ,
					Fat = 0.2 ,
					SaturatedFat = 0.0 ,
					Sugar = 2.6 ,
					Carbonhydrate = 3.9 ,
					DietaryFiber = 1.2 ,
					Protein = 0.9 ,
					Sodium = 5
				}
			};

			_unitOfWorkMock.Setup(x => x.IngredientCategoryRepository.GetByIdAsync(createIngredientRequest.IngredientCategoryId.ToString()))
				.ReturnsAsync((IngredientCategory)null);//không tồn tại ingredient category

			var ingredientList = new List<Ingredient>();
			_unitOfWorkMock.Setup(x => x.IngredientRepository.GetAllAsync())
			 .ReturnsAsync(ingredientList);

			var validResult = new ValidationResult();
			_validatorMock.Setup(x => x.Validate(createIngredientRequest))
				.Returns(validResult); // Xác thực thành công

			// Act
			var result = await _ingredientService.CreateIngredient(createdBy , createIngredientRequest);

			// Assert
			Assert.Equal(400 , result.StatusCode);
			Assert.NotNull(result.Message);
			Assert.Contains("Ingredient category with id: " , result.Message);
		}

		[Fact]
		public async Task UpdateIngredient_WithValidData_ReturnsSuccess()
		{
			//Arrange
			var ingredientCategoryId = Guid.NewGuid();
			var createIngredientRequest = new CreateIngredientRequest
			{
				IngredientCategoryId = ingredientCategoryId ,
				Name = "Tomato" ,
				Img = "imgtest" ,
				Unit = "kg" ,
				Price = 101 ,
				PackagingMethod = "Vacuum Sealed" ,
				PreservationMethod = "Refrigeration" ,
				NutrientInfo = new CreateIngredientNutrientRequest
				{
					Calories = 18 ,
					Fat = 0.2 ,
					SaturatedFat = 0.0 ,
					Sugar = 2.6 ,
					Carbonhydrate = 3.9 ,
					DietaryFiber = 1.2 ,
					Protein = 0.9 ,
					Sodium = 5
				}
			};

			var ingredientCategory = new IngredientCategory { Id = createIngredientRequest.IngredientCategoryId };
			var ingredientId = Guid.NewGuid();
			var recipeId1 = Guid.NewGuid();
			var recipeId2 = Guid.NewGuid();
			// Tạo một số đối tượng Recipe để mô phỏng
			var recipes = new List<Recipe>
			{
				new Recipe { Id = recipeId1, Name = "Recipe 1" },
				new Recipe { Id = recipeId2, Name = "Recipe 2" }
			};
			var ingredient = new Ingredient
			{
				Id = ingredientId ,
				Name = "Tomato" ,
				IngredientCategory = ingredientCategory ,
				RecipeIngredients = new List<RecipeIngredient>
				{
					new RecipeIngredient { RecipeId = recipeId1, IngredientId = ingredientId },
					new RecipeIngredient { RecipeId = recipeId2, IngredientId = ingredientId }
				} ,
				IngredientNutrient = new IngredientNutrient
				{
					Calories = 0 ,
					Fat = 0.2 ,
					SaturatedFat = 1 ,
					Sugar = 3 ,
					Carbonhydrate = 3.9 ,
					DietaryFiber = 1.2 ,
					Protein = 0.9 ,
					Sodium = 5
				}
			};

			var ingredientList = new List<Ingredient>()
			{
				ingredient
			};
			_unitOfWorkMock.Setup(x => x.IngredientRepository.GetAllAsync())
				.ReturnsAsync(ingredientList);

			_unitOfWorkMock.Setup(x => x.IngredientRepository.GetByIdAsync(ingredient.Id.ToString()))
				.ReturnsAsync(ingredient);

			var validResult = new ValidationResult();
			_validatorMock.Setup(x => x.Validate(createIngredientRequest))
				.Returns(validResult); // Xác thực thành công

			// Setup ingredient nutrient service mock
			_ingredientNutrientServiceMock.Setup(x => x.Update(It.IsAny<Guid>() , new IngredientNutrientRequest()))
				.ReturnsAsync(new ResponseObject<IngredientNutrientResponse> { StatusCode = 200 });

			// Setup recipe service mock
			_recipeServiceMock.Setup(x => x.AutoUpdateRecipeAsync(It.IsAny<Guid>()))
				.ReturnsAsync(true); // Mô phỏng thành công khi cập nhật công thức

			_unitOfWorkMock.Setup(x => x.IngredientRepository.UpdateAsync(It.IsAny<Ingredient>()))
				.ReturnsAsync(true); // Giả lập cập nhật thành công

			// Act
			var result = await _ingredientService.UpdateIngredient(ingredientId , createIngredientRequest);

			// Assert
			Assert.Equal(200 , result.StatusCode);
			Assert.Equal("Update ingredient successfully." , result.Message);

			// Verify that AutoUpdateRecipeAsync was called for each recipe ingredient
			foreach ( var recipeIngredient in ingredient.RecipeIngredients )
			{
				_recipeServiceMock.Verify(x => x.AutoUpdateRecipeAsync(recipeIngredient.RecipeId) , Times.Once);
			}

		}


	}
}
