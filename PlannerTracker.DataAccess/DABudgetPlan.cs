using Microsoft.EntityFrameworkCore.Storage;
using PlannerTracker.DataModel;
using PlannerTracker.ViewModel;
using System.Net;

namespace PlannerTracker.DataAccess
{
    public class DABudgetPlan
    {
        private readonly PlannerTrackerContext db;

        public DABudgetPlan(PlannerTrackerContext _db)
        {
            this.db = _db;
        }

        public VMResponse<List<VMBudgetPlan>> GetAll(string? filter)
        {
            VMResponse<List<VMBudgetPlan>> response = new();

            try
            {
                response.Data = (
                        from bp in db.BudgetPlans
                        where bp.IsDelete == false && (
                            !string.IsNullOrEmpty(filter) ? bp.PlanName.ToLower().Contains(filter.ToLower()) : true
                        )
                        orderby bp.CreatedOn descending
                        select new VMBudgetPlan(bp)
                        {
                            TotalIncome = (
                                from i in db.Incomes
                                where i.IsDelete == false && i.BudgetPlanId == bp.Id
                                select i.Amount
                            ).Sum(),
                            TotalExpense = (
                                from e in db.Expenses
                                where e.IsDelete == false && e.BudgetPlanId == bp.Id
                                select e.Amount
                            ).Sum(),
                        }
                    ).ToList();

                response.Data.ForEach(bp => bp.Progress = ((int)bp.TotalIncome - (int)bp.TotalExpense) * 100 / (int)bp.TotalBudget);

                response.Data = response.Data.OrderByDescending(bp => bp.Progress).ToList();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Successfully fetched!";
            }
            catch (Exception ex)
            {
                response.Message = $"{response.StatusCode} - {ex.Message}";
            }

            return response;
        }

        public VMResponse<VMBudgetPlan> GetById(Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            try
            {
                response.Data = (
                        from bp in db.BudgetPlans
                        where bp.IsDelete == false && bp.Id == id
                        select new VMBudgetPlan(bp)
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

        public VMResponse<VMBudgetPlan> Create(VMBudgetPlanReq req)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
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

                    db.Add(budgetPlan);
                    db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMBudgetPlan(budgetPlan);
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

        public VMResponse<VMBudgetPlan> Update(VMBudgetPlanReq req, Guid id)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan? budgetPlan = db.BudgetPlans.Where(bp => bp.Id == id && bp.IsDelete == false).FirstOrDefault();

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

                    db.Update(budgetPlan);
                    db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = new VMBudgetPlan(budgetPlan);
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

        public VMResponse<VMBudgetPlan> Delete(Guid id, Guid userId)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    BudgetPlan? budgetPlan = db.BudgetPlans.Where(bp => bp.Id == id && bp.IsDelete == false).FirstOrDefault();

                    if (budgetPlan == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.Message = $"Plan id : {id} is not found!";
                        return response;
                    }

                    budgetPlan.IsDelete = true;
                    budgetPlan.DeletedBy = userId;
                    budgetPlan.DeletedOn = DateTime.Now;

                    db.Update(budgetPlan);
                    db.SaveChanges();

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Plan {budgetPlan.PlanName} is successfully deleted!";
                }
                catch (Exception ex)
                {
                    dbTran.Rollback();
                    response.Message = $"{response.StatusCode} - {ex.Message}";
                }
            }

            return response;
        }

        public VMResponse<VMBudgetPlan> DeleteMultiple(List<Guid> ids, Guid userId)
        {
            VMResponse<VMBudgetPlan> response = new();

            using (IDbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var id in ids)
                    {
                        BudgetPlan? budgetPlan = db.BudgetPlans.Where(bp => bp.Id == id && bp.IsDelete == false).FirstOrDefault();

                        if (budgetPlan == null)
                        {
                            response.StatusCode = HttpStatusCode.NotFound;
                            response.Message = $"Plan id : {id} is not found!";
                            return response;
                        }

                        budgetPlan.IsDelete = true;
                        budgetPlan.DeletedBy = userId;
                        budgetPlan.DeletedOn = DateTime.Now;

                        db.Update(budgetPlan);
                        db.SaveChanges();
                    }

                    dbTran.Commit();

                    // response
                    response.Data = default;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = $"Successfully deleted!";
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
