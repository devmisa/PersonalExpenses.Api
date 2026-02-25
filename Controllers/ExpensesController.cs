using Microsoft.AspNetCore.Mvc;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;

namespace PersonalExpenses.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController(IExpenseService expenseService) : ControllerBase
    {

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            ExpenseResponse expense = await expenseService.GetByIdAsync(id);

            return expense == null ? NotFound() : Ok(expense);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetExpensesQueryParams queryParams)
        {
            PaginatedResponse<ExpenseResponse> expenses = await expenseService.GetAllAsync(queryParams);
            return Ok(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
        {
            ExpenseResponse createdExpense = await expenseService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = createdExpense.Id }, createdExpense);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            ExpenseResponse updatedExpense = await expenseService.UpdateAsync(id, request);

            return updatedExpense == null ? NotFound() : Ok(updatedExpense);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool response = await expenseService.DeleteAsync(id);

            return !response ? NotFound() : NoContent();
        }
    }
}