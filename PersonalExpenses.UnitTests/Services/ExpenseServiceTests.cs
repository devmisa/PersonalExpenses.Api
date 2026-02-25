using FluentValidation;
using Moq;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Application.Services;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PersonalExpenses.UnitTests.Services
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IExpenseRepository> _mockRepository;
        private readonly ExpenseService _service;

        public ExpenseServiceTests()
        {
            _mockRepository = new Mock<IExpenseRepository>();
            _service = new ExpenseService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsExpenseResponse()
        {
            // Arrange
            int expenseId = 1;
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            Expense expense = new()
            {
                Id = expenseId,
                Title = "Lunch",
                Amount = 12.50m,
                Date = yesterday,
                Category = "Food"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(expenseId)).ReturnsAsync(expense);

            // Act
            ExpenseResponse result = await _service.GetByIdAsync(expenseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseId, result.Id);
            Assert.Equal("Lunch", result.Title);
            _mockRepository.Verify(r => r.GetByIdAsync(expenseId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsException()
        {
            // Arrange
            int expenseId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(expenseId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetByIdAsync(expenseId));
            _mockRepository.Verify(r => r.GetByIdAsync(expenseId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithPagination_ReturnsPaginatedResponse()
        {
            // Arrange
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            List<Expense> expenses = new()
            {
                new() { Id = 1, Title = "Lunch", Amount = 12.50m, Date = yesterday, Category = "Food" },
                new() { Id = 2, Title = "Taxi", Amount = 8.75m, Date = yesterday.AddDays(-1), Category = "Transport" }
            };

            GetExpensesQueryParams queryParams = new() { Page = 1, PageSize = 20, Category = null };

            _mockRepository.Setup(r => r.GetListAsync(1, 20, null))
                .ReturnsAsync((expenses, 2));

            // Act
            PaginatedResponse<ExpenseResponse> result = await _service.GetAllAsync(queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(20, result.PageSize);
            Assert.Equal(2, result.TotalCount);
            _mockRepository.Verify(r => r.GetListAsync(1, 20, null), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithCategoryFilter_ReturnsCategorizedExpenses()
        {
            // Arrange
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            List<Expense> expenses = new()
            {
                new() { Id = 1, Title = "Lunch", Amount = 12.50m, Date = yesterday, Category = "Food" },
                new() { Id = 2, Title = "Dinner", Amount = 15m, Date = yesterday.AddDays(-1), Category = "Food" }
            };

            GetExpensesQueryParams queryParams = new() { Page = 1, PageSize = 20, Category = "Food" };

            _mockRepository.Setup(r => r.GetListAsync(1, 20, "Food"))
                .ReturnsAsync((expenses, 2));

            // Act
            PaginatedResponse<ExpenseResponse> result = await _service.GetAllAsync(queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("Food", queryParams.Category);
            _mockRepository.Verify(r => r.GetListAsync(1, 20, "Food"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_CreatesAndReturnsExpense()
        {
            // Arrange
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            CreateExpenseRequest request = new()
            {
                Title = "Lunch",
                Amount = 12.50m,
                Date = yesterday,
                Category = "Food"
            };

            Expense createdExpense = new()
            {
                Id = 1,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                Category = request.Category
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Expense>())).ReturnsAsync(createdExpense);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            ExpenseResponse result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Lunch", result.Title);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidRequest_ThrowsValidationException()
        {
            // Arrange
            CreateExpenseRequest request = new()
            {
                Title = "", // Invalid: empty title
                Amount = -10m, // Invalid: negative amount
                Date = DateTime.UtcNow,
                Category = ""
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await _service.CreateAsync(request));
        }

        [Fact]
        public async Task UpdateAsync_WithValidRequest_UpdatesAndReturnsExpense()
        {
            // Arrange
            int expenseId = 1;
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            UpdateExpenseRequest request = new()
            {
                Title = "Updated Lunch",
                Amount = 15m,
                Date = yesterday,
                Category = "Food"
            };

            Expense existingExpense = new()
            {
                Id = expenseId,
                Title = "Old Lunch",
                Amount = 12.50m,
                Date = yesterday.AddDays(-1),
                Category = "Food"
            };

            Expense updatedExpense = new()
            {
                Id = expenseId,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                Category = request.Category
            };

            _mockRepository.Setup(r => r.GetByIdAsync(expenseId)).ReturnsAsync(existingExpense);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Expense>())).ReturnsAsync(updatedExpense);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            ExpenseResponse result = await _service.UpdateAsync(expenseId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseId, result.Id);
            Assert.Equal("Updated Lunch", result.Title);
            Assert.Equal(15m, result.Amount);
            _mockRepository.Verify(r => r.GetByIdAsync(expenseId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Expense>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int expenseId = 999;
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            UpdateExpenseRequest request = new()
            {
                Title = "Updated",
                Amount = 15m,
                Date = yesterday,
                Category = "Food"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(expenseId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.UpdateAsync(expenseId, request));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesExpenseAndReturnsTrue()
        {
            // Arrange
            int expenseId = 1;
            Expense expense = new()
            {
                Id = expenseId,
                Title = "Lunch",
                Amount = 12.50m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(expenseId)).ReturnsAsync(expense);
            _mockRepository.Setup(r => r.DeleteAsync(expense)).ReturnsAsync(expense);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            bool result = await _service.DeleteAsync(expenseId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.GetByIdAsync(expenseId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(expense), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            int expenseId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(expenseId))
                .ReturnsAsync((Expense?)null);

            // Act
            bool result = await _service.DeleteAsync(expenseId);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Expense>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
