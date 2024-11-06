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

        public VMResponse<List<VMBudgetPlan>> GetAll()
        {
            VMResponse<List<VMBudgetPlan>> response = new();

            try
            {
                response.Data = (
                        from bp in db.BudgetPlans
                        where bp.IsDelete == false
                        orderby bp.CreatedOn descending
                        select new VMBudgetPlan(bp)
                    ).ToList();

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
    }
}
