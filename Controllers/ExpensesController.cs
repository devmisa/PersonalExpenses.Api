using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using System.Security.Claims;

namespace PersonalExpenses.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController(IExpenseService expenseService) : ControllerBase
    {
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            ExpenseResponse expense = await expenseService.GetByIdAsync(id, CurrentUserId);

            return expense == null ? NotFound() : Ok(expense);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetExpensesQueryParams queryParams)
        {
            PaginatedResponse<ExpenseResponse> expenses = await expenseService.GetAllAsync(queryParams, CurrentUserId);
            return Ok(expenses);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
        {
            request = request with { UserId = CurrentUserId };
            ExpenseResponse createdExpense = await expenseService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = createdExpense.Id }, createdExpense);
        }

        [Authorize]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            ExpenseResponse updatedExpense = await expenseService.UpdateAsync(id, request, CurrentUserId);

            return updatedExpense == null ? NotFound() : Ok(updatedExpense);
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool response = await expenseService.DeleteAsync(id, CurrentUserId);

            return !response ? NotFound() : NoContent();
        }
    }
}