using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.Net;

namespace SmartSpender.DataAccess
{
    public class DATransaction
    {
        private readonly SmartSpenderContext _db;

        public DATransaction(SmartSpenderContext db)
        {
            _db = db;
        }

        public VMResponse<VMTransaction> CreateORM(VMTransactionReq req)
        {
            VMResponse<VMTransaction> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    if (req.Type.ToUpper() == "INCOME")
                    {
                        Income income = new Income()
                        {
                            Amount = req.Amount,
                            BudgetPlanId = req.BudgetPlanId,
                            CategoryId = req.CategoryId,
                            CreatedBy = req.ModifiedBy,
                            IncomeDate = req.Date,
                            Source = req.Name
                        };

                        _db.Add(income);
                        _db.SaveChanges();

                        List<string> tagsStr = req.Tag?.Split(",").ToList() ?? new();

                        if (tagsStr.Count > 0)
                        {
                            foreach (var item in tagsStr)
                            {
                                Tag? tag = _db.Tags.Where(t => t.TagName.ToLower() == item.ToLower()).FirstOrDefault();

                                if (tag == null)
                                {
                                    tag = new()
                                    {
                                        CreatedBy = req.ModifiedBy,
                                        CreatedOn = income.CreatedOn,
                                        TagName = item,
                                    };

                                    _db.Add(tag);
                                    _db.SaveChanges();
                                }

                                IncomeTag incomeTag = new()
                                {
                                    CreatedBy = req.ModifiedBy,
                                    CreatedOn = income.CreatedOn,
                                    IncomeId = income.Id,
                                    TagId = tag.Id,
                                };

                                _db.Add(incomeTag);
                                _db.SaveChanges();
                            }
                        }

                    }
                    else if (req.Type.ToUpper() == "EXPENSE")
                    {
                        Expense expense = new Expense()
                        {
                            Amount = req.Amount,
                            BudgetPlanId = req.BudgetPlanId,
                            CategoryId = req.CategoryId,
                            CreatedBy = req.ModifiedBy,
                            ExpenseDate = req.Date,
                            ExpenseName = req.Name,
                        };

                        _db.Add(expense);
                        _db.SaveChanges();

                        List<string> tagsStr = req.Tag?.Split(",").ToList() ?? new();

                        if (tagsStr.Count > 0)
                        {
                            foreach (var item in tagsStr)
                            {
                                Tag? tag = _db.Tags.Where(t => t.TagName.ToLower() == item.ToLower()).FirstOrDefault();

                                if (tag == null)
                                {
                                    tag = new()
                                    {
                                        CreatedBy = req.ModifiedBy,
                                        CreatedOn = expense.CreatedOn,
                                        TagName = item,
                                    };

                                    _db.Add(tag);
                                    _db.SaveChanges();
                                }

                                ExpenseTag expenseTag = new()
                                {
                                    CreatedBy = req.ModifiedBy,
                                    CreatedOn = expense.CreatedOn,
                                    ExpenseId = expense.Id,
                                    TagId = tag.Id,
                                };

                                _db.Add(expenseTag);
                                _db.SaveChanges();
                            }
                        }
                    }

                    dbTran.Commit();

                    //response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = "Successfully created transaction!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMTransaction>> CreateRaw(VMTransactionReq req)
        {
            VMResponse<VMTransaction> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    if (req.Type.ToUpper() == "INCOME")
                    {
                        Income? income = _db.Incomes
                            .FromSqlRaw("INSERT INTO incomes " +
                            "(amount, budget_plan_id, category_id, " +
                            "created_by, income_date, source) " +
                            "OUTPUT INSERTED.* " +
                            "VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                            req.Amount,
                            req.BudgetPlanId,
                            req.CategoryId,
                            req.ModifiedBy!,
                            req.Date,
                            req.Name)
                            .AsEnumerable()
                            .FirstOrDefault();

                        List<string> tagsStr = req.Tag?.Split(",").ToList() ?? new();

                        if (tagsStr.Count > 0)
                        {
                            foreach (var item in tagsStr)
                            {
                                Tag? tag = await _db.Tags
                                    .FromSqlRaw("SELECT * FROM tags " +
                                    "WHERE LOWER(tag_name) = {0}", item.ToLower())
                                    .FirstOrDefaultAsync();

                                if (tag == null)
                                {
                                    tag = _db.Tags
                                        .FromSqlRaw("INSERT INTO tags " +
                                        "(created_by, created_on, tag_name) " +
                                        "OUTPUT INSERTED.* " +
                                        "VALUES ({0}, {1}, {2})",
                                        req.ModifiedBy!,
                                        income!.CreatedOn!,
                                        item)
                                        .AsEnumerable()
                                        .FirstOrDefault();
                                }

                                var result = await _db.Database
                                    .ExecuteSqlRawAsync("INSERT INTO income_tags (created_by, created_on, income_id, tag_id) " +
                                    "VALUES ({0}, {1}, {2}, {3})",
                                    req.ModifiedBy!,
                                    income!.CreatedOn!,
                                    income.Id,
                                    tag!.Id);
                            }
                        }

                    }
                    else if (req.Type.ToUpper() == "EXPENSE")
                    {
                        Expense? expense = _db.Expenses
                            .FromSqlRaw("INSERT INTO expenses (amount, budget_plan_id, category_id, created_by, expense_date, expense_name) " +
                            "OUTPUT INSERTED.* " +
                            "VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                            req.Amount,
                            req.BudgetPlanId,
                            req.CategoryId,
                            req.ModifiedBy!,
                            req.Date,
                            req.Name)
                            .AsEnumerable()
                            .FirstOrDefault();

                        List<string> tagsStr = req.Tag?.Split(",").ToList() ?? new();

                        if (tagsStr.Count > 0)
                        {
                            foreach (var item in tagsStr)
                            {
                                Tag? tag = await _db.Tags
                                    .FromSqlRaw("SELECT * FROM tags " +
                                    "WHERE LOWER(tag_name) = {0}", item.ToLower())
                                    .FirstOrDefaultAsync();

                                if (tag == null)
                                {
                                    tag = _db.Tags
                                        .FromSqlRaw("INSERT INTO tags (created_by, created_on, tag_name) " +
                                        "OUTPUT INSERTED.* " +
                                        "VALUES ({0}, {1}, {2})",
                                        req.ModifiedBy!,
                                        expense!.CreatedOn!,
                                        item)
                                        .AsEnumerable()
                                        .FirstOrDefault();
                                }

                                var result = await _db.Database
                                    .ExecuteSqlRawAsync("INSERT INTO expense_tags (created_by, created_on, expense_id, tag_id) " +
                                    "VALUES ({0}, {1}, {2}, {3})",
                                    req.ModifiedBy!,
                                    expense!.CreatedOn!,
                                    expense.Id,
                                    tag!.Id);
                            }
                        }
                    }

                    dbTran.Commit();

                    //response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = "Successfully created transaction!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<List<VMTransaction>> FetchORM(string? filter)
        {
            VMResponse<List<VMTransaction>> response = new();

            try
            {
                List<VMTransaction> data = (
                        from i in _db.Incomes
                        join bp in _db.BudgetPlans on i.BudgetPlanId equals bp.Id
                        join c in _db.Categories on i.CategoryId equals c.Id
                        select new VMTransaction()
                        {
                            Amount = i.Amount,
                            BudgetPlan = bp.PlanName,
                            Category = c.CategoryName,
                            Date = i.IncomeDate,
                            Id = i.Id,
                            Name = i.Source,
                            Type = "Income",
                            Tag = (
                                from it in _db.IncomeTags
                                where it.IncomeId == i.Id
                                join t in _db.Tags on it.TagId equals t.Id
                                select t.TagName
                            ).ToList()
                        }
                    ).ToList();

                List<VMTransaction> dataExpense = (
                        from e in _db.Expenses
                        join bp in _db.BudgetPlans on e.BudgetPlanId equals bp.Id
                        join c in _db.Categories on e.CategoryId equals c.Id
                        select new VMTransaction()
                        {
                            Amount = e.Amount,
                            BudgetPlan = bp.PlanName,
                            Category = c.CategoryName,
                            Date = e.ExpenseDate,
                            Id = e.Id,
                            Name = e.ExpenseName,
                            Type = "Expense",
                            Tag = (
                                from et in _db.ExpenseTags
                                where et.ExpenseId == e.Id
                                join t in _db.Tags on et.TagId equals t.Id
                                select t.TagName
                            ).ToList()
                        }
                    ).ToList();

                data.AddRange(dataExpense);

                data = data.OrderByDescending(d => d.Date).ToList();
                if (!string.IsNullOrEmpty(filter))
                {
                    data = data.Where(dt =>
                        dt.BudgetPlan.ToLower().Contains(filter.ToLower()) ||
                        dt.Category.ToLower().Contains(filter.ToLower()) ||
                        dt.Type.ToLower().Contains(filter.ToLower()) ||
                        dt.Name.ToLower().Contains(filter.ToLower())
                    ).ToList();
                }

                response.Data = data;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        // TO BE COMPLETED
        public VMResponse<List<VMTransaction>> FetchRaw(string? filter, Guid? userId)
        {
            VMResponse<List<VMTransaction>> response = new();

            try
            {
                List<VMTransaction> data = (
                        from i in _db.Incomes
                        join bp in _db.BudgetPlans on i.BudgetPlanId equals bp.Id
                        join c in _db.Categories on i.CategoryId equals c.Id
                        where i.CreatedBy == userId
                        select new VMTransaction()
                        {
                            Amount = i.Amount,
                            BudgetPlan = bp.PlanName,
                            Category = c.CategoryName,
                            Date = i.IncomeDate,
                            Id = i.Id,
                            Name = i.Source,
                            Type = "Income",
                            Tag = (
                                from it in _db.IncomeTags
                                where it.IncomeId == i.Id
                                join t in _db.Tags on it.TagId equals t.Id
                                select t.TagName
                            ).ToList()
                        }
                    ).ToList();

                List<VMTransaction> dataExpense = (
                        from e in _db.Expenses
                        join bp in _db.BudgetPlans on e.BudgetPlanId equals bp.Id
                        join c in _db.Categories on e.CategoryId equals c.Id
                        where e.CreatedBy == userId
                        select new VMTransaction()
                        {
                            Amount = e.Amount,
                            BudgetPlan = bp.PlanName,
                            Category = c.CategoryName,
                            Date = e.ExpenseDate,
                            Id = e.Id,
                            Name = e.ExpenseName,
                            Type = "Expense",
                            Tag = (
                                from et in _db.ExpenseTags
                                where et.ExpenseId == e.Id
                                join t in _db.Tags on et.TagId equals t.Id
                                select t.TagName
                            ).ToList()
                        }
                    ).ToList();

                data.AddRange(dataExpense);

                data = data.OrderByDescending(d => d.Date).ToList();
                if (!string.IsNullOrEmpty(filter))
                {
                    data = data.Where(dt =>
                        dt.BudgetPlan.ToLower().Contains(filter.ToLower()) ||
                        dt.Category.ToLower().Contains(filter.ToLower()) ||
                        dt.Type.ToLower().Contains(filter.ToLower()) ||
                        dt.Name.ToLower().Contains(filter.ToLower())
                    ).ToList();
                }

                response.Data = data;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<List<VMTransactionCategory>> FetchExpenseByCategory()
        {
            VMResponse<List<VMTransactionCategory>> response = new();

            try
            {
                List<VMTransactionCategory> data = new();

                List<VMTransactionCategory> dataExpense = (
                        from e in _db.Expenses
                        join bp in _db.BudgetPlans on e.BudgetPlanId equals bp.Id
                        join c in _db.Categories on e.CategoryId equals c.Id
                        group e by new { c.Id, c.CategoryName } into grouped
                        select new VMTransactionCategory()
                        {
                            Amount = grouped.Sum(a => a.Amount),
                            Category = grouped.Key.CategoryName,
                            Id = grouped.Key.Id,
                            Type = "Expense",
                        }
                    ).ToList();

                data.AddRange(dataExpense);

                decimal totalEx = 0;
                data.ForEach(d => totalEx += d.Amount);
                data.ForEach(d => d.TotalAmount = totalEx);

                response.Data = data;
                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }
    }
}
