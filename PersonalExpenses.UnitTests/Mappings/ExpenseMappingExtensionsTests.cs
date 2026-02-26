using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Mapppings;
using PersonalExpenses.Domain.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace PersonalExpenses.UnitTests.Mappings
{
    public class ExpenseMappingExtensionsTests
    {
        [Fact]
        public void ToResponse_WithValidExpense_ReturnsMappedResponse()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            Expense expense = new()
            {
                Id = 1,
                Title = "Lunch",
                Amount = 12.50m,
                Date = now,
                Category = "Food"
            };

            // Act
            ExpenseResponse response = expense.ToResponse();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expense.Id, response.Id);
            Assert.Equal(expense.Title, response.Title);
            Assert.Equal(expense.Amount, response.Amount);
            Assert.Equal(expense.Date, response.Date);
            Assert.Equal(expense.Category, response.Category);
        }

        [Fact]
        public void ToResponseList_WithValidExpenses_ReturnsMappedList()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            List<Expense> expenses = new()
            {
                new() { Id = 1, Title = "Lunch", Amount = 12.50m, Date = now, Category = "Food" },
                new() { Id = 2, Title = "Taxi", Amount = 8.75m, Date = now.AddDays(-1), Category = "Transport" },
                new() { Id = 3, Title = "Gas", Amount = 50m, Date = now.AddDays(-2), Category = "Transport" }
            };

            // Act
            IList<ExpenseResponse> responses = expenses.ToResponseList();

            // Assert
            Assert.NotNull(responses);
            Assert.Equal(3, responses.Count);
            Assert.Equal(expenses[0].Id, responses[0].Id);
            Assert.Equal(expenses[1].Title, responses[1].Title);
            Assert.Equal(expenses[2].Category, responses[2].Category);
        }

        [Fact]
        public void ToResponseList_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            List<Expense> expenses = new();

            // Act
            IList<ExpenseResponse> responses = expenses.ToResponseList();

            // Assert
            Assert.NotNull(responses);
            Assert.Empty(responses);
        }

        [Fact]
        public void ToEntity_WithValidCreateRequest_ReturnsMappedEntity()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            CreateExpenseRequest request = new()
            {
                Title = "Lunch",
                Amount = 12.50m,
                Date = now,
                Category = "Food"
            };

            // Act
            Expense entity = request.ToEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(request.Title, entity.Title);
            Assert.Equal(request.Amount, entity.Amount);
            Assert.Equal(request.Date, entity.Date);
            Assert.Equal(request.Category, entity.Category);
            Assert.Equal(0, entity.Id); 
        }

        [Fact]
        public void ToEntity_WithValidUpdateRequest_ReturnsMappedEntity()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            UpdateExpenseRequest request = new()
            {
                Title = "Updated Lunch",
                Amount = 15m,
                Date = now,
                Category = "Food"
            };

            // Act
            Expense entity = request.ToEntity();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(request.Title, entity.Title);
            Assert.Equal(request.Amount, entity.Amount);
            Assert.Equal(request.Date, entity.Date);
            Assert.Equal(request.Category, entity.Category);
        }

        [Fact]
        public void UpdateEntity_WithValidRequest_UpdatesEntity()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            Expense entity = new()
            {
                Id = 1,
                Title = "Old Title",
                Amount = 10m,
                Date = now.AddDays(-1),
                Category = "OldCategory"
            };

            UpdateExpenseRequest request = new()
            {
                Title = "New Title",
                Amount = 20m,
                Date = now,
                Category = "NewCategory"
            };

            // Act
            request.UpdateEntity(entity);

            // Assert
            Assert.Equal("New Title", entity.Title);
            Assert.Equal(20m, entity.Amount);
            Assert.Equal(now, entity.Date);
            Assert.Equal("NewCategory", entity.Category);
            Assert.Equal(1, entity.Id);
        }

        [Fact]
        public void UpdateEntity_PreservesEntityId()
        {
            // Arrange
            int originalId = 42;
            Expense entity = new()
            {
                Id = originalId,
                Title = "Old Title",
                Amount = 10m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            UpdateExpenseRequest request = new()
            {
                Title = "New Title",
                Amount = 20m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            // Act
            request.UpdateEntity(entity);

            // Assert
            Assert.Equal(originalId, entity.Id);
        }
    }
}
