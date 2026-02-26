using PersonalExpenses.Application.Dtos;
using System.Collections.Generic;
using Xunit;

namespace PersonalExpenses.UnitTests.Dtos
{
    public class PaginatedResponseTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaultValues()
        {
            // Act
            PaginatedResponse<ExpenseResponse> response = new();

            // Assert
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Equal(0, response.Page);
            Assert.Equal(0, response.PageSize);
            Assert.Equal(0, response.TotalCount);
        }

        [Fact]
        public void TotalPages_CalculatesCorrectly()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                PageSize = 20,
                TotalCount = 50
            };

            // Act
            int totalPages = response.TotalPages;

            // Assert
            Assert.Equal(3, totalPages); // (50 + 20 - 1) / 20 = 3
        }

        [Fact]
        public void TotalPages_WithExactDivision_CalculatesCorrectly()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                PageSize = 20,
                TotalCount = 60
            };

            // Act
            int totalPages = response.TotalPages;

            // Assert
            Assert.Equal(3, totalPages); // (60 + 20 - 1) / 20 = 3
        }

        [Fact]
        public void TotalPages_WithOneItem_ReturnsOne()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                PageSize = 20,
                TotalCount = 1
            };

            // Act
            int totalPages = response.TotalPages;

            // Assert
            Assert.Equal(1, totalPages);
        }

        [Fact]
        public void TotalPages_WithZeroItems_ReturnsZero()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                PageSize = 20,
                TotalCount = 0
            };

            // Act
            int totalPages = response.TotalPages;

            // Assert
            Assert.Equal(0, totalPages);
        }

        [Fact]
        public void HasNextPage_OnFirstPageWithMorePages_ReturnsTrue()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 1,
                PageSize = 20,
                TotalCount = 50
            };

            // Act
            bool hasNextPage = response.HasNextPage;

            // Assert
            Assert.True(hasNextPage);
        }

        [Fact]
        public void HasNextPage_OnLastPage_ReturnsFalse()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 3,
                PageSize = 20,
                TotalCount = 50
            };

            // Act
            bool hasNextPage = response.HasNextPage;

            // Assert
            Assert.False(hasNextPage);
        }

        [Fact]
        public void HasNextPage_OnOnlyPage_ReturnsFalse()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 1,
                PageSize = 20,
                TotalCount = 10
            };

            // Act
            bool hasNextPage = response.HasNextPage;

            // Assert
            Assert.False(hasNextPage);
        }

        [Fact]
        public void HasPreviousPage_OnFirstPage_ReturnsFalse()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 1,
                PageSize = 20,
                TotalCount = 50
            };

            // Act
            bool hasPreviousPage = response.HasPreviousPage;

            // Assert
            Assert.False(hasPreviousPage);
        }

        [Fact]
        public void HasPreviousPage_OnSecondPage_ReturnsTrue()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 2,
                PageSize = 20,
                TotalCount = 50
            };

            // Act
            bool hasPreviousPage = response.HasPreviousPage;

            // Assert
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void HasPreviousPage_OnLastPage_ReturnsTrue()
        {
            // Arrange
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 3,
                PageSize = 20,
                TotalCount = 50
            };

            // Act
            bool hasPreviousPage = response.HasPreviousPage;

            // Assert
            Assert.True(hasPreviousPage);
        }

        [Fact]
        public void PopulateData_WithExpenses_StoresCorrectly()
        {
            // Arrange
            List<ExpenseResponse> expenses =
            [
                new() { Id = 1, Title = "Lunch", Amount = 12.50m, Category = "Food" },
                new() { Id = 2, Title = "Taxi", Amount = 8.75m, Category = "Transport" }
            ];

            PaginatedResponse<ExpenseResponse> response = new()
            {
                Data = expenses,
                Page = 1,
                PageSize = 20,
                TotalCount = 2
            };

            // Assert
            Assert.Equal(2, response.Data.Count);
            Assert.Equal(1, response.Data[0].Id);
            Assert.Equal("Lunch", response.Data[0].Title);
            Assert.Equal("Taxi", response.Data[1].Title);
        }

        [Fact]
        public void FullPaginationScenario_MiddlePage()
        {
            // Arrange & Act
            PaginatedResponse<ExpenseResponse> response = new()
            {
                Page = 2,
                PageSize = 10,
                TotalCount = 35
            };

            // Assert
            Assert.Equal(2, response.Page);
            Assert.Equal(10, response.PageSize);
            Assert.Equal(35, response.TotalCount);
            Assert.Equal(4, response.TotalPages);
            Assert.True(response.HasNextPage);
            Assert.True(response.HasPreviousPage);
        }
    }
}
