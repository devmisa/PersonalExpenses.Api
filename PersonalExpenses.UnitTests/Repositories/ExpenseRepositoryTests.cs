using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure.Data;
using PersonalExpenses.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PersonalExpenses.UnitTests.Repositories
{
    public class ExpenseRepositoryTests
    {
        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsExpense()
        {
            // Arrange
            int expenseId = 1;
            int userId = 1;
            Expense expense = new()
            {
                Id = expenseId,
                UserId = userId,
                Title = "Lunch",
                Amount = 12.50m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            Mock<DbSet<Expense>> mockSet = GetMockDbSet(new List<Expense> { expense }.AsQueryable());

            Mock<AppDbContext> mockContext = new(new DbContextOptions<AppDbContext>());
            _ = mockContext.Setup(m => m.Set<Expense>()).Returns(mockSet.Object);

            ExpenseRepository repository = new(mockContext.Object);

            // Act
            Expense result = await repository.GetByIdAsync(expenseId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expense.Id, result.Id);
            Assert.Equal(expense.Title, result.Title);
            Assert.Equal(expense.Amount, result.Amount);
            Assert.Equal(expense.Category, result.Category);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            Mock<DbSet<Expense>> mockSet = GetMockDbSet(new List<Expense>().AsQueryable());

            Mock<AppDbContext> mockContext = new(new DbContextOptions<AppDbContext>());
            _ = mockContext.Setup(m => m.Set<Expense>()).Returns(mockSet.Object);

            ExpenseRepository repository = new(mockContext.Object);

            // Act & Assert
            _ = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repository.GetByIdAsync(999, 1));
        }


        [Fact]
        public async Task AddAsync_WithValidExpense_ReturnsExpense()
        {
            // Arrange
            Expense expense = new()
            {
                Title = "Lunch",
                Amount = 12.50m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            Mock<DbSet<Expense>> mockSet = new();
            var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _ = mockContext.Setup(m => m.Set<Expense>()).Returns(mockSet.Object);

            ExpenseRepository repository = new(mockContext.Object);

            // Act
            var result = await repository.AddAsync(expense);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expense.Title, result.Title);
            Assert.Equal(expense.Amount, result.Amount);
            mockSet.Verify(m => m.AddAsync(expense, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidExpense_RemovesAndReturnsExpense()
        {
            // Arrange
            int expenseId = 1;
            int userId = 1;
            Expense existingExpense = new()
            {
                Id = expenseId,
                UserId = userId,
                Title = "Lunch",
                Amount = 12.50m,
                Date = DateTime.UtcNow,
                Category = "Food"
            };

            Mock<DbSet<Expense>> mockSet = GetMockDbSet(new List<Expense> { existingExpense }.AsQueryable());

            var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _ = mockContext.Setup(m => m.Set<Expense>()).Returns(mockSet.Object);

            ExpenseRepository repository = new(mockContext.Object);

            // Act
            var result = await repository.DeleteAsync(existingExpense);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingExpense.Id, result.Id);
            mockSet.Verify(m => m.Remove(existingExpense), Times.Once);
        }
                [Fact]
                public async Task SaveChangesAsync_CallsContextSaveChangesOnce()
        {
            // Arrange
            Mock<AppDbContext> mockContext = new(
                MockBehavior.Strict,
                new object[] { new DbContextOptions<AppDbContext>() }
            );
            _ = mockContext
                .Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            ExpenseRepository repository = new(mockContext.Object);

            // Act
            await repository.SaveChangesAsync();

            // Assert
            mockContext.Verify(
                m => m.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        private static Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> source) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            mock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<T>(source.GetEnumerator()));
            mock.As<IQueryable<T>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(source.Provider));
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
            return mock;
        }
    }

    internal class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
    {
        private static readonly MethodInfo _fromResultMethod =
            typeof(Task).GetMethod(nameof(Task.FromResult))!;

        public IQueryable CreateQuery(Expression expression) =>
            new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new TestAsyncEnumerable<TElement>(expression);

        public object? Execute(Expression expression) => inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var result = inner.Execute(expression);
            return (TResult)_fromResultMethod.MakeGenericMethod(resultType).Invoke(null, [result])!;
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default) =>
            new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    public class AsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator = enumerator;

        public T Current => _enumerator.Current;

        public async ValueTask<bool> MoveNextAsync()
        {
            return _enumerator.MoveNext();
        }

        public async ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
        }
    }
}
