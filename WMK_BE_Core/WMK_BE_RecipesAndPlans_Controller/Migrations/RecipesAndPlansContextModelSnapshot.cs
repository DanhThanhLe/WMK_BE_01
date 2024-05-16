﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WMK_BE_RecipesAndPlans_DataAccess.Models;

#nullable disable

namespace WMK_BE_RecipesAndPlans_Controller.Migrations
{
    [DbContext(typeof(RecipesAndPlansContext))]
    partial class RecipesAndPlansContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.BankingInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameBanking")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("BankingInfo");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.CustomeWeeklyPlan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StandardWeeklyPlanId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("CustomeWeeklyPlans");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Feedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Ingredient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Nutrition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Calories")
                        .HasColumnType("float");

                    b.Property<double>("Carbonhydrate")
                        .HasColumnType("float");

                    b.Property<double>("DietaryFiber")
                        .HasColumnType("float");

                    b.Property<double>("Fat")
                        .HasColumnType("float");

                    b.Property<double>("Protein")
                        .HasColumnType("float");

                    b.Property<Guid>("RecipeID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("SaturatedFat")
                        .HasColumnType("float");

                    b.Property<double>("Sodium")
                        .HasColumnType("float");

                    b.Property<double>("Sugar")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("RecipeID")
                        .IsUnique();

                    b.ToTable("Nutritions");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ShipDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("StanderdWeeklyPlanId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<double>("TotalPrice")
                        .HasColumnType("float");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("StanderdWeeklyPlanId")
                        .IsUnique()
                        .HasFilter("[StanderdWeeklyPlanId] IS NOT NULL");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ApprovedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Difficulty")
                        .HasColumnType("int");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Popularity")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("ProcessStatus")
                        .HasColumnType("int");

                    b.Property<int>("ServingSize")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipeAmount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<Guid>("IngredientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("IngredientId");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeAmounts");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipeCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeCategories");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipePLan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StandardWeeklyPlanId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.HasIndex("StandardWeeklyPlanId");

                    b.ToTable("RecipesPlans");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipeStep", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<string>("MediaURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RecipeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("RecipeSteps");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<Guid>("BankingInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExtraData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Signature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankingInfoId");

                    b.HasIndex("OrderId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmailConfirm")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.WeeklyPlan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ApprovedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("BeginDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProcessStatus")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WeeklyPlans");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.CustomeWeeklyPlan", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", "Order")
                        .WithMany("CustomeWeeklyPlans")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Feedback", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", "Order")
                        .WithMany("FeedBacks")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Nutrition", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", "Recipe")
                        .WithOne("Nutrition")
                        .HasForeignKey("WMK_BE_RecipesAndPlans_DataAccess.Models.Nutrition", "RecipeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.WeeklyPlan", "WeeklyPlan")
                        .WithOne("Order")
                        .HasForeignKey("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", "StanderdWeeklyPlanId");

                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WeeklyPlan");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipeAmount", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Ingredient", "Ingredient")
                        .WithMany("RecipeAmounts")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", "Recipe")
                        .WithMany("RecipeAmounts")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ingredient");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipeCategory", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Category", "Category")
                        .WithMany("RecipeCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", "Recipe")
                        .WithMany("RecipeCategories")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipePLan", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", "Recipe")
                        .WithMany("RecipePlans")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.WeeklyPlan", "WeeklyPlan")
                        .WithMany("RecipePLans")
                        .HasForeignKey("StandardWeeklyPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");

                    b.Navigation("WeeklyPlan");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.RecipeStep", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", "Recipe")
                        .WithMany("RecipeSteps")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Transaction", b =>
                {
                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.BankingInfo", "BankingInfo")
                        .WithMany("Transactions")
                        .HasForeignKey("BankingInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", "Order")
                        .WithMany("Transactions")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BankingInfo");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.BankingInfo", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Category", b =>
                {
                    b.Navigation("RecipeCategories");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Ingredient", b =>
                {
                    b.Navigation("RecipeAmounts");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Order", b =>
                {
                    b.Navigation("CustomeWeeklyPlans");

                    b.Navigation("FeedBacks");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.Recipe", b =>
                {
                    b.Navigation("Nutrition")
                        .IsRequired();

                    b.Navigation("RecipeAmounts");

                    b.Navigation("RecipeCategories");

                    b.Navigation("RecipePlans");

                    b.Navigation("RecipeSteps");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.User", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("WMK_BE_RecipesAndPlans_DataAccess.Models.WeeklyPlan", b =>
                {
                    b.Navigation("Order")
                        .IsRequired();

                    b.Navigation("RecipePLans");
                });
#pragma warning restore 612, 618
        }
    }
}
