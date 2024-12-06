using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartSpender.DataModel;
using SmartSpender.ViewModel;
using System.Net;

namespace SmartSpender.DataAccess
{
    public class DABudgetPlan
    {
        private readonly SmartSpenderContext _db;

        public DABudgetPlan(SmartSpenderContext db)
        {
            _db = db;
        }

        public VMResponse<List<VMBudgetPlan>> FetchAllORM(string? filter, Guid userId)
        {
            VMResponse<List<VMBudgetPlan>> response = new();

            try
            {
                response.Data = (
                        from bp in _db.BudgetPlans
                        where bp.UserId == userId && !string.IsNullOrEmpty(filter)
                            ? bp.PlanName.ToLower().Contains(filter.ToLower())
                            : true && bp.EndDate >= DateTime.Now
                        orderby bp.CreatedOn descending
                        select new VMBudgetPlan()
                        {
                            TotalIncome = (
                                from i in _db.Incomes
                                where i.BudgetPlanId == bp.Id
                                select i.Amount
                            ).Sum(),
                            TotalExpense = (
                                from e in _db.Expenses
                                where e.BudgetPlanId == bp.Id
                                select e.Amount
                            ).Sum(),
                            CreatedBy = bp.CreatedBy,
                            CreatedOn = bp.CreatedOn,
                            EndDate = bp.EndDate,
                            Id = bp.Id,
                            ModifiedBy = bp.ModifiedBy,
                            ModifiedOn = bp.ModifiedOn,
                            PlanName = bp.PlanName,
                            StartDate = bp.StartDate,
                            TotalBudget = bp.TotalBudget,
                            UserId = bp.UserId,
                        }
                    ).ToList();

                response.Data.ForEach(bp =>
                    bp.Progress = ((int)bp.TotalIncome - (int)bp.TotalExpense) * 100
                        / (int)bp.TotalBudget);

                response.Data = response.Data
                    .OrderByDescending(bp => bp.Progress)
                    .ToList();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public async Task<VMResponse<List<VMBudgetPlan>>> FetchAllRaw(string? filter, Guid userId)
        {
            VMResponse<List<VMBudgetPlan>> response = new();

            try
            {
                string sql = "SELECT * FROM budget_plans " +
                    "WHERE user_id = {0} " +
                    "AND end_date >= {1} ";
                if (!string.IsNullOrEmpty(filter))
                {
                    sql += "AND LOWER(plan_name) LIKE '%{2}%' ";
                }
                sql += "ORDER BY created_on DESC";

                List<BudgetPlan> budgetPlans = await _db.BudgetPlans
                    .FromSqlRaw(
                        sql,
                        userId,
                        DateTime.Now,
                        filter ?? string.Empty)
                    .ToListAsync();

                // Native SQL queries
                var incomeData = await _db.Set<VMBudgetIncome>()
                    .FromSqlRaw("SELECT " +
                    "budget_plan_id AS BudgetPlanId, " +
                    "COALESCE(SUM(amount), 0) AS TotalIncome " +
                    "FROM incomes " +
                    "GROUP BY budget_plan_id")
                    .ToListAsync();

                var expenseData = await _db.Set<VMBudgetExpense>()
                    .FromSqlRaw("SELECT " +
                    "budget_plan_id AS BudgetPlanId, " +
                    "COALESCE(SUM(amount), 0) AS TotalExpense " +
                    "FROM expenses " +
                    "GROUP BY budget_plan_id")
                    .ToListAsync();

                // Map to VMBudgetPlan
                List<VMBudgetPlan> data = budgetPlans.Select(bp =>
                {
                    var totalIncome = incomeData.FirstOrDefault(i =>
                        i.BudgetPlanId == bp.Id)?.TotalIncome ?? 0;
                    var totalExpense = expenseData.FirstOrDefault(e =>
                        e.BudgetPlanId == bp.Id)?.TotalExpense ?? 0;

                    return new VMBudgetPlan()
                    {
                        TotalIncome = totalIncome,
                        TotalExpense = totalExpense,
                        CreatedBy = bp.CreatedBy,
                        CreatedOn = bp.CreatedOn,
                        EndDate = bp.EndDate,
                        Id = bp.Id,
                        ModifiedBy = bp.ModifiedBy,
                        ModifiedOn = bp.ModifiedOn,
                        PlanName = bp.PlanName,
                        StartDate = bp.StartDate,
                        TotalBudget = bp.TotalBudget,
                        UserId = bp.UserId,
                    };
                }).ToList();

                data.ForEach(bp =>
                    bp.Progress = ((int)bp.TotalIncome - (int)bp.TotalExpense) * 100
                        / (int)bp.TotalBudget);

                data = data.OrderByDescending(bp => bp.Progress).ToList();

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

        public VMResponse<VMBudgetPlan> FetchByIdORM(Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            try
            {
                response.Data = (
                        from bp in _db.BudgetPlans
                        where bp.Id == id
                        select new VMBudgetPlan()
                        {
                            CreatedBy = bp.CreatedBy,
                            CreatedOn = bp.CreatedOn,
                            EndDate = bp.EndDate,
                            Id = bp.Id,
                            ModifiedBy = bp.ModifiedBy,
                            ModifiedOn = bp.ModifiedOn,
                            PlanName = bp.PlanName,
                            StartDate = bp.StartDate,
                            TotalBudget = bp.TotalBudget,
                            UserId = bp.UserId,
                        }
                    ).FirstOrDefault();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public async Task<VMResponse<VMBudgetPlan>> FetchByIdRaw(Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            try
            {
                BudgetPlan? bp = await _db.BudgetPlans
                    .FromSqlRaw(
                        "SELECT * FROM budget_plans " +
                        "WHERE id = {0}", id)
                    .FirstOrDefaultAsync();

                if (bp != null)
                {
                    response.Data = new VMBudgetPlan()
                    {
                        CreatedBy = bp.CreatedBy,
                        CreatedOn = bp.CreatedOn,
                        EndDate = bp.EndDate,
                        Id = bp.Id,
                        ModifiedBy = bp.ModifiedBy,
                        ModifiedOn = bp.ModifiedOn,
                        PlanName = bp.PlanName,
                        StartDate = bp.StartDate,
                        TotalBudget = bp.TotalBudget,
                        UserId = bp.UserId,
                    };
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<VMBudgetPlan> CreateORM(VMBudgetPlanReq req)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan budgetPlan = new()
                    {
                        CreatedBy = req.ModifiedBy,
                        EndDate = req.EndDate,
                        PlanName = req.PlanName,
                        StartDate = req.StartDate,
                        TotalBudget = req.TotalBudget,
                        UserId = req.UserId,
                    };

                    _db.Add(budgetPlan);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMBudgetPlan()
                    {
                        CreatedBy = budgetPlan.CreatedBy,
                        CreatedOn = budgetPlan.CreatedOn,
                        EndDate = budgetPlan.EndDate,
                        Id = budgetPlan.Id,
                        ModifiedBy = budgetPlan.ModifiedBy,
                        ModifiedOn = budgetPlan.ModifiedOn,
                        PlanName = budgetPlan.PlanName,
                        StartDate = budgetPlan.StartDate,
                        TotalBudget = budgetPlan.TotalBudget,
                        UserId = budgetPlan.UserId
                    };
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Plan {budgetPlan.PlanName} is successfully created!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMBudgetPlan>> CreateRaw(VMBudgetPlanReq req)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    var result = await _db.Database
                        .ExecuteSqlRawAsync(
                            "INSERT INTO budget_plans " +
                            "(created_by, end_date, plan_name, " +
                            "start_date, total_budget, user_id) " +
                            "VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                            req.ModifiedBy!,
                            req.EndDate!,
                            req.PlanName,
                            req.StartDate!,
                            req.TotalBudget,
                            req.UserId);

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Message = $"Plan {req.PlanName} is successfully created!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<VMBudgetPlan> UpdateORM(VMBudgetPlanReq req, Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan? budgetPlan = _db.BudgetPlans
                        .Where(bp => bp.Id == id)
                        .FirstOrDefault();

                    if (budgetPlan == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Plan id : {id} is not found!";
                        return response;
                    }

                    budgetPlan.PlanName = req.PlanName;
                    budgetPlan.StartDate = req.StartDate;
                    budgetPlan.EndDate = req.EndDate;
                    budgetPlan.TotalBudget = req.TotalBudget;
                    budgetPlan.ModifiedBy = req.ModifiedBy;
                    budgetPlan.ModifiedOn = DateTime.Now;

                    _db.Update(budgetPlan);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMBudgetPlan()
                    {
                        CreatedBy = budgetPlan.CreatedBy,
                        CreatedOn = budgetPlan.CreatedOn,
                        EndDate = budgetPlan.EndDate,
                        Id = budgetPlan.Id,
                        ModifiedBy = budgetPlan.ModifiedBy,
                        ModifiedOn = budgetPlan.ModifiedOn,
                        PlanName = budgetPlan.PlanName,
                        StartDate = budgetPlan.StartDate,
                        TotalBudget = budgetPlan.TotalBudget,
                        UserId = budgetPlan.UserId
                    };
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Plan {budgetPlan.PlanName} is successfully updated!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMBudgetPlan>> UpdateRaw(VMBudgetPlanReq req, Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan? budgetPlan = await _db.BudgetPlans
                        .FromSqlRaw(
                            "SELECT * FROM budget_plans " +
                            "WHERE id = {0}", id)
                        .FirstOrDefaultAsync();

                    if (budgetPlan == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Plan id : {id} is not found!";
                        return response;
                    }

                    var result = await _db.Database
                        .ExecuteSqlRawAsync(
                            "UPDATE budget_plans " +
                            "SET plan_name = {0}, " +
                            "start_date = {1}, " +
                            "end_date = {2}, " +
                            "total_budget = {3}, " +
                            "modified_by = {4}, " +
                            "modified_on = {5} " +
                            "WHERE id = {6}",
                            req.PlanName,
                            req.StartDate!,
                            req.EndDate!,
                            req.TotalBudget,
                            req.ModifiedBy!,
                            DateTime.Now,
                            id);

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Plan {req.PlanName} is successfully updated!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<VMBudgetPlan> DeleteORM(Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan? budgetPlan = _db.BudgetPlans.Where(bp => bp.Id == id).FirstOrDefault();

                    if (budgetPlan == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Plan id : {id} is not found!";
                        return response;
                    }

                    _db.Remove(budgetPlan);
                    _db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Plan is successfully deleted!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public async Task<VMResponse<VMBudgetPlan>> DeleteRaw(Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = _db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan? budgetPlan = await _db.BudgetPlans
                        .FromSqlRaw(
                            "SELECT * FROM budget_plans " +
                            "WHERE id = {0}", id)
                        .FirstOrDefaultAsync();

                    if (budgetPlan == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Plan id : {id} is not found!";
                        return response;
                    }

                    var result = await _db.Database
                        .ExecuteSqlRawAsync(
                            "DELETE budget_plans " +
                            "WHERE id = {0}", id);

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Plan is successfully deleted!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }
    }
}
