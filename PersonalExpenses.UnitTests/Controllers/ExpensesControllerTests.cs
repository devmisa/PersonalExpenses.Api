using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalExpenses.Api.Controllers;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace PersonalExpenses.UnitTests.Controllers
{
    public class ExpensesControllerTests
    {
        private static ExpensesController CreateController(Mock<IExpenseService> mockService, int userId = 1)
        {
            ExpensesController controller = new(mockService.Object);
            ClaimsPrincipal user = new(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "TestAuth"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            return controller;
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithPaginatedExpenses()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            List<ExpenseResponse> expenses =
            [
                new() { Id = 1, Title = "Lunch", Amount = 12.50m, Date = now, Category = "Food" },
                new() { Id = 2, Title = "Taxi", Amount = 8.75m, Date = now.AddDays(-1), Category = "Transport" }
            ];

            PaginatedResponse<ExpenseResponse> paginatedResponse = new()
            {
                Data = expenses,
                Page = 1,
                PageSize = 20,
                TotalCount = 2
            };

            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.GetAllAsync(It.IsAny<GetExpensesQueryParams>(), It.IsAny<int>())).ReturnsAsync(paginatedResponse);

            ExpensesController controller = CreateController(mockService);

            // Act
            GetExpensesQueryParams queryParams = new() { Page = 1, PageSize = 20 };
            IActionResult result = await controller.GetAll(queryParams);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            PaginatedResponse<ExpenseResponse> returnedResponse = Assert.IsType<PaginatedResponse<ExpenseResponse>>(okResult.Value);
            Assert.Equal(2, returnedResponse.Data.Count);
            Assert.Equal(1, returnedResponse.Page);
            mockService.Verify(s => s.GetAllAsync(It.IsAny<GetExpensesQueryParams>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithExpense()
        {
            // Arrange
            int expenseId = 1;
            DateTime now = DateTime.UtcNow;
            ExpenseResponse expense = new()
            {
                Id = expenseId,
                Title = "Lunch",
                Amount = 12.50m,
                Date = now,
                Category = "Food"
            };

            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.GetByIdAsync(expenseId, It.IsAny<int>())).ReturnsAsync(expense);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.GetById(expenseId);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ExpenseResponse returnedExpense = Assert.IsType<ExpenseResponse>(okResult.Value);
            Assert.Equal(expense.Id, returnedExpense.Id);
            Assert.Equal(expense.Title, returnedExpense.Title);
            mockService.Verify(s => s.GetByIdAsync(expenseId, It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((ExpenseResponse?)null);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.GetById(999);

            // Assert
            _ = Assert.IsType<NotFoundResult>(result);
            mockService.Verify(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
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

            ExpenseResponse createdExpense = new()
            {
                Id = 1,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                Category = request.Category
            };

            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.CreateAsync(It.IsAny<CreateExpenseRequest>())).ReturnsAsync((ExpenseResponse?)createdExpense);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.Create(request);

            // Assert
            CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ExpensesController.GetAll), createdResult.ActionName);
            Assert.Equal(201, createdResult.StatusCode);
            mockService.Verify(s => s.CreateAsync(It.IsAny<CreateExpenseRequest>()), Times.Once());
        }

        [Fact]
        public async Task Update_WithValidId_ReturnsOkWithUpdatedExpense()
        {
            // Arrange
            int expenseId = 1;
            DateTime now = DateTime.UtcNow;
            UpdateExpenseRequest request = new()
            {
                Title = "Updated Lunch",
                Amount = 15.00m,
                Date = now,
                Category = "Food"
            };

            ExpenseResponse updatedExpense = new()
            {
                Id = expenseId,
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                Category = request.Category
            };

            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.UpdateAsync(expenseId, It.IsAny<UpdateExpenseRequest>(), It.IsAny<int>())).ReturnsAsync((ExpenseResponse?)updatedExpense);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.Update(expenseId, request);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            ExpenseResponse returnedExpense = Assert.IsType<ExpenseResponse>(okResult.Value);
            Assert.Equal(request.Title, returnedExpense.Title);
            mockService.Verify(s => s.UpdateAsync(expenseId, It.IsAny<UpdateExpenseRequest>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int expenseId = 999;
            UpdateExpenseRequest request = new()
            {
                Title = "Test",
                Amount = 10m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateExpenseRequest>(), It.IsAny<int>())).ReturnsAsync((ExpenseResponse?)null);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.Update(expenseId, request);

            // Assert
            _ = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            int expenseId = 1;
            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.DeleteAsync(expenseId, It.IsAny<int>())).ReturnsAsync(true);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.Delete(expenseId);

            // Assert
            _ = Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.DeleteAsync(expenseId, It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int expenseId = 999;
            Mock<IExpenseService> mockService = new(MockBehavior.Strict);
            _ = mockService.Setup(s => s.DeleteAsync(expenseId, It.IsAny<int>())).ReturnsAsync(false);

            ExpensesController controller = CreateController(mockService);

            // Act
            IActionResult result = await controller.Delete(expenseId);

            // Assert
            _ = Assert.IsType<NotFoundResult>(result);
            mockService.Verify(s => s.DeleteAsync(expenseId, It.IsAny<int>()), Times.Once());
        }
    }
}
