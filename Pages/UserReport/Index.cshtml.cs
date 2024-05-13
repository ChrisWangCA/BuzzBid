using BuzzBid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace BuzzBid.Pages.UserReport
{
    [Authorize]
    public class IndexModel : PageModel
    {
		private readonly UserManager _userManager;
		public List<UserReportModel> UserReports { get; set; }

		private readonly BuzzBidContext _context;

		String getSoldNum = "SELECT COUNT(*) " +
							"FROM Item " +
                            "WHERE Item.ListBy = '{0}'  AND Item.WinDate IS NOT NULL";

        String getWonNum = "SELECT COUNT(*) " +
							"FROM Item " +
							"WHERE Winner = '{0}'";

		String getRatedNum = "SELECT COUNT(*) " +
							 "FROM Rating INNER JOIN Item on Rating.ItemId = Item.ItemId " +
							 "WHERE Winner = '{0}'";

        String getMostFreqCondition =   "SELECT TOP(1) Condition " +
                                        "FROM Item " +
                                        "WHERE ListBy = '{0}' " +
                                        "GROUP BY Condition " +
                                        "ORDER BY Count(Condition) desc, Condition asc;";

        public IndexModel(BuzzBidContext db, UserManager userManager)
		{
			this._context = db;
			_userManager = userManager;
		}
        public async Task<IActionResult> OnGetAsync()
        {
            Debug.WriteLine(_userManager.IsAdminUser() + "hahha");
            UserReports = new List<UserReportModel>();


            var getListedData = @"SELECT COUNT(ListBy), [User].UserName 
                                FROM Item right join [User] on Item.ListBy = [User].UserName 
                                Group By [User].UserName 
                                ORDER BY COUNT(ListBy) DESC;";




            DBService svc = new DBService();
            var ds = svc.ExecuteSql(getListedData);
            DataRowCollection categoryTable = ds.Tables[0].Rows;

            foreach (DataRow row in categoryTable)
            {
                UserReportModel userReportModel = new UserReportModel();
                userReportModel.userName = row[1].ToString();
                userReportModel.listedNum = row[0].ToString();
                Debug.WriteLine(userReportModel.userName);
                Debug.WriteLine(userReportModel.listedNum);

                UserReports.Add(userReportModel);
            }



            for (int i = 0; i < categoryTable.Count; i++)
            {

                Debug.WriteLine("UID: " + UserReports[i].userName);
                ds = svc.ExecuteSql(String.Format(getSoldNum, UserReports[i].userName.Replace("'", "''")));
                UserReports[i].soldNum = ds.Tables[0].Rows[0][0].ToString();

                ds = svc.ExecuteSql(String.Format(getWonNum, UserReports[i].userName.Replace("'", "''")));
                UserReports[i].wonNum = ds.Tables[0].Rows[0][0].ToString();

                ds = svc.ExecuteSql(String.Format(getRatedNum, UserReports[i].userName.Replace("'", "''")));
                UserReports[i].ratedNum = ds.Tables[0].Rows[0][0].ToString();

                ds = svc.ExecuteSql(String.Format(getMostFreqCondition, UserReports[i].userName.Replace("'", "''")));
                UserReports[i].mostFreqCondition = GetConditionString(ds.Tables[0].Rows.Count > 0 ? (int)ds.Tables[0].Rows[0][0] : -1);

            }
            if (!_userManager.IsAdminUser())
            {
                return RedirectToPage("/MainMenu");
            }
            return Page();

            
    
        }




        private String GetConditionString(int condition)
        {
            if (condition == 1)
            {
                return "Poor";
            }
            else if (condition == 2)
            {
                return "Fair";
            }
            else if (condition == 3)
            {
                return "Good";
            }
            else if (condition == 4)
            {
                return "Very Good";
            }
            else if(condition == 5)
            {
                return "New";
            }
            else
            {
                return "N/A";
            }
        }
    }
}
