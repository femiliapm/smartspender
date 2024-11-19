using Microsoft.EntityFrameworkCore.Storage;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.Net;

namespace PlannerTracker.DataAccess
{
    public class DATransaction
    {
        private readonly PlannerTrackerContext db;

        public DATransaction(PlannerTrackerContext db)
        {
            this.db = db;
        }

        public VMResponse<VMTransaction> Create(VMTransactionReq req)
        {
            VMResponse<VMTransaction> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
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

                        db.Add(income);
                        db.SaveChanges();

                        List<string> tagsStr = req.Tag?.Split(",").ToList() ?? new();

                        if (tagsStr.Count > 0)
                        {
                            foreach (var item in tagsStr)
                            {
                                Tag? tag = db.Tags.Where(t => t.IsDelete == false && t.TagName.ToLower() == item.ToLower()).FirstOrDefault();

                                if (tag == null)
                                {
                                    tag = new()
                                    {
                                        CreatedBy = req.ModifiedBy,
                                        CreatedOn = income.CreatedOn,
                                        TagName = item,
                                    };

                                    db.Add(tag);
                                    db.SaveChanges();
                                }

                                IncomeTag incomeTag = new()
                                {
                                    CreatedBy = req.ModifiedBy,
                                    CreatedOn = income.CreatedOn,
                                    IncomeId = income.Id,
                                    TagId = tag.Id,
                                };

                                db.Add(incomeTag);
                                db.SaveChanges();
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

                        db.Add(expense);
                        db.SaveChanges();

                        List<string> tagsStr = req.Tag?.Split(",").ToList() ?? new();

                        if (tagsStr.Count > 0)
                        {
                            foreach (var item in tagsStr)
                            {
                                Tag? tag = db.Tags.Where(t => t.IsDelete == false && t.TagName.ToLower() == item.ToLower()).FirstOrDefault();

                                if (tag == null)
                                {
                                    tag = new()
                                    {
                                        CreatedBy = req.ModifiedBy,
                                        CreatedOn = expense.CreatedOn,
                                        TagName = item,
                                    };

                                    db.Add(tag);
                                    db.SaveChanges();
                                }

                                ExpenseTag expenseTag = new()
                                {
                                    CreatedBy = req.ModifiedBy,
                                    CreatedOn = expense.CreatedOn,
                                    ExpenseId = expense.Id,
                                    TagId = tag.Id,
                                };

                                db.Add(expenseTag);
                                db.SaveChanges();
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

        public VMResponse<List<VMTransaction>> Fetch(string? filter)
        {
            VMResponse<List<VMTransaction>> response = new();

            try
            {
                List<VMTransaction> data = (
                        from i in db.Incomes
                        where i.IsDelete == false
                        join bp in db.BudgetPlans on i.BudgetPlanId equals bp.Id
                        join c in db.Categories on i.CategoryId equals c.Id
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
                                from it in db.IncomeTags
                                where it.IsDelete == false && it.IncomeId == i.Id
                                join t in db.Tags on it.TagId equals t.Id
                                select t.TagName
                            ).ToList()
                        }
                    ).ToList();

                List<VMTransaction> dataExpense = (
                        from e in db.Expenses
                        where e.IsDelete == false
                        join bp in db.BudgetPlans on e.BudgetPlanId equals bp.Id
                        join c in db.Categories on e.CategoryId equals c.Id
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
                                from et in db.ExpenseTags
                                where et.IsDelete == false && et.ExpenseId == e.Id
                                join t in db.Tags on et.TagId equals t.Id
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
                        from e in db.Expenses
                        where e.IsDelete == false
                        join bp in db.BudgetPlans on e.BudgetPlanId equals bp.Id
                        join c in db.Categories on e.CategoryId equals c.Id
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
