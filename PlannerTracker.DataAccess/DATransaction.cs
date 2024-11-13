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
    }
}
