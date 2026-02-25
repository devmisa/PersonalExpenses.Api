using PersonalExpenses.Application.Dtos;
using Xunit;

namespace PersonalExpenses.UnitTests.Dtos
{
    public class GetExpensesQueryParamsTests
    {
        [Fact]
        public void Constructor_SetsDefaultValues()
        {
            // Act
            GetExpensesQueryParams queryParams = new();

            // Assert
            Assert.Equal(1, queryParams.Page);
            Assert.Equal(20, queryParams.PageSize);
            Assert.Null(queryParams.Category);
        }

        [Fact]
        public void Validate_WithValidParameters_DoesNotChangeValues()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 2,
                PageSize = 50,
                Category = "Food"
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(2, queryParams.Page);
            Assert.Equal(50, queryParams.PageSize);
            Assert.Equal("Food", queryParams.Category);
        }

        [Fact]
        public void Validate_WithPageLessThanOne_SetsPageToOne()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 0,
                PageSize = 20
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(1, queryParams.Page);
        }

        [Fact]
        public void Validate_WithNegativePage_SetsPageToOne()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = -5,
                PageSize = 20
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(1, queryParams.Page);
        }

        [Fact]
        public void Validate_WithPageSizeLessThanOne_SetsPageSizeToTwenty()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 1,
                PageSize = 0
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(20, queryParams.PageSize);
        }

        [Fact]
        public void Validate_WithNegativePageSize_SetsPageSizeToTwenty()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 1,
                PageSize = -10
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(20, queryParams.PageSize);
        }

        [Fact]
        public void Validate_WithPageSizeGreaterThanHundred_SetsPageSizeToHundred()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 1,
                PageSize = 150
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(100, queryParams.PageSize);
        }

        [Fact]
        public void Validate_WithPageSizeExactlyHundred_DoesNotChange()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 1,
                PageSize = 100
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(100, queryParams.PageSize);
        }

        [Fact]
        public void Validate_WithCategoryFilter_PreservesCategory()
        {
            // Arrange
            string category = "Entertainment";
            GetExpensesQueryParams queryParams = new()
            {
                Page = 1,
                PageSize = 20,
                Category = category
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(category, queryParams.Category);
        }

        [Fact]
        public void Validate_WithNullCategory_RemainsNull()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = 1,
                PageSize = 20,
                Category = null
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Null(queryParams.Category);
        }

        [Fact]
        public void Validate_WithMultipleInvalidParameters_CorrectionAllValues()
        {
            // Arrange
            GetExpensesQueryParams queryParams = new()
            {
                Page = -1,
                PageSize = 200,
                Category = "Food"
            };

            // Act
            queryParams.Validate();

            // Assert
            Assert.Equal(1, queryParams.Page);
            Assert.Equal(100, queryParams.PageSize);
            Assert.Equal("Food", queryParams.Category);
        }
    }
}
